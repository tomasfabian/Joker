using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Autofac;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using SelfHostedODataService.Configuration;
using SelfHostedODataService.SignalR.Hubs;
using StackExchange.Redis;

namespace SelfHostedODataService
{
  public class StartupForBlazorAndOData : StartupBaseWithOData
  {
    public StartupForBlazorAndOData(IWebHostEnvironment env) 
      : base(env)
    {
    }

    protected override void RegisterTypes(ContainerBuilder builder)
    {
      base.RegisterTypes(builder);

      builder
        .Register(ctx => ConnectionMultiplexer.Connect(ctx.Resolve<IProductsConfigurationProvider>().RedisUrl))
        .As<IConnectionMultiplexer>()
        .SingleInstance();
    }

    protected override void OnConfigureServices(IServiceCollection services)
    {
      base.OnConfigureServices(services);

      services.AddControllersWithViews();
      services.AddRazorPages();

      ConfigureSignalR(services);

      services.AddResponseCompression(opts =>
      {
        opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
          new[] { "application/octet-stream" });
      });
    }

    protected override void OnConfigureApp(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime applicationLifetime)
    {
      base.OnConfigureApp(app, env, applicationLifetime);

      if (env.IsDevelopment())
      {
        app.UseWebAssemblyDebugging();
      }

      app.UseResponseCompression();

      app.UseBlazorFrameworkFiles();
      app.UseStaticFiles();
    }

    protected override void OnUseEndpoints(IEndpointRouteBuilder endpoints)
    {
      base.OnUseEndpoints(endpoints);

      endpoints.MapHub<DataChangesHub>("/dataChangesHub");
      
      endpoints.MapGet("generateToken", c => c.Response.WriteAsync(GenerateJwtToken(c)));

      endpoints.MapRazorPages();
      endpoints.MapControllers();
      endpoints.MapFallbackToFile("index.html");
    }

    #region ConfigureSignalR

    private readonly SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Guid.NewGuid().ToByteArray());
    private readonly JwtSecurityTokenHandler jwtTokenHandler = new JwtSecurityTokenHandler();

    private void ConfigureSignalR(IServiceCollection services)
    {
      services.AddSignalR();

      services.AddAuthorization(options =>
      {
        options.AddPolicy(JwtBearerDefaults.AuthenticationScheme, policy =>
        {
          policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
          policy.RequireClaim(ClaimTypes.NameIdentifier);
        });
      });

      services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
          options.TokenValidationParameters =
            new TokenValidationParameters
            {
              LifetimeValidator = (before, expires, token, parameters) => expires > DateTime.UtcNow,
              ValidateAudience = false,
              ValidateIssuer = false,
              ValidateActor = false,
              ValidateLifetime = true,
              IssuerSigningKey = securityKey
            };
        });
    }

    #endregion

    #region GenerateJwtToken

    private string GenerateJwtToken(HttpContext httpContext)
    {
      var claims = new[] { new Claim(ClaimTypes.NameIdentifier, httpContext.Request.Query["user"]) };
      var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
      var token = new JwtSecurityToken("SignalR.Blazor", "Blazor", claims, expires: DateTime.UtcNow.AddMinutes(30), signingCredentials: credentials);

      return jwtTokenHandler.WriteToken(token);
    }

    #endregion
  }
}
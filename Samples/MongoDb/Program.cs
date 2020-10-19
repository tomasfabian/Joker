using System.Threading.Tasks;
using Joker.OData.Hosting;

namespace Joker.AspNetCore.MongoDb
{
  public class Program
  {
    public static async Task Main(string[] args)
    {
      var webHostConfig = new IISWebHostConfig();
      
      await new ApiHost<ApiStartup>().RunAsync(args, webHostConfig);
    }
  }
}
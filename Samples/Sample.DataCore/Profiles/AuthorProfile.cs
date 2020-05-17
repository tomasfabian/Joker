using AutoMapper;
using Sample.Domain.DTO;
using Sample.Domain.Models;

namespace Sample.DataCore.Profiles
{
  public class AuthorProfile : Profile
  {
    public AuthorProfile()
    {
      CreateMap<Author, AuthorDto>();
    }
  }
}
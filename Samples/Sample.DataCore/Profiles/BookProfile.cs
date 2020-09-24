using AutoMapper;
using Sample.Domain.DTO;
using Sample.Domain.Models;

namespace Sample.DataCore.Profiles
{
  public class BookProfile : Profile
  {
    public BookProfile()
    {
      CreateMap<Book, BookDto>();
    }
  }
}
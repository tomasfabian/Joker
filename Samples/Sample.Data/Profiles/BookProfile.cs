using AutoMapper;
using Sample.Domain.DTO;
using Sample.Domain.Models;

namespace Sample.Data.Profiles
{
  public class BookProfile : Profile
  {
    public BookProfile()
    {
      CreateMap<Book, BookDto>()
        .ForPath(c => c.Publisher.Books, c => c.Ignore());
    }
  }
}
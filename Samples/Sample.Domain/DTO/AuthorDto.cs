using System.Collections.Generic;

namespace Sample.Domain.DTO
{
  public class AuthorDto
  {
    public string LastName { get; set; }    
    
    public List<BookDto> Books { get; set; }
  }
}
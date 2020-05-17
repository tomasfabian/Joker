using System.Collections.Generic;

namespace Sample.Domain.DTO
{
  public class PublisherDto
  {
    public string Title { get; set; }    

    public List<BookDto> Books { get; set; }
  }
}
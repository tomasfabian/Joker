using System.Collections.Generic;

namespace Sample.Domain.DTO
{
  public class BookDto
  {
    public string Title { get; set; }    

    public PublisherDto Publisher { get; set; }
  }
}
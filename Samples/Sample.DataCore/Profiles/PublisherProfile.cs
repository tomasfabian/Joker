using AutoMapper;
using Sample.Domain.DTO;
using Sample.Domain.Models;

namespace Sample.DataCore.Profiles
{
  public class PublisherProfile : Profile
  {
    public PublisherProfile()
    {
      CreateMap<Publisher, PublisherDto>();
    }
  }
}
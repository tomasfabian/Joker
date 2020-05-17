using System;
using System.Data.Entity;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Joker.Contracts.Data;
using Sample.Domain.DTO;
using Sample.Domain.Models;

namespace Sample.Data.Repositories
{
  public class BooksMappedRepository : IReadOnlyRepository<BookDto>
  {
    private readonly IMapper mapper;
    private readonly IRepository<Book> booksRepository;

    public BooksMappedRepository(IMapper mapper, IRepository<Book> booksRepository)
    {
      this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
      this.booksRepository = booksRepository ?? throw new ArgumentNullException(nameof(booksRepository));
    }

    public IQueryable<BookDto> GetAll()
    {
      return booksRepository.GetAll().ProjectTo<BookDto>(mapper.ConfigurationProvider);
    }

    public IQueryable<BookDto> GetAllIncluding(string path)
    {
      return booksRepository.GetAll().Include(path).ProjectTo<BookDto>(mapper.ConfigurationProvider);
    }
  }
}
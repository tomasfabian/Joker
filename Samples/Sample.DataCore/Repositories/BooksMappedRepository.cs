using System;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Joker.Contracts.Data;
using Microsoft.EntityFrameworkCore;
using Sample.Domain.DTO;

namespace Sample.DataCore.Repositories
{
  public class BooksMappedRepository : IReadOnlyRepository<BookDto>
  {
    private readonly IMapper mapper;
    private readonly BooksRepository booksRepository;

    public BooksMappedRepository(IMapper mapper, BooksRepository booksRepository)
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
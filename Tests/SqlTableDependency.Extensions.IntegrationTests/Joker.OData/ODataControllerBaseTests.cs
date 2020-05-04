using FluentAssertions;
using Microsoft.OData.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sample.Domain.Models;
using System;
using System.Configuration;
using System.Data.Entity.Infrastructure.Pluralization;
using System.Linq;
using System.Threading.Tasks;
using UnitTests;

namespace SqlTableDependency.Extensions.IntegrationTests.Dev.Joker.OData
{
  [TestClass]
  public class ODataControllerBaseTests : TestBase
  {
    #region Fields

    private static readonly string ODataUrl = ConfigurationManager.AppSettings["ODataUrl"];

    private const string IntegrationTests = "OData.IntegrationTests";

    private static readonly EnglishPluralizationService EnglishPluralizationService = new EnglishPluralizationService();
    
    private ODataServiceContext dataServiceContext;

    #endregion

    #region Methods

    [TestInitialize]
    public override void TestInitialize()
    {
      base.TestInitialize();
      
      dataServiceContext = CreateDataServiceContext();
    }

    private static ODataServiceContext CreateDataServiceContext()
    {
      return new ODataServiceContext(new Uri(ODataUrl))
      {
        MergeOption = MergeOption.OverwriteChanges
      };
    }

    private string Pluralize<TEntity>()
    {
      return EnglishPluralizationService.Pluralize(typeof(TEntity).Name);
    }

    [TestMethod]
    [TestCategory(IntegrationTests)]
    [DataRow(SaveChangesOptions.None)]
    [DataRow(SaveChangesOptions.BatchWithSingleChangeset)]
    public async Task CRUD(SaveChangesOptions saveChangesOptions)
    {
      var entitiesCount = await dataServiceContext.Books.EntitiesCount();

      var book = new Book {Id = "Id" + RandomInt(), Title = "OData"};
      
      dataServiceContext.AddObject(book);

      await dataServiceContext.SaveChangesAsync(saveChangesOptions);

      var refreshedBooksCount = await dataServiceContext.Books.EntitiesCount();

      refreshedBooksCount.Should().Be(entitiesCount + 1, "New book was inserted to database");

      string changedTitle = book.Title + " renamed";
      book.Title = changedTitle;
      dataServiceContext.UpdateObject(book);

      await dataServiceContext.SaveChangesAsync(saveChangesOptions);

      var refreshedBook = await GetBookAsync(book.Id);

      refreshedBook.Should().NotBeNull("Book was saved to database");
      refreshedBook.Title.Should().Be(changedTitle, "Existing book's title was updated in the database");

      dataServiceContext.DeleteObject(book);

      await dataServiceContext.SaveChangesAsync(saveChangesOptions);

      refreshedBooksCount = await dataServiceContext.Books.EntitiesCount();

      refreshedBooksCount.Should().Be(entitiesCount, "Existing book was deleted from database");
    }

    [TestMethod]
    [TestCategory(IntegrationTests)]
    [DataRow(SaveChangesOptions.None)]
    // [DataRow(SaveChangesOptions.BatchWithSingleChangeset)]
    public async Task References(SaveChangesOptions saveChangesOptions)
    {
      var book = new Book() {Id = "Id" + RandomInt(), Title = "Title " + RandomInt()};
      var publisher = new Publisher() {Title = "New Publisher " + RandomInt()};
      var author = new Author() {LastName = "New Publisher " + RandomInt()};
      
      dataServiceContext.AddObject(author);
      dataServiceContext.AddObject(publisher);
      dataServiceContext.AddObject(book);

      dataServiceContext.AddLink(publisher, Pluralize<Book>(), book);
      dataServiceContext.AddLink(author, Pluralize<Book>(), book);

      var dataServiceResponse = await dataServiceContext.SaveChangesAsync(saveChangesOptions);

      dataServiceResponse.Select(c => c.StatusCode).SequenceEqual(new[] {201, 201, 204});

      var authorWithBooks = await GetAuthorAsync(author.Id);
      authorWithBooks.Should().NotBeNull("Author was saved");
      authorWithBooks.Books.Any().Should().BeTrue("Book was added to author");

      var publisherWithBooks = await GetPublisherAsync(publisher);
      publisherWithBooks.Should().NotBeNull("Publisher was saved");
      publisherWithBooks.Books.Any().Should().BeTrue("Book was added to publisher");

      var bookWithPublisher = await GetBookAsync(book.Id);
      bookWithPublisher.Publisher.Should().NotBeNull("Reference was added");

      dataServiceContext.DeleteLink(publisher, "Books", book);

      dataServiceResponse = await dataServiceContext.SaveChangesAsync(saveChangesOptions);
      bookWithPublisher = await GetBookAsync(book.Id);
      bookWithPublisher.Publisher.Should().BeNull("Reference was removed");

      dataServiceContext.DeleteObject(book);
      dataServiceContext.DeleteObject(author);
      dataServiceContext.DeleteObject(publisher);
      dataServiceResponse = await dataServiceContext.SaveChangesAsync(saveChangesOptions);

      (await GetPublisherAsync(publisher)).Should().BeNull("Publisher was deleted");
    }

    private async Task<Author> GetAuthorAsync(int id)
    {
      var author = await CreateDataServiceContext().Authors.Expand(c => c.Books).FirstOrDefaultAsync(c => c.Id == id);

      return author;
    }

    private async Task<Book> GetBookAsync(string id)
    {
      var book = await CreateDataServiceContext().Books.Expand(c => c.Authors).Expand(c => c.Publisher.Books)
        .FirstOrDefaultAsync(c => c.Id == id);

      return book;
    }

    private async Task<Publisher> GetPublisherAsync(Publisher publisher)
    {
      var refreshedPublisher = await CreateDataServiceContext()
        .Publishers.Expand(c => c.Books)
        .FirstOrDefaultAsync(c => c.PublisherId1 == publisher.PublisherId1 && c.PublisherId2 == publisher.PublisherId2);

      return refreshedPublisher;
    }

    [TestMethod]
    public async Task SetLink_ThenRemoveLink_ThenUpdate_ThenDelete_CompoundKey()
    {
      var book = new Book() {Id = "Id" + RandomInt(), Title = "Title " + RandomInt()};
      var publisher = new Publisher() {Title = "New Publisher " + RandomInt()};

      dataServiceContext.AddObject(book);
      dataServiceContext.AddObject(publisher);

      dataServiceContext.SetLink(book, nameof(Book.Publisher), publisher);

      var dataServiceResponse = await dataServiceContext.SaveChangesAsync(SaveChangesOptions.None);
      
      var bookWithPublisher = await GetBookAsync(book.Id);
      bookWithPublisher.Should().NotBeNull("Book was saved");
      bookWithPublisher.Publisher.PublisherId1.Should().Be(publisher.PublisherId1, "Compound key1");
      bookWithPublisher.Publisher.PublisherId2.Should().Be(publisher.PublisherId2, "Compound key2");

      dataServiceContext.SetLink(book, nameof(Book.Publisher), null);

      dataServiceResponse = await dataServiceContext.SaveChangesAsync(SaveChangesOptions.None);
      
      var refreshedBookWithoutPublisher = await GetBookAsync(book.Id);
      refreshedBookWithoutPublisher.Publisher.Should().BeNull("Book was removed from publisher");

      publisher.Title = publisher.Title + " Changed";

      dataServiceContext.UpdateObject(publisher);
      dataServiceResponse = await dataServiceContext.SaveChangesAsync(SaveChangesOptions.None);
      
      var refreshedPublisher = await GetPublisherAsync(publisher);
      refreshedPublisher.Title.Should().Be(publisher.Title, "Publishers title was updated");
      refreshedPublisher.Books.Any().Should().BeFalse("Books were removed from publisher");

      dataServiceContext.DeleteObject(publisher);
      dataServiceContext.DeleteObject(book);
      dataServiceResponse = await dataServiceContext.SaveChangesAsync(SaveChangesOptions.None);

      var wasPublisherDeleted = await GetPublisherAsync(publisher) == null;
      wasPublisherDeleted.Should().BeTrue("Publisher was deleted");
      var wasBookDeleted = await GetBookAsync(book.Id) == null;
      wasBookDeleted.Should().BeTrue("Book was deleted");
    }

    private static int RandomInt(int maxValue = 10000000)
    {
      return new Random().Next(1, maxValue);
    }

    [TestMethod]
    [ExpectedException(typeof(DataServiceRequestException))]
    public async Task Batch_UniqueKeyViolation_Throws()
    {
      var productsCount = await dataServiceContext.Authors.EntitiesCount();

      dataServiceContext.AddObject(new Author() {LastName = new Random().Next(1, 100000).ToString()});
      dataServiceContext.AddObject(new Author() {LastName = "Asimov"});
      dataServiceContext.AddObject(new Author() {LastName = new Random().Next(1, 100000).ToString()});

      var dataServiceResponse = await dataServiceContext.SaveChangesAsync(SaveChangesOptions.BatchWithSingleChangeset);
    }

    [TestMethod]
    public async Task Batch_UniqueKeyViolation()
    {
      var productsCount = await dataServiceContext.Authors.EntitiesCount();

      var author1 = new Author {LastName = new Random().Next(1, 100000).ToString()};
      var author2 = new Author {LastName = new Random().Next(1, 100000).ToString()};
      dataServiceContext.AddObject(author1);
      dataServiceContext.AddObject(author2);

      var dataServiceResponse = await dataServiceContext.SaveChangesAsync(SaveChangesOptions.BatchWithSingleChangeset);
      
      var newProductsCount = await dataServiceContext.Authors.EntitiesCount();

      newProductsCount.Should().Be(productsCount + 2, "Two books were added in a single batch");

      dataServiceContext.DeleteObject(author2);
      dataServiceContext.DeleteObject(author1);

      await dataServiceContext.SaveChangesAsync(SaveChangesOptions.BatchWithSingleChangeset);
    }

    [TestCleanup]
    public void CleanUp()
    {
      dataServiceContext = null;
    }

    #endregion
  }
}
namespace Sample.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Books_And_Authors : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Authors",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LastName = c.String(nullable: false, maxLength: 128),
                        Timestamp = c.DateTime(nullable: false, precision: 0, storeType: "datetime2", defaultValueSql: "GetDate()"),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.LastName, unique: true, name: "UX_Author_LastName");
            
            CreateTable(
                "dbo.Books",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Title = c.String(),
                        Timestamp = c.DateTime(nullable: false, precision: 0, storeType: "datetime2", defaultValueSql: "GetDate()"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.BookAuthors",
                c => new
                    {
                        Book_Id = c.String(nullable: false, maxLength: 128),
                        Author_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Book_Id, t.Author_Id })
                .ForeignKey("dbo.Books", t => t.Book_Id, cascadeDelete: true)
                .ForeignKey("dbo.Authors", t => t.Author_Id, cascadeDelete: true)
                .Index(t => t.Book_Id)
                .Index(t => t.Author_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BookAuthors", "Author_Id", "dbo.Authors");
            DropForeignKey("dbo.BookAuthors", "Book_Id", "dbo.Books");
            DropIndex("dbo.BookAuthors", new[] { "Author_Id" });
            DropIndex("dbo.BookAuthors", new[] { "Book_Id" });
            DropIndex("dbo.Authors", "UX_Author_LastName");
            DropTable("dbo.BookAuthors");
            DropTable("dbo.Books");
            DropTable("dbo.Authors");
        }
    }
}

namespace Sample.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Book_Publisher : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Publishers",
                c => new
                    {
                        PublisherId1 = c.Int(nullable: false, identity: true),
                        PublisherId2 = c.Int(nullable: false),
                        Title = c.String(),
                    })
                .PrimaryKey(t => new { t.PublisherId1, t.PublisherId2 });
            
            AddColumn("dbo.Books", "PublisherId1", c => c.Int());
            AddColumn("dbo.Books", "PublisherId2", c => c.Int());
            CreateIndex("dbo.Books", new[] { "PublisherId1", "PublisherId2" });
            AddForeignKey("dbo.Books", new[] { "PublisherId1", "PublisherId2" }, "dbo.Publishers", new[] { "PublisherId1", "PublisherId2" });
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Books", new[] { "PublisherId1", "PublisherId2" }, "dbo.Publishers");
            DropIndex("dbo.Books", new[] { "PublisherId1", "PublisherId2" });
            DropColumn("dbo.Books", "PublisherId2");
            DropColumn("dbo.Books", "PublisherId1");
            DropTable("dbo.Publishers");
        }
    }
}

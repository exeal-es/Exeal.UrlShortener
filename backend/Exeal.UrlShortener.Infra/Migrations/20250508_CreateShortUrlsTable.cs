using FluentMigrator;

namespace Exeal.UrlShortener.Infra.Migrations;

[Migration(20250508)]
public class CreateShortUrlsTable : Migration
{
    public override void Up()
    {
        Create.Table("ShortUrls")
            .WithColumn("Slug").AsString(50).PrimaryKey()
            .WithColumn("DestinationUrl").AsString(2000).NotNullable()
            .WithColumn("CreatedAt").AsDateTime().NotNullable();
    }

    public override void Down()
    {
        Delete.Table("ShortUrls");
    }
} 
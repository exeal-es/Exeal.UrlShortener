using FluentMigrator;

namespace Exeal.UrlShortener.Infra.Migrations;

[Migration(20260616)]
public class AddTitleToShortUrlsTable : Migration
{
    public override void Up()
    {
        Alter.Table("ShortUrls")
            .AddColumn("Title").AsString(500).Nullable();
    }

    public override void Down()
    {
        Delete.Column("Title").FromTable("ShortUrls");
    }
}

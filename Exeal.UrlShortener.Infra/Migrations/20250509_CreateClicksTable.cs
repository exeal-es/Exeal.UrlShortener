using System.Data;
using FluentMigrator;

namespace Exeal.UrlShortener.Infra.Migrations;

[Migration(20250509)]
public class CreateClicksTable : Migration
{
    public override void Up()
    {
        Create.Table("Clicks")
            .WithColumn("Id").AsInt32().PrimaryKey().Identity()
            .WithColumn("Slug").AsString().NotNullable()
            .ForeignKey("fk_clicks_short_urls", "ShortUrls", "Slug").OnDelete(Rule.Cascade)
            .WithColumn("VisitorFingerprint").AsString().NotNullable()
            .WithColumn("CreatedAt").AsDateTimeOffset().NotNullable();
        
        Create.Index("idx_clicks_slug")
            .OnTable("Clicks")
            .OnColumn("Slug");

        Create.Index("idx_clicks_slug_fingerprint")
            .OnTable("Clicks")
            .OnColumn("Slug").Ascending()
            .OnColumn("VisitorFingerprint").Ascending();
    }

    public override void Down()
    {
        Delete.Index("idx_clicks_slug_fingerprint").OnTable("Clicks");
        Delete.Index("idx_clicks_slug").OnTable("Clicks");
        Delete.Table("Clicks");
    }
} 
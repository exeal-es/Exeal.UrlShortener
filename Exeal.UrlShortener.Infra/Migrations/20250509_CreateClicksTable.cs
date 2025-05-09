using System.Data;
using FluentMigrator;

namespace Exeal.UrlShortener.Infra.Migrations;

[Migration(20250509)]
public class CreateClicksTable : Migration
{
    public override void Up()
    {
        Create.Table("clicks")
            .WithColumn("id").AsInt32().PrimaryKey().Identity()
            .WithColumn("slug").AsString().NotNullable()
            .ForeignKey("fk_clicks_short_urls", "short_urls", "slug").OnDelete(Rule.Cascade)
            .WithColumn("ip_address").AsString().NotNullable()
            .WithColumn("user_agent").AsString().NotNullable()
            .WithColumn("created_at").AsDateTimeOffset().NotNullable();
        
        Create.Index("idx_clicks_slug")
            .OnTable("clicks")
            .OnColumn("slug");

        Create.Index("idx_clicks_slug_ip")
            .OnTable("clicks")
            .OnColumn("slug").Ascending()
            .OnColumn("ip_address").Ascending();
    }

    public override void Down()
    {
        Delete.Index("idx_clicks_slug_ip").OnTable("clicks");
        Delete.Index("idx_clicks_slug").OnTable("clicks");
        Delete.Table("clicks");
    }
} 
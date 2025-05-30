namespace Exeal.UrlShortener.Api.Services;

using System.Globalization;
using System.Text;
using Notion.Client;

public static class Notion2Markdown
{
    public static async Task<string> ExportPageToMarkdown(this NotionClient notionClient, string pageId)
    {
        var page = await notionClient.Pages.RetrieveAsync(pageId);
        var blocks = await notionClient.Blocks.RetrieveChildrenAsync(pageId);
        var comments = await notionClient.Comments.RetrieveAsync(new RetrieveCommentsParameters()
        {
            BlockId = pageId
        });
        
        var pageProperties = RenderPageProperties(page);
        var pageContents = RenderBlocks(blocks);
        var pageComments = RenderComments(comments);
        
        var sb = new StringBuilder();
        sb.AppendLine(pageProperties);
        sb.AppendLine(pageContents);
        sb.AppendLine(pageComments);
        return sb.ToString();
    }
    
    private static string RenderBlocks(PaginatedList<IBlock> blocks)
    {
        var sb = new StringBuilder();

        foreach (var block in blocks.Results)
        {
            if (block is HeadingOneBlock h1)
            {
                sb.AppendLine(RenderH1(h1));
            }
            if (block is HeadingTwoBlock h2)
            {
                sb.AppendLine(RenderH2(h2));
            }
            if (block is HeadingThreeBlock h3)
            {
                sb.AppendLine(RenderH3(h3));
            }
            if (block is ParagraphBlock p)
            {
                p.Paragraph.RichText.Select(RenderRichText)
                    .ToList()
                    .ForEach(text => sb.Append(text));
                sb.AppendLine();
            }
            if (block is BulletedListItemBlock li)
            {
                sb.AppendLine(RenderLi(li));
            }
            sb.AppendLine();
        }
        return sb.ToString();
    }

    private static string RenderLi(BulletedListItemBlock li)
    {
        return $"- {li.BulletedListItem.RichText.Select(RenderRichText).FirstOrDefault()}";
    }

    private static string RenderH3(HeadingThreeBlock h3)
    {
        return $"### {h3.Heading_3.RichText.Select(RenderRichText).FirstOrDefault()}";
    }

    private static string RenderH2(HeadingTwoBlock h2)
    {
        return $"## {h2.Heading_2.RichText.Select(RenderRichText).FirstOrDefault()}";
    }

    private static string RenderH1(HeadingOneBlock h1)
    {
        return $"# {h1.Heading_1.RichText.Select(RenderRichText).FirstOrDefault()}";
    }

    private static string RenderRichText(RichTextBase richText)
    {
        if (richText.Href != null)
        {
            return $"[{richText.PlainText}]({richText.Href})";
        }

        return richText.PlainText;
    }

    private static string RenderPageProperties(Page page)
    {
        StringBuilder sb = new();
        
        sb.AppendLine("---");
        foreach (var property in page.Properties)
        {
            AppendProperty(sb, property);
        }
        sb.AppendLine("---");
        
        return sb.ToString();
    }

    private static void AppendProperty(StringBuilder sb, KeyValuePair<string, PropertyValue> property)
    {
        if (property.Value is NumberPropertyValue numberPropertyValue)
        {
            if (numberPropertyValue.Number.HasValue)
            {
                sb.AppendLine(FormatKeyValue(property.Key, numberPropertyValue.Number.Value.ToString(CultureInfo.InvariantCulture)));
            }
        }
        else if (property.Value is LastEditedTimePropertyValue lastEditedTimePropertyValue)
        {
            var lastEditedTime = lastEditedTimePropertyValue.LastEditedTime;
            if (!string.IsNullOrEmpty(lastEditedTime))
            {
                sb.AppendLine(FormatKeyValue(property.Key, lastEditedTime));
            }
        }
        else if (property.Value is SelectPropertyValue selectPropertyValue)
        {
            if (selectPropertyValue.Select != null)
            {
                sb.AppendLine(FormatKeyValue(property.Key, selectPropertyValue.Select.Name));
            }
        }
        else if (property.Value is UrlPropertyValue urlPropertyValue)
        {
            var url = urlPropertyValue.Url;
            if (!string.IsNullOrEmpty(url))
            {
                sb.AppendLine(FormatKeyValue(property.Key, url));
            }
        }
        else if (property.Value is DatePropertyValue datePropertyValue)
        {
            var date = datePropertyValue.Date?.ToString();
            if (!string.IsNullOrEmpty(date))
            {
                sb.AppendLine(FormatKeyValue(property.Key, date));
            }
        }
        else if (property.Value is CreatedTimePropertyValue createdTimePropertyValue)
        {
            sb.AppendLine(FormatKeyValue(property.Key, createdTimePropertyValue.CreatedTime));
        }
        else if (property.Value is TitlePropertyValue titlePropertyValue)
        {
            var title = titlePropertyValue.Title[0].PlainText;
            if (!string.IsNullOrEmpty(title))
            {
                sb.AppendLine(FormatKeyValue(property.Key, title));
            }
        }
        else if (property.Value is RelationPropertyValue relationPropertyValue)
        {
            if (relationPropertyValue.Relation.Count > 0)
            {
                sb.AppendLine(FormatKeyValue(property.Key, relationPropertyValue.Relation.Count + " items"));
            }
        }
        else if (property.Value is EmailPropertyValue emailPropertyValue)
        {
            var email = emailPropertyValue.Email;
            if (!string.IsNullOrEmpty(email))
            {             
                sb.AppendLine(FormatKeyValue(property.Key, email));
            }
        }
        else if (property.Value is MultiSelectPropertyValue multiSelectPropertyValue)
        {
            var multiSelectSeparatedByCommas = string.Join(", ", multiSelectPropertyValue.MultiSelect.Select(s => s.Name));
            if (!string.IsNullOrEmpty(multiSelectSeparatedByCommas))
            {
                sb.AppendLine(FormatKeyValue(property.Key, multiSelectSeparatedByCommas));                
            }
        }
        else if (property.Value is PhoneNumberPropertyValue phoneNumberPropertyValue)
        {
            var phoneNumber = phoneNumberPropertyValue.PhoneNumber;
            if (!string.IsNullOrEmpty(phoneNumber))
            {
                sb.AppendLine(FormatKeyValue(property.Key, phoneNumber));                
            }
        }
        else if (property.Value is CheckboxPropertyValue checkboxPropertyValue)
        {
            var formatPropertyValue = checkboxPropertyValue.Checkbox ? "true" : "false";
            sb.AppendLine(FormatKeyValue(property.Key, formatPropertyValue));
        }
        else if (property.Value is RichTextPropertyValue richTextPropertyValue)
        {
            var formatPropertyValue = richTextPropertyValue.RichText.Select(RenderRichText).FirstOrDefault();
            if (!string.IsNullOrEmpty(formatPropertyValue))
            {
                sb.AppendLine(FormatKeyValue(property.Key, formatPropertyValue));
            }
        }
        else
        {
            sb.AppendLine(FormatKeyValue(property.Key, "(unknown property type)"));
        }
    }

    private static string FormatKeyValue(string key, string value)
    {
        return $"\"{key}\": \"{value}\"";
    }

    private static string RenderComments(RetrieveCommentsResponse comments)
    {
        var sb = new StringBuilder();
        sb.AppendLine("# Comments");
        comments.Results.Select(RenderComment)
            .ToList()
            .ForEach(comment => sb.AppendLine(comment));
        return sb.ToString();
    }

    private static string RenderComment(Comment comment)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"{comment.CreatedTime}");
        foreach (var richText in comment.RichText.Select(RenderRichText))
        {
            sb.Append(richText);
        }

        sb.AppendLine();
        return sb.ToString();
    }
}

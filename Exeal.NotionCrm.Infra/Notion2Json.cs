using System.Globalization;
using System.Text;
using Exeal.UrlShortener.Api.Services;
using Notion.Client;

namespace Exeal.NotionCrm.Infra;

public static class Notion2Json
{
    public static NotionContactDto ExportContactPageToJson(Page page)
    {
        var properties = ExtractPageProperties(page);

        return new NotionContactDto(
            page.Id,
            properties.GetValueOrDefault("esFueAlumnoEn", string.Empty),
            properties.GetValueOrDefault("ciudad", string.Empty),
            properties.GetValueOrDefault("telefono", string.Empty),
            properties.GetValueOrDefault("emailPersonal", string.Empty),
            properties.GetValueOrDefault("emailEmpresa", string.Empty),
            properties.GetValueOrDefault("tags", string.Empty),
            properties.GetValueOrDefault("empresa", string.Empty),
            properties.GetValueOrDefault("lastEditedTime", string.Empty),
            properties.GetValueOrDefault("decisionMaker", string.Empty),
            properties.GetValueOrDefault("createdTime", string.Empty),
            properties.GetValueOrDefault("name", string.Empty),
            properties.GetValueOrDefault("ultimaInteraccion", string.Empty),
            properties.GetValueOrDefault("perfilDeLinkedin", string.Empty),
            properties.GetValueOrDefault("cargo", string.Empty)
        );
    }

    private static Dictionary<string, string> ExtractPageProperties(Page page)
    {
        Dictionary<string, string> properties = new();
        foreach (var property in page.Properties)
        {
            AppendProperty(properties, property);
        }

        return properties;
    }

    private static void AppendProperty(Dictionary<string, string> properties,
        KeyValuePair<string, PropertyValue> property)
    {
        if (property.Value is NumberPropertyValue numberPropertyValue)
        {
            if (numberPropertyValue.Number.HasValue)
            {
                properties[ToCamelCase(property.Key)] = numberPropertyValue.Number.Value.ToString(CultureInfo.InvariantCulture);
            }
        }
        else if (property.Value is LastEditedTimePropertyValue lastEditedTimePropertyValue)
        {
            var lastEditedTime = lastEditedTimePropertyValue.LastEditedTime;
            if (!string.IsNullOrEmpty(lastEditedTime))
            {
                properties[ToCamelCase(property.Key)] = lastEditedTime;
            }
        }
        else if (property.Value is SelectPropertyValue selectPropertyValue)
        {
            if (selectPropertyValue.Select != null)
            {
                properties[ToCamelCase(property.Key)] = selectPropertyValue.Select.Name;
            }
        }
        else if (property.Value is UrlPropertyValue urlPropertyValue)
        {
            var url = urlPropertyValue.Url;
            if (!string.IsNullOrEmpty(url))
            {
                properties[ToCamelCase(property.Key)] = url;
            }
        }
        else if (property.Value is DatePropertyValue datePropertyValue)
        {
            var date = datePropertyValue.Date?.ToString();
            if (!string.IsNullOrEmpty(date))
            {
                properties[ToCamelCase(property.Key)] = date;
            }
        }
        else if (property.Value is CreatedTimePropertyValue createdTimePropertyValue)
        {
            properties[ToCamelCase(property.Key)] = createdTimePropertyValue.CreatedTime;
        }
        else if (property.Value is TitlePropertyValue titlePropertyValue)
        {
            if (titlePropertyValue.Title != null && titlePropertyValue.Title.Count > 0)
            {
                var title = titlePropertyValue.Title[0].PlainText;
                if (!string.IsNullOrEmpty(title))
                {
                    properties[ToCamelCase(property.Key)] = title;
                }
            }
        }
        else if (property.Value is RelationPropertyValue relationPropertyValue)
        {
            if (relationPropertyValue.Relation.Count > 0)
            {
                properties[ToCamelCase(property.Key)] = relationPropertyValue.Relation.Count + " items";
            }
        }
        else if (property.Value is EmailPropertyValue emailPropertyValue)
        {
            var email = emailPropertyValue.Email;
            if (!string.IsNullOrEmpty(email))
            {             
                properties[ToCamelCase(property.Key)] = email;
            }
        }
        else if (property.Value is MultiSelectPropertyValue multiSelectPropertyValue)
        {
            var multiSelectSeparatedByCommas = string.Join(", ", multiSelectPropertyValue.MultiSelect.Select(s => s.Name));
            if (!string.IsNullOrEmpty(multiSelectSeparatedByCommas))
            {
                properties[ToCamelCase(property.Key)] = multiSelectSeparatedByCommas;
            }
        }
        else if (property.Value is PhoneNumberPropertyValue phoneNumberPropertyValue)
        {
            var phoneNumber = phoneNumberPropertyValue.PhoneNumber;
            if (!string.IsNullOrEmpty(phoneNumber))
            {
                properties[ToCamelCase(property.Key)] = phoneNumber;
            }
        }
        else if (property.Value is CheckboxPropertyValue checkboxPropertyValue)
        {
            var formatPropertyValue = checkboxPropertyValue.Checkbox ? "true" : "false";
            properties[ToCamelCase(property.Key)] = formatPropertyValue;
        }
        else if (property.Value is RichTextPropertyValue richTextPropertyValue)
        {
            var formatPropertyValue = richTextPropertyValue.RichText.Select(RenderRichText).FirstOrDefault();
            if (!string.IsNullOrEmpty(formatPropertyValue))
            {
                properties[ToCamelCase(property.Key)] = formatPropertyValue;
            }
        }
        else
        {
            properties[ToCamelCase(property.Key)] = "(unknown property type)";
        }
    }
    
    private static string RenderRichText(RichTextBase richText)
    {
        if (richText.Href != null)
        {
            return $"[{richText.PlainText}]({richText.Href})";
        }

        return richText.PlainText;
    }

    private static string ToCamelCase(string text)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        // Convert to camelCase
        var parts = text.Split(new[] { ' ', '_' }, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0)
            return text;

        var sb = new StringBuilder(parts[0].ToLowerInvariant());
        for (int i = 1; i < parts.Length; i++)
        {
            sb.Append(char.ToUpperInvariant(parts[i][0]));
            sb.Append(parts[i].Substring(1).ToLowerInvariant());
        }
        
        // Remove acentos
        sb.Replace("á", "a").Replace("é", "e").Replace("í", "i")
          .Replace("ó", "o").Replace("ú", "u").Replace("ñ", "n")
          .Replace("ü", "u").Replace("ç", "c");
        
        return sb.ToString();
    }
}

using System.Globalization;
using System.Text;
using Exeal.NotionCrm.Infra.Ports;
using Notion.Client;

namespace Exeal.NotionCrm.Infra;

public static class Notion2Json
{
    public static NotionContactDto ExportContactPageToJson(Page page)
    {
        var properties = ExtractPageProperties(page);

        return new NotionContactDto(
            page.Id,
            (string?)properties.GetValueOrDefault("esFueAlumnoEn", null),
            (string?)properties.GetValueOrDefault("ciudad", null),
            (string?)properties.GetValueOrDefault("telefono", null),
            (string?)properties.GetValueOrDefault("emailPersonal", null),
            (string?)properties.GetValueOrDefault("emailEmpresa", null),
            (List<string>)properties.GetValueOrDefault("tags", new List<string>()),
            (string?)properties.GetValueOrDefault("empresa", null),
            DateTime.Parse((string)properties.GetValueOrDefault("lastEditedTime", null)),
            (bool)properties.GetValueOrDefault("decisionMaker", false),
            DateTime.Parse((string)properties.GetValueOrDefault("createdTime", null)),
            (string?)properties.GetValueOrDefault("name", null),
            (DateTime?)properties.GetValueOrDefault("ultimaInteraccion", null),
            (string?)properties.GetValueOrDefault("perfilDeLinkedin", null),
            (string?)properties.GetValueOrDefault("cargo", null)
        );
    }

    private static Dictionary<string, object> ExtractPageProperties(Page page)
    {
        Dictionary<string, object> properties = new();
        foreach (var property in page.Properties)
        {
            AppendProperty(properties, property);
        }

        return properties;
    }

    private static void AppendProperty(Dictionary<string, object> properties,
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
            if (datePropertyValue.Date?.Start != null)
            {
                properties[ToCamelCase(property.Key)] = datePropertyValue.Date.Start;
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
                properties[ToCamelCase(property.Key)] = relationPropertyValue.Relation[0].Id;
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
            var multiSelect = multiSelectPropertyValue.MultiSelect.Select(s => s.Name).ToList();
            properties[ToCamelCase(property.Key)] = multiSelect;
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
            properties[ToCamelCase(property.Key)] = checkboxPropertyValue.Checkbox;
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

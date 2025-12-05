using Umbraco.Cms.Core.Models;

namespace Lexicon.Package;

/// <summary>
/// Builds localization extensions from dictionary items.
/// Converts flat dictionary keys (Area_Key or Area.Key) to nested localization format.
/// </summary>
public static class LocalizationBuilder
{
    /// <summary>
    /// Builds localization extensions from dictionary items.
    /// Keys must contain '_' or '.' to be included. The first separator splits area from key.
    /// </summary>
    /// <example>
    /// Dictionary key "DocumentType_ButtonColor" becomes:
    /// { "DocumentType": { "ButtonColor": "translated value" } }
    /// Reference in UI: {#DocumentType_ButtonColor}
    /// </example>
    public static List<Dictionary<string, object>> BuildLocalizationExtensions(
        IEnumerable<IDictionaryItem> items)
    {
        // Filter: only keys with '_' or '.' (area_key format required for Umbraco localization)
        var filtered = items.Where(i => i.ItemKey.Contains('_') || i.ItemKey.Contains('.'));

        // Group by culture, then build nested structure
        var byCulture = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();

        foreach (var item in filtered)
        {
            var (area, key) = SplitKey(item.ItemKey);
            if (area == null || key == null) continue;

            foreach (var translation in item.Translations)
            {
                if (!byCulture.ContainsKey(translation.LanguageIsoCode))
                    byCulture[translation.LanguageIsoCode] = new();

                if (!byCulture[translation.LanguageIsoCode].ContainsKey(area))
                    byCulture[translation.LanguageIsoCode][area] = new();

                byCulture[translation.LanguageIsoCode][area][key] = translation.Value ?? "";
            }
        }

        // Create localization extension per culture with nested structure
        return byCulture.Select(kvp => new Dictionary<string, object>
        {
            ["type"] = "localization",
            ["alias"] = $"Lexicon.Localization.{kvp.Key}",
            ["name"] = $"Lexicon Dictionary ({kvp.Key})",
            ["meta"] = new Dictionary<string, object>
            {
                ["culture"] = kvp.Key,
                ["localizations"] = kvp.Value
            }
        }).ToList();
    }

    /// <summary>
    /// Splits a dictionary key into area and key parts.
    /// Uses the first '_' or '.' as the separator.
    /// </summary>
    public static (string? Area, string? Key) SplitKey(string itemKey)
    {
        var underscoreIndex = itemKey.IndexOf('_');
        var dotIndex = itemKey.IndexOf('.');

        int separatorIndex;
        if (underscoreIndex < 0 && dotIndex < 0) return (null, null);
        else if (underscoreIndex < 0) separatorIndex = dotIndex;
        else if (dotIndex < 0) separatorIndex = underscoreIndex;
        else separatorIndex = Math.Min(underscoreIndex, dotIndex);

        if (separatorIndex <= 0) return (null, null);

        var area = itemKey.Substring(0, separatorIndex);
        var key = itemKey.Substring(separatorIndex + 1);

        if (string.IsNullOrEmpty(key)) return (null, null);

        return (area, key);
    }
}

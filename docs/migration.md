# Migration from Umbraco 13

This guide covers the key differences between dictionary-based localization in Umbraco 13 and Lexicon in Umbraco 14+.

## Syntax Changes

Lexicon uses Umbraco's built-in [UI Localization](https://docs.umbraco.com/umbraco-cms/extending/language-files/ui-localization) system. The syntax differs depending on where you use it:

### Reference Syntax

| Field Type | Umbraco 13 | Lexicon (Umbraco 14+) |
|------------|------------|----------------------|
| Labels | `#MyKey` | `#Area_Key` |
| Descriptions | `#MyKey` | `{#Area_Key}` |

**Labels** (property labels, tab names, group names) use the simple `#` prefix:
```
#Article_Title
```

**Descriptions** and other fields that support [Umbraco Flavoured Markdown](https://docs.umbraco.com/umbraco-cms/reference/umbraco-flavored-markdown) (UFM) use curly braces:
```
{#Article_Description}
```

### Key Format

| Umbraco 13 | Lexicon |
|------------|---------|
| `MyKey` | `Area_Key` |
| `ButtonLabel` | `Form_ButtonLabel` |

In Umbraco 13, dictionary keys could be any string. Lexicon requires a two-level structure:

```
Area_Key
```

Where:
- **Area** - A grouping/namespace for related keys
- **Key** - The specific translation key

## Migration Steps

### 1. Update Dictionary Keys

Add an area prefix to your existing dictionary keys:

**Before (Umbraco 13):**
- `Title`
- `Description`
- `SubmitButton`

**After (Lexicon):**
- `Article_Title`
- `Article_Description`
- `Form_SubmitButton`

Choose area names that group related translations logically.

### 2. Create Content Languages

**Important**: You must create a content language for each UI language you want to support.

For example, if your editors use Danish (`da`) as their backoffice UI language, you must:
1. Go to **Settings** → **Languages**
2. Create Danish as a content language (if it doesn't exist)
3. Add translations to your dictionary items for Danish

Lexicon only outputs translations for languages that exist as content languages in Umbraco.

### 3. Update References

Update all references in your document types:

**Labels (property labels, tab names, group names):**

Before: `#Title`
After: `#Article_Title`

**Descriptions (UFM-enabled fields):**

Before: `#Description`
After: `{#Article_Description}`

### 4. Test Each Document Type

After migration:
1. Open each document type in the Settings section
2. Verify labels and descriptions display correctly
3. Create/edit content to confirm the translated values appear

## Feature Comparison

| Feature | Umbraco 13 | Lexicon |
|---------|------------|---------|
| Simple key syntax (`#Key`) | Yes | No |
| Area-based keys (`#Area_Key`) | No | Yes |
| Property labels | Yes | Yes |
| Property descriptions | Yes | Yes (UFM syntax) |
| Tab names | Yes | Yes |
| Inline translation in descriptions | Yes | Yes* |
| Real-time dictionary updates | Yes | No** |

\* Inline translation like "Click {#Form_Here} to submit" works in description fields that support UFM.

\** Lexicon loads dictionary values at application startup. Changes require a restart.

## What's Not Supported

The following Umbraco 13 features are not available in Lexicon:

### Simple Key References

```
#SimpleKey → Not supported
```

All keys must have an area prefix. If you have flat keys, add a generic area:

```
#General_SimpleKey (for labels)
{#General_SimpleKey} (for descriptions)
```

### Language File Fallback

In Umbraco 13, if a dictionary key wasn't found, it would fall back to language files (`.xml` files). Lexicon only reads from the dictionary database.

### Dynamic Updates

In Umbraco 13, changing a dictionary value immediately reflected in the backoffice. With Lexicon, changes require an application restart because localizations are registered at startup.

## Troubleshooting

### Translation Not Appearing

1. **Check the key format**: Must contain `_` or `.`
2. **Check the syntax**: Labels use `#Area_Key`, descriptions use `{#Area_Key}`
3. **Verify content language exists**: The user's UI language must exist as a content language
4. **Restart the application**: Changes require a restart

### Key Shows Instead of Value

If you see `#Area_Key` or `{#Area_Key}` displayed instead of the translated value:

1. Verify the dictionary item exists with exactly that key
2. Check that the key follows the `Area_Key` format
3. Ensure the dictionary item has a value for the user's UI language
4. Verify the content language exists (Settings → Languages)

### Missing for Some Languages

Lexicon creates localization entries per culture. If a translation is missing:

1. Ensure the content language exists in Umbraco (Settings → Languages)
2. Add the translation to the dictionary item for that language
3. Restart the application

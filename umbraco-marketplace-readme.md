# Lexicon for Umbraco

Use dictionary items to localize the Umbraco Backoffice in Umbraco 14+.

## What is it?

Dictionary-based backoffice localization was a built-in feature in Umbraco 13 and earlier, allowing you to use `#DictionaryKey` in document type labels and descriptions to display translated values. This functionality was removed in Umbraco 14.

**Lexicon** reintroduces this capability by converting your dictionary items into Umbraco's new localization extension format, making them available to the backoffice.

## Quick Start

1. Create dictionary items in the Umbraco backoffice with keys in the format `Area_Key` or `Area.Key`
2. Reference them in document types:
   - **Labels**: `#Area_Key`
   - **Descriptions**: `{#Area_Key}`

For example, if you have a dictionary item `Article_Title` with the value "Article Title", you can use `#Article_Title` in a document type property label.

## Important: Content Languages

You must create a content language for each UI language you want to support. For example, if your editors use Danish as their backoffice language, you must add Danish as a content language in **Settings > Languages**.

## Migrating from Umbraco 13

If you're upgrading from Umbraco 13, there are a few changes to be aware of:

### Key Format Changes

Dictionary keys now require an area prefix with `_` or `.` as separator:

| Umbraco 13 | Lexicon |
|------------|---------|
| `Title` | `Article_Title` |
| `SubmitButton` | `Form_SubmitButton` |

### Syntax Changes

| Field Type | Umbraco 13 | Lexicon |
|------------|------------|---------|
| Labels | `#MyKey` | `#Area_Key` |
| Descriptions | `#MyKey` | `{#Area_Key}` |

### Dynamic Updates

In Umbraco 13, dictionary changes were reflected immediately. With Lexicon, changes require a browser refresh because localizations are loaded at startup.

For detailed migration steps, see the [migration guide](https://github.com/iOvergaard/umbraco-lexicon/blob/main/docs/migration.md).

## Documentation

For detailed documentation, visit the [GitHub repository](https://github.com/iOvergaard/umbraco-lexicon).

## Contributing

Contributions are welcome! Please visit the [GitHub repository](https://github.com/iOvergaard/umbraco-lexicon) to report issues or submit pull requests.

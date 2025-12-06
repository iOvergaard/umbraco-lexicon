# Lexicon for Umbraco

[![Downloads](https://img.shields.io/nuget/dt/Umbraco.Community.Lexicon?color=cc9900)](https://www.nuget.org/packages/Umbraco.Community.Lexicon/)
[![NuGet](https://img.shields.io/nuget/vpre/Umbraco.Community.Lexicon?color=0273B3)](https://www.nuget.org/packages/Umbraco.Community.Lexicon)
[![GitHub license](https://img.shields.io/github/license/iOvergaard/umbraco-lexicon?color=8AB803)](../LICENSE)

Use dictionary items to localize the Umbraco Backoffice in Umbraco 14+.

## What is it?

Dictionary-based backoffice localization was a built-in feature in Umbraco 13 and earlier, allowing you to use `#DictionaryKey` in document type labels and descriptions to display translated values. This functionality was removed in Umbraco 14.

**Lexicon** reintroduces this capability by converting your dictionary items into Umbraco's new localization extension format, making them available to the backoffice.

## Installation

Add the package to an existing Umbraco website (v14+) from NuGet:

```bash
dotnet add package Umbraco.Community.Lexicon
```

## Quick Start

1. Create dictionary items in the Umbraco backoffice with keys in the format `Area_Key` or `Area.Key`
2. Reference them in document types:
   - **Labels**: `#Area_Key`
   - **Descriptions**: `{#Area_Key}`

For example, if you have a dictionary item `Article_Title` with the value "Article Title", you can use `#Article_Title` in a document type property label.

## Important: Content Languages

You must create a content language for each UI language you want to support. For example, if your editors use Danish as their backoffice language, you must add Danish as a content language in **Settings → Languages**.

## Documentation

See the [documentation](../docs/README.md) for:

- [Usage Guide](../docs/usage.md) - How to use Lexicon
- [Migration from Umbraco 13](../docs/migration.md) - Differences and migration steps
- [Technical Details](../docs/technical.md) - How it works under the hood

## Contributing

Contributions to this package are most welcome! Please read the [Contributing Guidelines](CONTRIBUTING.md).

## License

This project is licensed under the MPL-2.0 License - see the [LICENSE](../LICENSE) file for details.

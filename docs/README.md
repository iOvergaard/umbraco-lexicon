# Lexicon Documentation

<p align="center">
  <img src="https://raw.githubusercontent.com/iOvergaard/umbraco-lexicon/main/docs/images/logo.png" alt="Lexicon Logo" width="128" height="128">
</p>

Lexicon enables dictionary-based Umbraco BackOffice localization in Umbraco 14+.

## Requirements

- Umbraco 17 or higher
- .NET 10 or higher

## Installation

The package is available from NuGet and can be installed using Visual Studio or the .NET CLI.

### Visual Studio

1. Open your solution in Visual Studio
2. Right-click on your project and select `Manage NuGet Packages...`
3. Search for `Umbraco.Community.Lexicon` and click `Install`

### .NET CLI

```bash
dotnet add package Umbraco.Community.Lexicon
```

## How It Works

Lexicon reads all dictionary items from your Umbraco installation and converts them into Umbraco's localization extension format. This happens automatically at runtime - no configuration required.

Dictionary items with keys containing `_` or `.` are converted into a nested localization structure:

| Dictionary Key | Becomes |
|----------------|---------|
| `Article_Title` | `{ "Article": { "Title": "..." } }` |
| `Form.Submit` | `{ "Form": { "Submit": "..." } }` |
| `DocumentType_Button.Label` | `{ "DocumentType": { "Button.Label": "..." } }` |

## Documentation

- [Usage Guide](https://github.com/iOvergaard/umbraco-lexicon/blob/main/docs/usage.md) - How to use dictionary-based localization
- [Migration from Umbraco 13](https://github.com/iOvergaard/umbraco-lexicon/blob/main/docs/migration.md) - Key differences and migration steps
- [Technical Details](https://github.com/iOvergaard/umbraco-lexicon/blob/main/docs/technical.md) - Implementation details and approaches tried

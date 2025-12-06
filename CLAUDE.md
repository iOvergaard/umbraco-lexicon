# Lexicon - Umbraco Package

## Project Overview

**Lexicon** is an Umbraco community package that restores dictionary-based backoffice localization for Umbraco 14+. It converts dictionary items into Umbraco's localization extension format at startup.

## How It Works

Lexicon uses `IPackageManifestReader` to register dictionary items as localization extensions. Dictionary items with keys in `Area_Key` format are transformed into Umbraco's nested localization structure.

### Dictionary Key Format

Keys must contain `_` or `.` as separator. The first separator splits area from key:

- `Article_Title` → `{ "Article": { "Title": "..." } }`
- `Form.Submit` → `{ "Form": { "Submit": "..." } }`
- `Article_Button_Label` → `{ "Article": { "Button_Label": "..." } }`

### Usage Syntax

| Field Type | Syntax |
|------------|--------|
| Labels (property labels, tab names, group names) | `#Area_Key` |
| Descriptions (UFM-enabled fields) | `{#Area_Key}` |

### Important Requirements

- Dictionary keys must contain `_` or `.`
- Content languages must exist for each UI language to support
- Changes require application restart (localizations loaded at startup)

## Project Structure

```
src/
├── Lexicon/                    # Main package
│   ├── Package/
│   │   ├── UmbracoPackage.cs              # IComposer - registers services
│   │   ├── LexiconPackageManifestReader.cs # IPackageManifestReader
│   │   └── LocalizationBuilder.cs          # Transforms dictionary items
│   └── Lexicon.csproj
├── Lexicon.UnitTest/           # Unit tests
│   └── LocalizationBuilderTests.cs
└── Lexicon.sln
```

## Build Commands

```bash
# Build
dotnet build src/Lexicon.sln

# Test
dotnet test src/Lexicon.sln

# Pack
dotnet pack src/Lexicon/Lexicon.csproj -c Release /p:Version=1.0.0
```

## Package Details

- **NuGet ID**: `Umbraco.Community.Lexicon`
- **Target**: Umbraco 17+ / .NET 10
- **License**: MPL-2.0

## Documentation

- [Usage Guide](docs/usage.md)
- [Migration from Umbraco 13](docs/migration.md)
- [Technical Details](docs/technical.md)

## Approaches Tried (and why they failed)

1. **IAsyncResultFilter** - Translated API responses, but caused data overwrite on save
2. **Property name filtering** - Partial solution, still overwrote editable fields
3. **UFM Component** - Only works in description fields, inconsistent coverage
4. **Harmony patching** - Maintenance burden, upgrade risk, not officially supported
5. **MutationObserver** - Shadow DOM issues, performance problems
6. **Localization Manifest** - Current implementation, uses official extension point

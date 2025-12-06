# Lexicon - Source Directory

## Quick Reference

### Build & Test

```bash
dotnet build Lexicon.sln
dotnet test Lexicon.sln
```

### Project Structure

- `Lexicon/` - Main package (backend-only, no client code)
- `Lexicon.UnitTest/` - xUnit tests with Moq

### Key Files

| File | Purpose |
|------|---------|
| `Lexicon/Package/UmbracoPackage.cs` | IComposer - registers LexiconPackageManifestReader |
| `Lexicon/Package/LexiconPackageManifestReader.cs` | IPackageManifestReader - builds package manifest with localizations |
| `Lexicon/Package/LocalizationBuilder.cs` | Static class - transforms dictionary items to nested localization format |

### How Dictionary Keys Are Processed

```
Input:  Article_Title (with translation "Article Title" for en-US)
Output: { "type": "localization", "meta": { "culture": "en-US", "localizations": { "Article": { "Title": "Article Title" } } } }
```

The `SplitKey` method uses the first `_` or `.` as separator:
- `Article_Title` → Area: "Article", Key: "Title"
- `Form.Submit` → Area: "Form", Key: "Submit"

### Usage in Umbraco

- **Labels**: `#Area_Key` (property labels, tab names, group names)
- **Descriptions**: `{#Area_Key}` (UFM syntax)

### Testing

Tests cover:
- `SplitKey` with valid/invalid keys
- `BuildLocalizationExtensions` output structure
- Multi-language support
- Edge cases (empty translations, special characters)

Run tests: `dotnet test Lexicon.UnitTest/Lexicon.UnitTest.csproj`

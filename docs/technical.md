# Technical Details

This document explains how Lexicon works and documents the various implementation approaches that were explored during development.

## How Lexicon Works

Lexicon uses Umbraco's `IPackageManifestReader` interface to register localization extensions at startup:

1. **Startup**: Umbraco calls `ReadPackageManifestsAsync()` on all registered `IPackageManifestReader` implementations
2. **Dictionary Fetch**: Lexicon fetches all dictionary items using `IDictionaryItemService.GetDescendantsAsync(null)`
3. **Transform**: Dictionary items are converted to Umbraco's nested localization format
4. **Register**: Localizations are returned as package manifest extensions

### Localization Format

Umbraco 14+ uses a nested JSON structure for localizations:

```json
{
  "type": "localization",
  "alias": "Lexicon.Localization.en-US",
  "name": "Lexicon Dictionary (en-US)",
  "meta": {
    "culture": "en-US",
    "localizations": {
      "Article": {
        "Title": "Article Title",
        "Description": "Article description text"
      },
      "Form": {
        "Submit": "Submit",
        "Cancel": "Cancel"
      }
    }
  }
}
```

The backoffice then resolves `{#Article_Title}` by looking up `localizations.Article.Title`.

### Key Transformation

The `LocalizationBuilder.SplitKey()` method transforms flat dictionary keys:

```
Article_Title     → Area: "Article", Key: "Title"
Form.Submit       → Area: "Form",    Key: "Submit"
Article_Sub_Key   → Area: "Article", Key: "Sub_Key"
```

The first `_` or `.` separates area from key. Subsequent separators are preserved.

## Approaches Explored

During development, several approaches were explored before settling on the localization manifest approach.

### Approach 1: IAsyncResultFilter (Abandoned)

**Concept**: Use ASP.NET Core's `IAsyncResultFilter` to intercept Management API responses and translate `#Key` references before JSON serialization.

```csharp
public class DictionaryTranslationResultFilter : IAsyncResultFilter
{
    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        if (context.Result is ObjectResult { Value: not null } objectResult)
        {
            TranslateObject(objectResult.Value);
        }
        await next();
    }
}
```

**Why it was abandoned**: The same API endpoints serve both read and write operations. When translating the `name` field of a document type, the translated value would be saved back to the database when the user saved, overwriting the original `#Key` reference.

This affected:
- Document type names
- Property labels
- Property descriptions
- Any editable field

**Key insight**: There's no way to distinguish between "display mode" and "edit mode" at the API level because the same endpoint (`GET /document-type/{id}`) serves both the document type editor and the content editor.

### Approach 2: Property Name Filtering (Partial Solution)

**Concept**: Limit translation to specific property names (label, description, etc.) to reduce the risk of overwriting.

```csharp
private static readonly HashSet<string> TranslatableProperties = new(StringComparer.OrdinalIgnoreCase)
{
    "label", "description", "title", "placeholder", "hint", "text"
};
```

**Why it was abandoned**: Even with filtering, editable fields like `description` would still be overwritten on save. The fundamental problem remained.

### Approach 3: UFM Component with Custom API (Abandoned)

**Concept**: Create a custom Umbraco Flavoured Markdown (UFM) component that fetches translations from a custom API controller. The UFM syntax would allow embedding dictionary references in rich text and labels.

```
[lexicon key="MyKey"]
```

**Why it was abandoned**: UFM components only work in specific parts of the backoffice where UFM rendering is enabled. This approach would not provide consistent translation across all backoffice areas:

- Document type property labels don't render UFM
- Tab names and group names don't support UFM
- Only description fields and certain rich text areas would work

This would lead to an inconsistent experience where translations work in some places but not others.

### Approach 4: Harmony Runtime Patching (Not Implemented)

**Concept**: Use the [Harmony](https://github.com/pardeike/Harmony) library to patch Umbraco's internal methods at runtime, intercepting string values and translating `#Key` references.

```csharp
[HarmonyPatch(typeof(SomeUmbracoClass), "SomeMethod")]
public static class TranslationPatch
{
    public static void Postfix(ref string __result)
    {
        if (__result?.StartsWith("#") == true)
            __result = TranslateDictionaryKey(__result);
    }
}
```

**Why it was not implemented**:

1. **Maintenance burden** - Harmony patches are tightly coupled to internal implementation details. Each Umbraco update could break the patches, requiring constant maintenance.

2. **Debugging difficulty** - Runtime patching makes debugging significantly harder as the actual code execution differs from the source code.

3. **Upgrade risk** - Internal methods can change between minor versions without notice, potentially causing silent failures or unexpected behavior.

4. **Not officially supported** - Umbraco doesn't guarantee internal API stability, making this approach fragile for a community package.

### Approach 5: MutationObserver (Not Implemented)

**Concept**: Use JavaScript's MutationObserver to intercept DOM changes and translate `#Key` references client-side.

**Why it was not implemented**: The Umbraco backoffice extensively uses Shadow DOM, which MutationObservers cannot easily observe. Tools like password managers (e.g., Dashlane) that use this approach cause significant performance issues in the backoffice.

### Approach 6: Localization Manifest (Current Implementation)

**Concept**: Use Umbraco's built-in localization system by registering dictionary items as localization extensions.

**Advantages**:
1. **Official extension point** - Uses Umbraco's intended localization mechanism
2. **No data modification** - Never touches the stored values
3. **No runtime patching** - No Harmony or other reflection-based approaches
4. **Multi-language support** - Automatically handles all configured languages

**Trade-offs**:
1. **Different syntax** - Requires `{#Area_Key}` instead of `#Key`
2. **Two-level structure** - Keys must have area and key parts
3. **Restart required** - Changes need application restart

## Architecture

```
┌─────────────────────────────────────────────────────────┐
│                    Umbraco Startup                       │
└─────────────────────────────────────────────────────────┘
                           │
                           ▼
┌─────────────────────────────────────────────────────────┐
│              IPackageManifestReader                      │
│         (LexiconPackageManifestReader)                   │
└─────────────────────────────────────────────────────────┘
                           │
                           ▼
┌─────────────────────────────────────────────────────────┐
│              IDictionaryItemService                      │
│           GetDescendantsAsync(null)                      │
└─────────────────────────────────────────────────────────┘
                           │
                           ▼
┌─────────────────────────────────────────────────────────┐
│               LocalizationBuilder                        │
│    BuildLocalizationExtensions(items)                    │
│                                                         │
│    Article_Title (en-US: "Title", da-DK: "Titel")       │
│                        ▼                                 │
│    {                                                    │
│      "en-US": { "Article": { "Title": "Title" } },      │
│      "da-DK": { "Article": { "Title": "Titel" } }       │
│    }                                                    │
└─────────────────────────────────────────────────────────┘
                           │
                           ▼
┌─────────────────────────────────────────────────────────┐
│                 PackageManifest                          │
│    Extensions: [localization extensions per culture]     │
└─────────────────────────────────────────────────────────┘
                           │
                           ▼
┌─────────────────────────────────────────────────────────┐
│               Umbraco Backoffice                         │
│         {#Article_Title} → "Title" / "Titel"            │
└─────────────────────────────────────────────────────────┘
```

## Source Files

| File | Purpose |
|------|---------|
| `Package/UmbracoPackage.cs` | Composer - registers the manifest reader |
| `Package/LexiconPackageManifestReader.cs` | Builds and returns the package manifest |
| `Package/LocalizationBuilder.cs` | Transforms dictionary items to localization format |

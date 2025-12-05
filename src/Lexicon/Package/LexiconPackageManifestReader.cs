using Umbraco.Cms.Core.Manifest;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Manifest;

namespace Lexicon.Package;

internal class LexiconPackageManifestReader(
    IDictionaryItemService dictionaryItemService) : IPackageManifestReader
{
    public async Task<IEnumerable<PackageManifest>> ReadPackageManifestsAsync()
    {
        var version = GetVersion();
        var extensions = await BuildLocalizationExtensionsAsync();

        PackageManifest manifest = new()
        {
            Id = "Umbraco.Community.Lexicon",
            Name = "Lexicon",
            AllowTelemetry = true,
            Version = version,
            Extensions = extensions.ToArray()
        };

        return [manifest];
    }

    internal async Task<List<Dictionary<string, object>>> BuildLocalizationExtensionsAsync()
    {
        var items = await dictionaryItemService.GetDescendantsAsync(null);
        return LocalizationBuilder.BuildLocalizationExtensions(items);
    }

    private static string GetVersion()
    {
        return typeof(UmbracoPackage).Assembly.GetName().Version?.ToString() ?? "0.0.0";
    }
}

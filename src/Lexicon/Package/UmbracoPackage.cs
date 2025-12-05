using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Manifest;
using Umbraco.Cms.Infrastructure.Manifest;

namespace Lexicon.Package;

internal sealed class UmbracoPackage : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddSingleton<IPackageManifestReader, ServerVariablesPackage>();
    }

    private class ServerVariablesPackage : IPackageManifestReader
    {
        public Task<IEnumerable<PackageManifest>> ReadPackageManifestsAsync()
        {
            // get info from assembly
            Assembly assembly = typeof(UmbracoPackage).Assembly;
            var version = assembly.GetName().Version?.ToString() ?? "0.0.0";

            PackageManifest manifest = new()
            {
                Id = "Umbraco.Community.Lexicon",
                Name = "Lexicon",
                AllowTelemetry = true,
                Version = version,
                Extensions = [
                    BundleManifest()
                ]
            };

            IEnumerable<PackageManifest> manifests = new List<PackageManifest> { manifest };
            return Task.FromResult(manifests);
        }

        private static Dictionary<string, dynamic> BundleManifest() => new()
        {
            ["type"] = "bundle",
            ["name"] = "Lexicon Bundle",
            ["alias"] = "Lexicon.Bundle",
            ["js"] = "/App_Plugins/Lexicon/lexicon.js?v=" + Assembly.GetExecutingAssembly().GetName().Version
        };
    }
}

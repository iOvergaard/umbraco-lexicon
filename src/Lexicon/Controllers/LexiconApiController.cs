using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;

namespace Lexicon.Controllers
{
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "Lexicon")]
    public class LexiconApiController(IDictionaryItemService dictionaryItemService, ILogger<LexiconApiController> logger)
        : LexiconApiControllerBase
    {

        [HttpGet("dictionary-entries")]
        [ProducesResponseType<IEnumerable<DictionaryEntryModel>>(StatusCodes.Status200OK)]
        public async Task<IEnumerable<DictionaryEntryModel>> GetDictionaryEntries()
        {
            // Get all dictionary items
            IEnumerable<IDictionaryItem> items = await dictionaryItemService.GetDescendantsAsync(null);

            logger.LogInformation("Sending dictionary entries");

            return items.Select(item => new DictionaryEntryModel
            {
                Key = item.ItemKey,
                Translations = item.Translations.ToDictionary(
                    t => t.LanguageIsoCode,
                    t => t.Value ?? string.Empty
                )
            });
        }
    }

    public class DictionaryEntryModel
    {
        public required string Key { get; set; }
        public required Dictionary<string, string> Translations { get; set; }
    }
}

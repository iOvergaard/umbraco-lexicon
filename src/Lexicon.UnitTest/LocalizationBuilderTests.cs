using Lexicon.Package;
using Moq;
using Umbraco.Cms.Core.Models;

namespace Lexicon.UnitTest;

public class LocalizationBuilderTests
{
    [Theory]
    [InlineData("Area_Key", "Area", "Key")]
    [InlineData("Area.Key", "Area", "Key")]
    [InlineData("DocumentType_Button.Color", "DocumentType", "Button.Color")]
    [InlineData("DocumentType.Button_Color", "DocumentType", "Button_Color")]
    [InlineData("Area_Sub_Key", "Area", "Sub_Key")]
    [InlineData("A_B", "A", "B")]
    public void SplitKey_ValidKeys_ReturnsAreaAndKey(string input, string expectedArea, string expectedKey)
    {
        var (area, key) = LocalizationBuilder.SplitKey(input);

        Assert.Equal(expectedArea, area);
        Assert.Equal(expectedKey, key);
    }

    [Theory]
    [InlineData("NoSeparator")]
    [InlineData("_StartsWithSeparator")]
    [InlineData(".StartsWithDot")]
    [InlineData("EndsWithSeparator_")]
    [InlineData("EndsWithDot.")]
    [InlineData("")]
    public void SplitKey_InvalidKeys_ReturnsNull(string input)
    {
        var (area, key) = LocalizationBuilder.SplitKey(input);

        Assert.Null(area);
        Assert.Null(key);
    }

    [Fact]
    public void BuildLocalizationExtensions_WithValidItems_ReturnsNestedStructure()
    {
        // Arrange
        var items = new List<IDictionaryItem>
        {
            CreateDictionaryItem("Test_Label", new[] { ("en-US", "Hello"), ("da-DK", "Hej") }),
            CreateDictionaryItem("Test_Description", new[] { ("en-US", "World"), ("da-DK", "Verden") })
        };

        // Act
        var result = LocalizationBuilder.BuildLocalizationExtensions(items);

        // Assert
        Assert.Equal(2, result.Count); // One per culture

        var enExtension = result.First(r => ((Dictionary<string, object>)r["meta"])["culture"].ToString() == "en-US");
        var localizations = (Dictionary<string, Dictionary<string, string>>)((Dictionary<string, object>)enExtension["meta"])["localizations"];

        Assert.True(localizations.ContainsKey("Test"));
        Assert.Equal("Hello", localizations["Test"]["Label"]);
        Assert.Equal("World", localizations["Test"]["Description"]);
    }

    [Fact]
    public void BuildLocalizationExtensions_WithDotSeparator_ReturnsNestedStructure()
    {
        // Arrange
        var items = new List<IDictionaryItem>
        {
            CreateDictionaryItem("DocumentType.ButtonColor", new[] { ("en-US", "Primary") })
        };

        // Act
        var result = LocalizationBuilder.BuildLocalizationExtensions(items);

        // Assert
        Assert.Single(result);

        var extension = result[0];
        var localizations = (Dictionary<string, Dictionary<string, string>>)((Dictionary<string, object>)extension["meta"])["localizations"];

        Assert.True(localizations.ContainsKey("DocumentType"));
        Assert.Equal("Primary", localizations["DocumentType"]["ButtonColor"]);
    }

    [Fact]
    public void BuildLocalizationExtensions_FiltersOutInvalidKeys()
    {
        // Arrange
        var items = new List<IDictionaryItem>
        {
            CreateDictionaryItem("ValidArea_Key", new[] { ("en-US", "Value") }),
            CreateDictionaryItem("NoSeparator", new[] { ("en-US", "Ignored") }),
            CreateDictionaryItem("_InvalidStart", new[] { ("en-US", "Ignored") })
        };

        // Act
        var result = LocalizationBuilder.BuildLocalizationExtensions(items);

        // Assert
        Assert.Single(result);

        var localizations = (Dictionary<string, Dictionary<string, string>>)((Dictionary<string, object>)result[0]["meta"])["localizations"];

        Assert.Single(localizations); // Only ValidArea
        Assert.True(localizations.ContainsKey("ValidArea"));
    }

    [Fact]
    public void BuildLocalizationExtensions_EmptyItems_ReturnsEmptyList()
    {
        // Arrange
        var items = new List<IDictionaryItem>();

        // Act
        var result = LocalizationBuilder.BuildLocalizationExtensions(items);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void BuildLocalizationExtensions_SetsCorrectExtensionMetadata()
    {
        // Arrange
        var items = new List<IDictionaryItem>
        {
            CreateDictionaryItem("Area_Key", new[] { ("en-US", "Value") })
        };

        // Act
        var result = LocalizationBuilder.BuildLocalizationExtensions(items);

        // Assert
        var extension = result[0];
        Assert.Equal("localization", extension["type"]);
        Assert.Equal("Lexicon.Localization.en-US", extension["alias"]);
        Assert.Equal("Lexicon Dictionary (en-US)", extension["name"]);
    }

    private static IDictionaryItem CreateDictionaryItem(string key, (string culture, string value)[] translations)
    {
        var mockItem = new Mock<IDictionaryItem>();
        mockItem.Setup(x => x.ItemKey).Returns(key);

        var translationList = translations.Select(t =>
        {
            var mockTranslation = new Mock<IDictionaryTranslation>();
            mockTranslation.Setup(x => x.LanguageIsoCode).Returns(t.culture);
            mockTranslation.Setup(x => x.Value).Returns(t.value);
            return mockTranslation.Object;
        }).ToList();

        mockItem.Setup(x => x.Translations).Returns(translationList);

        return mockItem.Object;
    }
}

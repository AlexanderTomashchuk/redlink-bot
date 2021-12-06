using System.Text;
using Domain.Entities;
using Domain.Extensions;

namespace Application.Common;

public static class BotMessage
{
    public static string GetWelcomeMessage(AppUser currentAppUser)
    {
        var sb = new StringBuilder();

        sb.AppendLine(
            $"{Emoji.HELLO} Hello {currentAppUser.GetTelegramMarkdownLink()}\\. I can help you sell or buy a variety of clothes\\.");
        sb.AppendLine();
        sb.AppendLine("You can control me by sending these commands:");
        foreach (var commandType in CommandTypeEnumeration.GetAll())
        {
            sb.AppendLine($"{commandType.Name} - {commandType.Description}".Escape());
        }

        return sb.ToString();
    }

    public static string GetInitCountryMessage()
    {
        var sb = new StringBuilder();

        sb.AppendLine("Please select your country from the list below\\.");
        sb.AppendLine();
        sb.AppendLine(
            "_This information will help us to provide you the most relevant products we have in you country\\._");

        return sb.ToString();
    }

    public static string GetEditCountryMessage()
    {
        var sb = new StringBuilder();

        sb.AppendLine("Please select your country from the list below\\.");

        return sb.ToString();
    }

    public static string GetProfileInfoMessage(AppUser currentAppUser)
    {
        var sb = new StringBuilder();

        sb.AppendLine("Your current profile settings:");
        sb.AppendLine();
        sb.AppendLine(
            $"`{$"{Emoji.COUNTRY} Country:",-13} {currentAppUser.Country.Name}`");
        sb.AppendLine($"`{$"{Emoji.LANGUAGE} Language:",-13} {currentAppUser.Language.Name}`");
        sb.AppendLine();
        sb.AppendLine("_To change profile settings, please use the buttons below\\._");

        return sb.ToString();
    }

    public static string GetEditLanguageMessage()
    {
        var sb = new StringBuilder();

        sb.AppendLine("Please choose your language from the list below\\.");

        return sb.ToString();
    }

    public static string GetSelectedCountryMessage(string countryName) => $"Selected country: {countryName}";

    public static string GetSelectedLanguageMessage(string languageName) => $"Selected language: {languageName}";

    public static string GetRequestProductNameMessage() => "Please enter a name for the product you want to sell";
    
    public static string GetRequestProductPhotoMessage() => "Please attach the photo of your product (up to 10 MB)";
    
    public static string GetRequestProductConditionMessage() => "Please choose the condition of the product";
    
    public static string GetRequestProductPriceMessage() => "Please enter a price of your product";

    public static string GetShowProductMessage() =>
        "There is a product you created.\n Please review the product information, edit if necessary and press 'Publish' button to post it.";
}
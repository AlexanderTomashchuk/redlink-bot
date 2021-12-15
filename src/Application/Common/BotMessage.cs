using System.Text;
using Domain.Entities;
using Domain.Extensions;
using l10n = Application.Resources.Localization;

namespace Application.Common;

public static class BotMessage
{
    public static string GetWelcomeMessage(AppUser currentAppUser)
    {
        var sb = new StringBuilder();

        sb.AppendLine(
            $"{Emoji.HELLO} {l10n.Hello} {currentAppUser.GetTelegramMarkdownLink()}\\. " +
            $"{l10n.ICanHelpToSellClothes}\\.");
        sb.AppendLine();
        sb.AppendLine($"{l10n.YouCanUseMeUsingCommands}:");
        foreach (var commandType in CommandType.List)
        {
            sb.AppendLine($"{commandType.Name} - {commandType.Description}".Escape());
        }

        return sb.ToString();
    }

    public static string GetInitCountryMessage()
    {
        var sb = new StringBuilder();

        sb.AppendLine($"{l10n.ChooseCountryFromList}\\.");
        sb.AppendLine();
        sb.AppendLine($"_{l10n.CountryHelpsProvideRelevantProducts}\\._");

        return sb.ToString();
    }

    public static string GetProfileInfoMessage(AppUser currentAppUser)
    {
        var sb = new StringBuilder();

        sb.AppendLine($"{l10n.YourCurrProfileSettings}:");
        sb.AppendLine();
        sb.AppendLine(
            $"`{$"{Emoji.COUNTRY} {l10n.Country}:",-13} {l10n.ResourceManager.GetString(currentAppUser.Country.NameLocalizationKey)}`");
        sb.AppendLine($"`{$"{Emoji.LANGUAGE} {l10n.Language}:",-13} {l10n.ResourceManager.GetString(currentAppUser.Language.NameLocalizationKey)}`");
        sb.AppendLine();
        sb.AppendLine($"_{l10n.UseBtnsToChangeProfile}\\._");

        return sb.ToString();
    }

    public static string GetShowProductMessage()
    {
        var sb = new StringBuilder();

        sb.AppendLine($"{l10n.TheProductYouCreated}\\.");
        sb.AppendLine();
        sb.AppendLine($"{l10n.ReviewProductInformation}\\.");

        return sb.ToString();
    }
}
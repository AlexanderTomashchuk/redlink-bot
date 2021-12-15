using System.Collections.Generic;
using Domain.Common;
using Domain.Extensions;
using Domain.ValueObjects;

namespace Domain.Entities;

public class AppUser : AuditableEntity
{
    /// <summary>
    /// Equals to Telegram.User.Id
    /// </summary>
    public long Id { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Username { get; set; }

    public long? ChatId { get; set; }

    public AppUserStatus? Status { get; set; } = AppUserStatus.Member;

    public long? CountryId { get; set; }

    public Country Country { get; set; }

    public bool HasCountry => CountryId != null;

    public string LanguageCodeName => (LanguageCode ?? Language.DefaultLanguageCode).ToString().ToLower();
    
    public Language.LanguageCode? LanguageCode { get; set; }

    public Language Language { get; set; }

    public ICollection<Product> Products { get; }
    
    //todo: consider to use WorkflowType instead of string
    //todo: rename this prop
    public string LastMessageWorkflowType { get; set; }

    private AppUser()
    {
    }

    public string GetUsername()
    {
        return (Username ?? string.Join(" ", FirstName, LastName).Trim()).Escape();
    }

    public string GetTelegramMarkdownLink()
    {
        var userName = GetUsername();
        return $"[{userName}](tg://user?id={Id})";
    }
}
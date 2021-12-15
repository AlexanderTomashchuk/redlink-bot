using System.Collections.Generic;
using Domain.Common;

namespace Domain.Entities;

public class Language : AuditableEntity
{
    public enum LanguageCode
    {
        En,
        Uk,
        Ru
    }

    public LanguageCode Code { get; }
    public static LanguageCode DefaultLanguageCode => LanguageCode.En;

    public string NameLocalizationKey { get; }

    public string Flag { get; }

    public ICollection<AppUser> Users { get; }

    // ReSharper disable once UnusedMember.Local
    private Language()
    {
    }

    public Language(LanguageCode code, string nameLocalizationKey, string flag, ICollection<AppUser> users = null)
    {
        Code = code;
        NameLocalizationKey = nameLocalizationKey;
        Flag = flag;
        Users = users;
    }
}
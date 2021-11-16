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

    public string Name { get; }

    public string Flag { get; }

    public ICollection<AppUser> Users { get; }

    private Language()
    {
    }

    public Language(LanguageCode code, string name, string flag, ICollection<AppUser> users = null)
    {
        Code = code;
        Name = name;
        Flag = flag;
        Users = users;
    }
}
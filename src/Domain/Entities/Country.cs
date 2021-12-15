using System.Collections.Generic;
using Domain.Common;

namespace Domain.Entities;

public class Country : AuditableEntity
{
    public long Id { get; }

    public string NameLocalizationKey { get; }

    public string Code { get; }

    public string Flag { get; }

    public ICollection<AppUser> Users { get; }

    // ReSharper disable once UnusedMember.Local
    private Country()
    {
    }

    public Country(long id, string nameLocalizationKey, string code, string flag, ICollection<AppUser> users = null)
    {
        Id = id;
        NameLocalizationKey = nameLocalizationKey;
        Code = code;
        Flag = flag;
        Users = users;
    }
}
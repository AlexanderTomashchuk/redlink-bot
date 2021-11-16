using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Application.Common;

public abstract class Enumeration
{
    public int Id { get; }
    public string Name { get; }

    public Enumeration(int id, string name) => (Id, Name) = (id, name);

    public override string ToString() => Name;

    public static IEnumerable<T> GetAll<T>() where T : Enumeration =>
        typeof(T).GetFields(BindingFlags.Public |
                            BindingFlags.Static |
                            BindingFlags.DeclaredOnly)
            .Select(f => f.GetValue(null))
            .Cast<T>()
            .OrderBy(s => s.Id);
}
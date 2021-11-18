using AutoMapper;
using Newtonsoft.Json;

namespace Application.Common.Mappings;

public class FromJsonTypeConverter<TDestination> : ITypeConverter<string, TDestination>
{
    public TDestination Convert(string source, TDestination destination, ResolutionContext context)
    {
        return JsonConvert.DeserializeObject<TDestination>(source);
    }
}
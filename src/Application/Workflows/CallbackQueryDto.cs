using System;
using Application.Workflows.CreateProduct;
using Application.Workflows.Profile;
using Application.Workflows.Start;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Application.Workflows;

[JsonConverter(typeof(CallbackQueryDtoConverter))]
public abstract class CallbackQueryDto
{
    [JsonProperty("wt")] public string WorkflowTypeString => WorkflowType.Name;

    [JsonIgnore] protected abstract WorkflowType WorkflowType { get; }

    [JsonProperty("ei")] public long? EntityId { get; private set; }

    protected CallbackQueryDto(long? entityId)
    {
        EntityId = entityId;
    }
}

public class CallbackQueryDtoConverter : JsonConverter
{
    static readonly JsonSerializerSettings SpecifiedSubclassConversion =
        new() { ContractResolver = new CallbackQueryDtoSpecifiedConcreteClassConverter() };

    public override bool CanConvert(Type objectType)
    {
        return typeof(CallbackQueryDto).IsAssignableFrom(objectType);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var jObject = JObject.Load(reader);

        Type typeOfDto = null;

        WorkflowType.FromName(jObject["wt"].Value<string>())
            .When(WorkflowType.CreateProduct).Then(() => typeOfDto = typeof(CreateProductCbDto))
            .When(WorkflowType.EditProfile).Then(() => typeOfDto = typeof(EditProfileCqDto))
            .When(WorkflowType.DemandCountry).Then(() => typeOfDto = typeof(DemandCountryCbDto));

        return JsonConvert.DeserializeObject(jObject.ToString(), typeOfDto, SpecifiedSubclassConversion);
    }

    public override bool CanWrite => false;

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
    }
}

public class CallbackQueryDtoSpecifiedConcreteClassConverter : DefaultContractResolver
{
    protected override JsonConverter ResolveContractConverter(Type objectType)
    {
        if (typeof(CallbackQueryDto).IsAssignableFrom(objectType) && !objectType.IsAbstract)
            return null; // pretend TableSortRuleConvert is not specified (thus avoiding a stack overflow)
        return base.ResolveContractConverter(objectType);
    }
}
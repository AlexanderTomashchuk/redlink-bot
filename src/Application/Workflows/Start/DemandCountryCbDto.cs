using Newtonsoft.Json;

namespace Application.Workflows.Start;

public class DemandCountryCbDto : CallbackQueryDto
{
    protected override WorkflowType WorkflowType => WorkflowType.DemandCountry;

    [JsonProperty("s")] public DemandCountryWorkflow.State State { get; private set; }

    [JsonProperty("t")] public DemandCountryWorkflow.Trigger Trigger { get; private set; }

    public DemandCountryCbDto(DemandCountryWorkflow.State state, DemandCountryWorkflow.Trigger trigger,
        long? entityId = default) : base(entityId)
    {
        State = state;
        Trigger = trigger;
    }
}
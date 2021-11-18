using Newtonsoft.Json;

namespace Application.Workflows.Profile;

public class ProfileWorkflowDto
{
    [JsonProperty("s")] public ProfileWorkflow.State State { get; set; }

    [JsonProperty("t")] public ProfileWorkflow.Trigger Trigger { get; set; }

    [JsonProperty("ei")] public long? EntityId { get; set; }

    public CallbackQueryDto ToCallbackQueryDto()
    {
        var callbackQueryDto = new CallbackQueryDto { WorkflowType = WorkflowType.Profile, ProfileWorkflowDto = this };
        return callbackQueryDto;
    }

    public void Deconstruct(out ProfileWorkflow.State state, out ProfileWorkflow.Trigger trigger, out long? entityId)
    {
        state = State;
        trigger = Trigger;
        entityId = EntityId;
    }
}
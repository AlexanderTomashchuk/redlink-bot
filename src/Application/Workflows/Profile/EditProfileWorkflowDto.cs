using Newtonsoft.Json;

namespace Application.Workflows.Profile;

public class EditProfileWorkflowDto
{
    [JsonProperty("s")] public EditProfileWorkflow.State State { get; set; }

    [JsonProperty("t")] public EditProfileWorkflow.Trigger Trigger { get; set; }

    [JsonProperty("ei")] public long? EntityId { get; set; }

    public CallbackQueryDto ToCallbackQueryDto()
    {
        var callbackQueryDto = new CallbackQueryDto { WorkflowType = nameof(WorkflowType.EditProfile), EditProfileWorkflowDto = this };
        return callbackQueryDto;
    }

    public void Deconstruct(out EditProfileWorkflow.State state, out EditProfileWorkflow.Trigger trigger, out long? entityId)
    {
        state = State;
        trigger = Trigger;
        entityId = EntityId;
    }
}
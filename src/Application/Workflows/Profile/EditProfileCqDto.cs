using Newtonsoft.Json;

namespace Application.Workflows.Profile;

public class EditProfileCqDto : CallbackQueryDto
{
    protected override WorkflowType WorkflowType => WorkflowType.EditProfile;

    [JsonProperty("s")] public EditProfileWorkflow.State State { get; private set; }

    [JsonProperty("t")] public EditProfileWorkflow.Trigger Trigger { get; private set; }

    public EditProfileCqDto(EditProfileWorkflow.State state, EditProfileWorkflow.Trigger trigger,
        long? entityId = default) : base (entityId)
    {
        State = state;
        Trigger = trigger;
    }

    public void Deconstruct(out EditProfileWorkflow.State state, out EditProfileWorkflow.Trigger trigger,
        out long? entityId)
    {
        state = State;
        trigger = Trigger;
        entityId = EntityId;
    }
}
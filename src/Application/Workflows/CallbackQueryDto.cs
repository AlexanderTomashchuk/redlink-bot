using Application.Workflows.Profile;
using Newtonsoft.Json;

namespace Application.Workflows;

public class CallbackQueryDto
{
    [JsonProperty("w")] public WorkflowType WorkflowType { get; set; }

    [JsonProperty("pwd")] public ProfileWorkflowDto ProfileWorkflowDto { get; set; }
}
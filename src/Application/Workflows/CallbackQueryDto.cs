using Application.Workflows.CreateProduct;
using Application.Workflows.Profile;
using Newtonsoft.Json;

namespace Application.Workflows;

public class CallbackQueryDto
{
    [JsonProperty("w")] public string WorkflowType { get; set; }

    [JsonProperty("pwd")] public EditProfileWorkflowDto EditProfileWorkflowDto { get; set; }

    [JsonProperty("cpwd")] public CreateProductWorkflowDto CreateProductWorkflowDto { get; set; }
}
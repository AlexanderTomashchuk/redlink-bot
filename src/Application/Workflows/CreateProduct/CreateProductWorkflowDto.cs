using Newtonsoft.Json;

namespace Application.Workflows.CreateProduct;

public class CreateProductWorkflowDto
{
    [JsonProperty("t")] public CreateProductWorkflow.Trigger Trigger { get; set; }

    [JsonProperty("ei")] public long? EntityId { get; set; }

    public CallbackQueryDto ToCallbackQueryDto()
    {
        var callbackQueryDto = new CallbackQueryDto
            { WorkflowType = nameof(WorkflowType.CreateProduct), CreateProductWorkflowDto = this };
        return callbackQueryDto;
    }
}
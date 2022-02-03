using Newtonsoft.Json;

namespace Application.Workflows.CreateProduct;

public class CreateProductCbDto : CallbackQueryDto
{
    [JsonProperty("t")] public CreateProductWorkflow.Trigger Trigger { get; set; }

    protected override WorkflowType WorkflowType => WorkflowType.CreateProduct;

    [JsonProperty(PropertyName = "ib")]
    public bool IsBackButton { get; set; }

    public CreateProductCbDto(CreateProductWorkflow.Trigger trigger, long? entityId) : base(entityId)
    {
        Trigger = trigger;
    }
}
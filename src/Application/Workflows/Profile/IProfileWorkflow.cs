using System.Threading;
using System.Threading.Tasks;

namespace Application.Workflows.Profile;

//todo: remove
// public interface IProfileWorkflow
// {
//     IConfigureCallbackQueryIdState ForMessageId(int messageId);
//
//     IConfigureCallbackQueryIdState SkipForMessageId { get; }
//
//     public interface IConfigureCallbackQueryIdState
//     {
//         IConfigureStateMachineStage ForCallbackQueryId(string callbackQueryId);
//         IConfigureStateMachineStage SkipCallbackQueryId { get; }
//     }
//
//     public interface IConfigureStateMachineStage
//     {
//         IProvideActionsStage ConfigureStateMachine(EditProfileWorkflow.State state);
//     }
//
//     public interface IProvideActionsStage
//     {
//         Task TriggerAsync(EditProfileWorkflow.Trigger trigger, long? entityId = default,
//             CancellationToken cancellationToken = default);
//     }
//}
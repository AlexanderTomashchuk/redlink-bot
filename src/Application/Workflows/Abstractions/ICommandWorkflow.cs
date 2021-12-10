namespace Application.Workflows.Abstractions;

public interface ICommandWorkflow
{
    CommandType CommandType { get; }
}

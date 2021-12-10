namespace Application.Workflows.Abstractions;

public static class StateStorageMode
{
    public interface IExternal<TState>
        where TState : struct
    {
        public TState GetState();

        public void SetState(TState state);
    }

    public interface ITransitional<out TState>
        where TState : struct
    {
        public TState CurrentState { get; }
    }
}
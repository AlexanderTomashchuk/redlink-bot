using System;
using System.Threading.Tasks;
using Stateless;

namespace Application.Workflows;

public class StateMachineFactoryAdapter
{
    public async Task<StateMachine<TState, TTrigger>> CreateAsync<TState, TTrigger>(
        FiringMode firingMode = FiringMode.Queued)
        where TState : struct
        where TTrigger : struct
    {
        Func<TState> stateAccessor = () => new TState();
        Action<TState> stateMutator = state => state = state;
        
        var machine = new StateMachine<TState, TTrigger>( stateAccessor, stateMutator, firingMode);

        return machine;
    }
}

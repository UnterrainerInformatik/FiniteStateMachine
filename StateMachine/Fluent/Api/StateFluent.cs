using System;

namespace StateMachine.Fluent.Api
{
    interface StateFluent<STATE, TRIGGER> : BuilderFluent<STATE, TRIGGER>
    {
        TransitionFluent<STATE, TRIGGER> TransitionTo(STATE state);

        StateFluent<STATE, TRIGGER> OnEnter(Action<STATE, TRIGGER> enter);

        StateFluent<STATE, TRIGGER> OnExit(Action<STATE, TRIGGER> exit);

        StateFluent<STATE, TRIGGER> Update(Action<Fsm<STATE, TRIGGER>, float> update);
    }
}

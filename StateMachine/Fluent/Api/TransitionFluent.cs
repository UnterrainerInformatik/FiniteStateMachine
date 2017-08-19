using System;

namespace StateMachine.Fluent.Api
{
    interface TransitionFluent<STATE, TRIGGER>
    {
        TransitionStateFluent<STATE, TRIGGER> On(TRIGGER trigger);
        TransitionStateFluent<STATE, TRIGGER> If(Func<bool> condition);
    }
}

namespace StateMachine.Fluent.Api
{
    interface TransitionStateFluent<STATE, TRIGGER> : TransitionFluent<STATE, TRIGGER>, StateFluent<STATE, TRIGGER>
    {
    }
}

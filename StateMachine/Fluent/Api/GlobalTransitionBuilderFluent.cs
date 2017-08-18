namespace StateMachine.Fluent.Api
{
    interface GlobalTransitionBuilderFluent<STATE, TRIGGER> : GlobalTransitionFluent<STATE, TRIGGER>, BuilderFluent<STATE, TRIGGER>
    {
    }
}

namespace StateMachine.Fluent.Api
{
    interface BuilderFluent<STATE, TRIGGER>
    {
        GlobalTransitionFluent<STATE, TRIGGER> GlobalTransitionTo(STATE state);

        StateFluent<STATE, TRIGGER> State(STATE state);

        Fsm<STATE, TRIGGER> Build();
    }
}

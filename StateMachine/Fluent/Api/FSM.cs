namespace StateMachine.Fluent.Api
{
    public class Fsm<STATE, TRIGGER>
    {
        public STATE State { get; private set; }

        public void TransitionTo(STATE state)
        {

        }
    }
}

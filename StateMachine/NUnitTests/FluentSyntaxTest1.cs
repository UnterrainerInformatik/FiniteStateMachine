using System.Collections.Generic;
using StateMachine.Fluent.Api;

namespace StateMachine.NUnitTests
{
    class Spell
    {

    }
    class Timer
    {
        public void Start() { }
        public float Value { get; }
        public void StopAndReset() { }
    }

    class Hero
    {
        public void DoSpell(Spell spell) { }
    }

    abstract class Button
    {
        public enum ButtonKind { FLIPBACK }
        public enum ButtonState { IDLE, OVER, DOWN, REFRESHING };
        public ButtonState State { get; set; }   
        public ButtonKind Kind { get; set; }
        public Timer RefreshTimer { get; }

        public abstract Spell DoAssociatedSpell();
    }
    
    class FluentSyntaxTest1
    {
        private enum State { IDLE, OVER, PRESSED, REFRESHING };
        private enum Trigger { MOUSE_CLICKED, MOUSE_RELEASED, MOUSE_OVER, MOUSE_LEAVE };

        private Dictionary<Button, Fsm<State, Trigger>> buttonMachines = new Dictionary<Button, Fsm<State, Trigger>>();

        private void CreateMachineFor(BuilderFluent<State, Trigger> builder, Button button, Hero hero) { 
            buttonMachines.Add(button, builder
                .State(State.IDLE)
                .TransitionTo(State.OVER).On(Trigger.MOUSE_OVER)
                .OnEnter((s, t) => {
                    button.State = Button.ButtonState.IDLE;
                })
            .State(State.OVER)
                .TransitionTo(State.IDLE).On(Trigger.MOUSE_LEAVE)
                .TransitionTo(State.PRESSED).On(Trigger.MOUSE_CLICKED)
                .OnEnter((s, t) => {
                    button.State = Button.ButtonState.OVER;
                })
            .State(State.PRESSED)
                .TransitionTo(State.IDLE).On(Trigger.MOUSE_LEAVE).If(() => button.Kind == Button.ButtonKind.FLIPBACK)
                .TransitionTo(State.REFRESHING).On(Trigger.MOUSE_RELEASED)
                .OnEnter((s, t) => {
                    button.State = Button.ButtonState.DOWN;
                })
            .State(State.REFRESHING)
                .OnEnter((s, t) => {
                    hero.DoSpell(button.DoAssociatedSpell());
                    button.RefreshTimer.Start();
                    button.State = Button.ButtonState.REFRESHING;
                })
                .Update((machine, gameTime) => {
                    if (button.RefreshTimer.Value <= 0F)
                    {
                        button.RefreshTimer.StopAndReset();
                        machine.TransitionTo(State.IDLE);
                    }
                })
            .Build());
        }
    }
}

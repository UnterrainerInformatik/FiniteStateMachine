// *************************************************************************** 
// This is free and unencumbered software released into the public domain.
// 
// Anyone is free to copy, modify, publish, use, compile, sell, or
// distribute this software, either in source code form or as a compiled
// binary, for any purpose, commercial or non-commercial, and by any
// means.
// 
// In jurisdictions that recognize copyright laws, the author or authors
// of this software dedicate any and all copyright interest in the
// software to the public domain. We make this dedication for the benefit
// of the public at large and to the detriment of our heirs and
// successors. We intend this dedication to be an overt act of
// relinquishment in perpetuity of all present and future rights to this
// software under copyright law.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
// OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
// ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
// 
// For more information, please refer to <http://unlicense.org>
// ***************************************************************************

using System.Collections.Generic;
using StateMachine.Fluent.Api;

namespace StateMachine.NUnitTests
{
    class Spell
    {
    }

    class Timer
    {
        public void Start()
        {
        }

        public float Value { get; }

        public void StopAndReset()
        {
        }
    }

    class Hero
    {
        public void DoSpell(Spell spell)
        {
        }
    }

    class Button
    {
        public enum ButtonKind
        {
            FLIPBACK
        }

        public enum ButtonState
        {
            IDLE,
            OVER,
            DOWN,
            REFRESHING
        }

        public ButtonState State { get; set; }
        public ButtonKind Kind { get; set; }
        public Timer RefreshTimer { get; }

        public Spell DoAssociatedSpell() {
            //Do something useful.
            return new Spell();
        }
    }

    class FluentSyntaxTest1
    {
        private enum State
        {
            IDLE,
            OVER,
            PRESSED,
            REFRESHING
        };

        private enum Trigger
        {
            MOUSE_CLICKED,
            MOUSE_RELEASED,
            MOUSE_OVER,
            MOUSE_LEAVE
        };

        private readonly Dictionary<Button, Fsm<State, Trigger, float>> buttonMachines =
            new Dictionary<Button, Fsm<State, Trigger, float>>();

        private void main()
        {
            Hero hero = new Hero();
            CreateMachineFor(Fsm<State, Trigger, float> , new Button(), hero);
            CreateMachineFor(Fsm<State, Trigger, float>, new Button(), hero);
        }

        private void CreateMachineFor(BuilderFluent<State, Trigger, float> builder, Button button, Hero hero)
        {
            buttonMachines.Add(button, builder
                .State(State.IDLE)
                .TransitionTo(State.OVER).On(Trigger.MOUSE_OVER)
                .OnEnter((s, t) => { button.State = Button.ButtonState.IDLE; })
                .State(State.OVER)
                .TransitionTo(State.IDLE).On(Trigger.MOUSE_LEAVE)
                .TransitionTo(State.PRESSED).On(Trigger.MOUSE_CLICKED)
                .OnEnter((s, t) => { button.State = Button.ButtonState.OVER; })
                .State(State.PRESSED)
                .TransitionTo(State.IDLE).On(Trigger.MOUSE_LEAVE).If(() => button.Kind == Button.ButtonKind.FLIPBACK)
                .TransitionTo(State.REFRESHING).On(Trigger.MOUSE_RELEASED)
                .OnEnter((s, t) => { button.State = Button.ButtonState.DOWN; })
                .State(State.REFRESHING)
                .OnEnter((s, t) =>
                {
                    hero.DoSpell(button.DoAssociatedSpell());
                    button.RefreshTimer.Start();
                    button.State = Button.ButtonState.REFRESHING;
                })
                .Update((machine, gameTime) =>
                {
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
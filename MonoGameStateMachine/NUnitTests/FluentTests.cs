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

using System;
using Microsoft.Xna.Framework;
using NUnit.Framework;

namespace MonoGameStateMachine.NUnitTests
{
    public class FluentTests
    {
        private enum State
        {
            IDLE,
            OVER,
            PRESSED,
            REFRESHING
        }

        private enum Trigger
        {
            MOUSE_CLICKED,
            MOUSE_RELEASED,
            MOUSE_OVER,
            MOUSE_LEAVE
        }

        private class Button
        {
            public bool IsActivated { get; set; }
            public State BtnState { get; set; }
            public State OldState { get; set; } = State.IDLE;

            public int UpdateCounter { get; set; }
        }

        [Test]
        [Category("MonoGameStateMachine.FluentTests")]
        public void OnMethodHooksShouldBeTriggeredCorrectly()
        {
            var button = new Button();

            var m = Fsm<State, Trigger>.Builder(State.IDLE)
                .State(State.IDLE)
                    .TransitionTo(State.OVER).On(Trigger.MOUSE_OVER)
                    .OnEnter(t => button.BtnState = State.IDLE)
                    .OnExit(t => button.OldState = button.BtnState)
                .State(State.OVER)
                    .TransitionTo(State.IDLE).On(Trigger.MOUSE_LEAVE)
                    .TransitionTo(State.PRESSED).On(Trigger.MOUSE_CLICKED)
                    .OnEnter(t => button.BtnState = State.OVER)
                    .OnExit(t => button.OldState = button.BtnState)
                    .Update(a => button.UpdateCounter = button.UpdateCounter + 1)
                .State(State.PRESSED)
                    .TransitionTo(State.IDLE).On(Trigger.MOUSE_LEAVE)
                    .TransitionTo(State.REFRESHING).On(Trigger.MOUSE_RELEASED).If(a => button.IsActivated)
                    .OnEnter(t => button.BtnState = State.PRESSED)
                    .OnExit(t => button.OldState = button.BtnState)
                .State(State.REFRESHING)
                    .OnEnter(t => button.BtnState = State.REFRESHING)
                    .OnExit(t => button.OldState = button.BtnState)
                    .TransitionTo(State.OVER).After(1, TimeUnit.SECONDS)
                .GlobalTransitionTo(State.IDLE).AfterGlobal(10, TimeUnit.SECONDS)
                .Build();

            m.Update(new GameTime(TimeSpan.FromDays(1), TimeSpan.FromMilliseconds(16))); // Should do nothing.
            Assert.That(m.Current.Identifier, Is.EqualTo(State.IDLE));
            m.Trigger(Trigger.MOUSE_CLICKED);
            Assert.That(m.Current.Identifier, Is.EqualTo(State.IDLE));
            m.Trigger(Trigger.MOUSE_LEAVE);
            Assert.That(m.Current.Identifier, Is.EqualTo(State.IDLE));
            m.Trigger(Trigger.MOUSE_RELEASED);
            Assert.That(m.Current.Identifier, Is.EqualTo(State.IDLE));
            m.Trigger(Trigger.MOUSE_OVER);
            m.Update(new GameTime(TimeSpan.FromDays(1), TimeSpan.FromMilliseconds(16)));
            m.Update(new GameTime(TimeSpan.FromDays(1), TimeSpan.FromMilliseconds(16)));
            Assert.That(m.Current.Identifier, Is.EqualTo(State.OVER));
            Assert.That(button.BtnState, Is.EqualTo(State.OVER));
            Assert.That(button.OldState, Is.EqualTo(State.IDLE));
            m.Trigger(Trigger.MOUSE_CLICKED);
            Assert.That(m.Current.Identifier, Is.EqualTo(State.PRESSED));
            Assert.That(button.BtnState, Is.EqualTo(State.PRESSED));
            Assert.That(button.OldState, Is.EqualTo(State.OVER));
            m.Trigger(Trigger.MOUSE_RELEASED); // Button is deactivated.
            Assert.That(m.Current.Identifier, Is.EqualTo(State.PRESSED));
            Assert.That(button.BtnState, Is.EqualTo(State.PRESSED));
            Assert.That(button.OldState, Is.EqualTo(State.OVER));
            button.IsActivated = true;
            m.Trigger(Trigger.MOUSE_RELEASED); // Now it's activated.
            Assert.That(m.Current.Identifier, Is.EqualTo(State.REFRESHING));
            Assert.That(button.BtnState, Is.EqualTo(State.REFRESHING));
            Assert.That(button.OldState, Is.EqualTo(State.PRESSED));
            m.Update(new GameTime(TimeSpan.FromDays(1), TimeSpan.FromMilliseconds(500))); // No transition yet...
            Assert.That(m.Current.Identifier, Is.EqualTo(State.REFRESHING));
            Assert.That(button.BtnState, Is.EqualTo(State.REFRESHING));
            Assert.That(button.OldState, Is.EqualTo(State.PRESSED));
            m.Update(new GameTime(TimeSpan.FromDays(1), TimeSpan.FromMilliseconds(600))); // But now.
            Assert.That(m.Current.Identifier, Is.EqualTo(State.OVER));
            Assert.That(button.BtnState, Is.EqualTo(State.OVER));
            Assert.That(button.OldState, Is.EqualTo(State.REFRESHING));
            m.Update(new GameTime(TimeSpan.FromDays(1), TimeSpan.FromMilliseconds(10000)));
            Assert.That(m.Current.Identifier, Is.EqualTo(State.IDLE));
            Assert.That(button.BtnState, Is.EqualTo(State.IDLE));
            Assert.That(button.OldState, Is.EqualTo(State.OVER));

            // Update was triggered twice over all states.
            Assert.That(button.UpdateCounter, Is.EqualTo(3F));
        }

        [Test]
        [Category("MonoGameStateMachine.FluentTests")]
        public void WhenMultipleAfterConditionsFireOnASingleUpdateAdvanceStateCorrectly()
        {
            var m = Fsm<State, Trigger>.Builder(State.IDLE)
                .State(State.IDLE)
                    .TransitionTo(State.OVER).After(10, TimeUnit.MILLISECONDS)
                .State(State.OVER)
                    .TransitionTo(State.PRESSED).After(10, TimeUnit.MILLISECONDS)
                .State(State.PRESSED)
                    .TransitionTo(State.REFRESHING).After(10, TimeUnit.MILLISECONDS)
                .State(State.REFRESHING)
                    .TransitionTo(State.IDLE).After(10, TimeUnit.MILLISECONDS)
                .Build();

            Assert.That(m.Current.Identifier, Is.EqualTo(State.IDLE));
            m.Update(new GameTime(TimeSpan.FromDays(1), TimeSpan.FromMilliseconds(40)));
            Assert.That(m.Current.Identifier, Is.EqualTo(State.IDLE));
        }

        [Test]
        [Category("MonoGameStateMachine.FluentTests")]
        public void WhenMultipleAfterConditionsFireOnASingleUpdateOnEnterAndOnExitShoudFire()
        {
            int enterCount = 0;
            int exitCount = 0;
            var m = Fsm<State, Trigger>.Builder(State.IDLE)
                .State(State.IDLE)
                    .OnEnter(t => enterCount++)
                    .OnExit(t => exitCount++)
                    .TransitionTo(State.OVER).After(10, TimeUnit.MILLISECONDS)
                .State(State.OVER)
                    .OnEnter(t => enterCount++)
                    .OnExit(t => exitCount++)
                    .TransitionTo(State.PRESSED).After(10, TimeUnit.MILLISECONDS)
                .State(State.PRESSED)
                    .OnEnter(t => enterCount++)
                    .OnExit(t => exitCount++)
                    .TransitionTo(State.REFRESHING).After(10, TimeUnit.MILLISECONDS)
                .State(State.REFRESHING)
                    .OnEnter(t => enterCount++)
                    .OnExit(t => exitCount++)
                    .TransitionTo(State.IDLE).After(10, TimeUnit.MILLISECONDS)
                .Build();

            Assert.That(m.Current.Identifier, Is.EqualTo(State.IDLE));
            m.Update(new GameTime(TimeSpan.FromDays(1), TimeSpan.FromMilliseconds(40)));
            Assert.That(enterCount, Is.EqualTo(4));
            Assert.That(exitCount, Is.EqualTo(4));
        }
    }
}
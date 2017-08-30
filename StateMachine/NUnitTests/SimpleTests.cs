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

using System.Linq;
using NUnit.Framework;

namespace StateMachine.NUnitTests
{
    [TestFixture]
    [Category("StateMachine.Simple")]
    public class SimpleTests
    {
        private enum State
        {
            OPENED,
            CLOSED,
            POP
        }

        private enum Trigger
        {
            CLOSE,
            OPEN,
            PUSH_POP
        }

        [Test]
        [Category("StateMachine.Simple")]
        public void SimpleTest()
        {
            State<State, Trigger, float> opened = new State<State, Trigger, float>(State.OPENED);
            State<State, Trigger, float> closed = new State<State, Trigger, float>(State.CLOSED)
            {
                ClearStack = true
            };
            opened.AddTransisionOn(Trigger.CLOSE, State.CLOSED).AddTransisionOn(Trigger.OPEN, State.OPENED);
            closed.AddTransisionOn(Trigger.OPEN, State.OPENED).AddTransisionOn(Trigger.CLOSE, State.CLOSED);

            Fsm<State, Trigger, float> m =
                new Fsm<State, Trigger, float>(opened, true)
                    .AddStateChangeHandler(TestTools.ConsoleOut)
                    .Add(opened)
                    .Add(closed);

            TestSimple(m);
        }

        [Test]
        [Category("StateMachine.Simple")]
        public void FluentTest()
        {
            var m = Fsm<State, Trigger>.Builder(State.OPENED)
                .EnableStack()
                .State(State.OPENED)
                    .TransitionTo(State.OPENED).On(Trigger.OPEN)
                    .TransitionTo(State.CLOSED).On(Trigger.CLOSE)
                .State(State.CLOSED).ClearsStack()
                    .TransitionTo(State.CLOSED).On(Trigger.CLOSE)
                    .TransitionTo(State.OPENED).On(Trigger.OPEN)
                .Build();

            TestSimple(m);
        }
        
        private void TestSimple(Fsm<State, Trigger, float> m)
        {
            m.Trigger(Trigger.OPEN);
            Assert.That(m.Current.Identifier, Is.EqualTo(State.OPENED));
            Assert.That(m.Stack.Select(x => x.Identifier).ToArray(), Is.EquivalentTo(new[] {State.OPENED, State.OPENED}));

            m.Trigger(Trigger.CLOSE);
            Assert.That(m.Current.Identifier, Is.EqualTo(State.CLOSED));
            Assert.That(m.Stack.ToArray(), Is.EquivalentTo(new State<State, Trigger, float>[] {}));

            m.Trigger(Trigger.CLOSE);
            Assert.That(m.Current.Identifier, Is.EqualTo(State.CLOSED));
            Assert.That(m.Stack.ToArray(), Is.EquivalentTo(new State<State, Trigger, float>[] {}));

            m.Trigger(Trigger.OPEN);
            Assert.That(m.Current.Identifier, Is.EqualTo(State.OPENED));
            Assert.That(m.Stack.Select(x => x.Identifier).ToArray(), Is.EquivalentTo(new[] {State.OPENED}));
        }

        [Test]
        [Category("StateMachine.Simple")]
        public void FluentWithStringTest()
        {
            var m = Fsm<string, string>.Builder("OPENED")
                .EnableStack()
                .State("OPENED")
                    .TransitionTo("OPENED").On("OPEN")
                    .TransitionTo("CLOSED").On("CLOSE")
                .State("CLOSED").ClearsStack()
                    .TransitionTo("CLOSED").On("CLOSE")
                    .TransitionTo("OPENED").On("OPEN")
                .Build();

            m.Trigger("OPEN");
            Assert.That(m.Current.Identifier, Is.EqualTo("OPENED"));
            Assert.That(m.Stack.Select(x => x.Identifier).ToArray(), Is.EquivalentTo(new[] { "OPENED", "OPENED" }));

            m.Trigger("CLOSE");
            Assert.That(m.Current.Identifier, Is.EqualTo("CLOSED"));
            Assert.That(m.Stack.ToArray(), Is.EquivalentTo(new State<State, Trigger, float>[] { }));

            m.Trigger("CLOSE");
            Assert.That(m.Current.Identifier, Is.EqualTo("CLOSED"));
            Assert.That(m.Stack.ToArray(), Is.EquivalentTo(new State<State, Trigger, float>[] { }));

            m.Trigger("OPEN");
            Assert.That(m.Current.Identifier, Is.EqualTo("OPENED"));
            Assert.That(m.Stack.Select(x => x.Identifier).ToArray(), Is.EquivalentTo(new[] { "OPENED" }));
        }

        [Test]
        [Category("StateMachine.Simple")]
        public void SimpleTestWithPop()
        {
            State<State, Trigger, float> opened = new State<State, Trigger, float>(State.OPENED);
            State<State, Trigger, float> closed = new State<State, Trigger, float>(State.CLOSED)
            {
                ClearStack = true
            };
            State<State, Trigger, float> pop = new State<State, Trigger, float>(State.POP);
            opened.AddTransisionOn(Trigger.CLOSE, State.CLOSED)
                .AddTransisionOn(Trigger.OPEN, State.OPENED)
                .AddTransisionOn(Trigger.PUSH_POP, State.POP);
            pop.AddPopTransisionOn(Trigger.PUSH_POP);
            closed.AddTransisionOn(Trigger.OPEN, State.OPENED)
                .AddTransisionOn(Trigger.CLOSE, State.CLOSED);

            Fsm<State, Trigger, float> m =
                new Fsm<State, Trigger, float>(opened, true)
                    .AddStateChangeHandler(TestTools.ConsoleOut)
                    .Add(opened)
                    .Add(closed)
                    .Add(pop);

            TestWithPop(m);
        }

        [Test]
        [Category("StateMachine.Simple")]
        public void FluentTestWithPop()
        {
            var m = Fsm<State, Trigger>.Builder(State.OPENED)
                .EnableStack()
                .State(State.OPENED)
                    .TransitionTo(State.OPENED).On(Trigger.OPEN)
                    .TransitionTo(State.CLOSED).On(Trigger.CLOSE)
                    .TransitionTo(State.POP).On(Trigger.PUSH_POP)
                .State(State.CLOSED).ClearsStack()
                    .TransitionTo(State.CLOSED).On(Trigger.CLOSE)
                    .TransitionTo(State.OPENED).On(Trigger.OPEN)
                .State(State.POP)
                    .PopTransition().On(Trigger.PUSH_POP)
                .Build();

            TestWithPop(m);
        }

        private void TestWithPop(Fsm<State, Trigger, float> m)
        {
            m.Trigger(Trigger.OPEN);
            Assert.That(m.Current.Identifier, Is.EqualTo(State.OPENED));
            Assert.That(m.Stack.Select(x => x.Identifier).ToArray(), Is.EquivalentTo(new[] { State.OPENED, State.OPENED }));

            m.Trigger(Trigger.PUSH_POP);
            Assert.That(m.Current.Identifier, Is.EqualTo(State.POP));
            Assert.That(m.Stack.Select(x => x.Identifier).ToArray(), Is.EquivalentTo(new[] { State.OPENED, State.OPENED, State.POP }));

            m.Trigger(Trigger.PUSH_POP);
            Assert.That(m.Current.Identifier, Is.EqualTo(State.OPENED));
            Assert.That(m.Stack.Select(x => x.Identifier).ToArray(), Is.EquivalentTo(new[] { State.OPENED, State.OPENED }));

            m.Trigger(Trigger.CLOSE);
            Assert.That(m.Current.Identifier, Is.EqualTo(State.CLOSED));
            Assert.That(m.Stack.ToArray(), Is.EquivalentTo(new State<State, Trigger, float>[] { }));

            m.Trigger(Trigger.CLOSE);
            Assert.That(m.Current.Identifier, Is.EqualTo(State.CLOSED));
            Assert.That(m.Stack.ToArray(), Is.EquivalentTo(new State<State, Trigger, float>[] { }));

            m.Trigger(Trigger.OPEN);
            Assert.That(m.Current.Identifier, Is.EqualTo(State.OPENED));
            Assert.That(m.Stack.Select(x => x.Identifier).ToArray(), Is.EquivalentTo(new[] { State.OPENED }));
        }
    }
}
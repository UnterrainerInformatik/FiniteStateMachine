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
            CLOSED
        };

        private enum Trigger
        {
            CLOSE,
            OPEN,
            PUSH,
            POP
        };

        [Test]
        [Category("StateMachine.Simple")]
        public void SimpleTest()
        {
            State<State, Trigger, float> opened = new State<State, Trigger, float>(State.OPENED);
            State<State, Trigger, float> closed = new State<State, Trigger, float>(State.CLOSED)
            {
                EndState = true,
                ClearStack = true
            };
            opened.Add(new Transition<State, Trigger, float>(Trigger.CLOSE, closed))
                .Add(new Transition<State, Trigger, float>(Trigger.OPEN, opened));
            closed.Add(new Transition<State, Trigger, float>(Trigger.OPEN, opened))
                .Add(new Transition<State, Trigger, float>(Trigger.CLOSE, closed));

            Fsm<State, Trigger, float> m =
                new Fsm<State, Trigger, float>(opened).AddStateChangeHandler(TestTools.ConsoleOut);
            m.Add(opened);
            m.Add(closed);

            m.Trigger(Trigger.OPEN);
            Assert.That(State.OPENED, Is.EqualTo(m.Current));
            Assert.That(m.Stack.ToArray(), Is.EquivalentTo(new[] {opened, opened}));

            m.Trigger(Trigger.CLOSE);
            Assert.That(State.CLOSED, Is.EqualTo(m.Current));
            Assert.That(m.Stack.ToArray(), Is.EquivalentTo(new State<State, Trigger, float>[] {}));

            m.Trigger(Trigger.CLOSE);
            Assert.That(State.CLOSED, Is.EqualTo(m.Current));
            Assert.That(m.Stack.ToArray(), Is.EquivalentTo(new State<State, Trigger, float>[] {}));

            m.Trigger(Trigger.OPEN);
            Assert.That(State.OPENED, Is.EqualTo(m.Current));
            Assert.That(m.Stack.ToArray(), Is.EquivalentTo(new[] {opened}));
        }

        [Test]
        [Category("StateMachine.Simple")]
        public void SimpleTestWithPop()
        {
            State<string> opened = new State<string>("opened");
            State<string> closed = new State<string>("closed") {EndState = true, ClearStack = true};
            State<string> test = new State<string>("pop");
            opened.Add(new Transition<string>("close", "c", closed))
                .Add(new Transition<string>("open", "o", opened))
                .Add(new Transition<string>("push", "p", test));
            test.Add(new Transition<string> {Name = "pop", Trigger = "p", Target = opened, Pop = true});
            closed.Add(new Transition<string>("open", "o", opened)).Add(new Transition<string>("close", "c", closed));

            Fsm<string> m =
                new Fsm<string>(opened).AddStateChangedHandler(TestTools.ConsoleOut);

            m.Trigger("o");
            Assert.That("opened", Is.EqualTo(m.Current.Name));
            Assert.That(m.Stack.ToArray(), Is.EquivalentTo(new[] {opened, opened}));

            m.Trigger("p");
            Assert.That("pop", Is.EqualTo(m.Current.Name));
            Assert.That(m.Stack.ToArray(), Is.EquivalentTo(new[] {opened, opened, test}));

            m.Trigger("p");
            Assert.That("opened", Is.EqualTo(m.Current.Name));
            Assert.That(m.Stack.ToArray(), Is.EquivalentTo(new[] {opened, opened}));

            m.Trigger("c");
            Assert.That("closed", Is.EqualTo(m.Current.Name));
            Assert.That(m.Stack.ToArray(), Is.EquivalentTo(new State<string>[] {}));

            m.Trigger("c");
            Assert.That("closed", Is.EqualTo(m.Current.Name));
            Assert.That(m.Stack.ToArray(), Is.EquivalentTo(new State<string>[] {}));

            m.Trigger("o");
            Assert.That("opened", Is.EqualTo(m.Current.Name));
            Assert.That(m.Stack.ToArray(), Is.EquivalentTo(new[] {opened}));
        }
    }
}
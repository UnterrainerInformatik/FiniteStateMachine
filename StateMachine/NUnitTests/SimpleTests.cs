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
using NUnit.Framework;

namespace StateMachine.NUnitTests
{
    [TestFixture]
    [Category("StateMachine.Simple")]
    public class SimpleTests
    {
        [Test]
        [Category("StateMachine.Simple")]
        public void SimpleTest()
        {
            State<string> opened = new State<string>("opened");
            State<string> closed = State<string>.Builder().Name("closed").EndState(true).ClearStack(true).Get();
            opened.Add(new Transition<string>("close", "c", closed)).Add(new Transition<string>("open", "o", opened));
            closed.Add(new Transition<string>("open", "o", opened)).Add(new Transition<string>("close", "c", closed));

            Machine<string> m =
                new Machine<string>(opened).AddStateChangedHandler(ConsoleOut);

            m.Process("o");
            Assert.That("opened", Is.EqualTo(m.Current.Name));
            Assert.That(m.Stack.ToArray(), Is.EquivalentTo(new[] {opened, opened}));

            m.Process("c");
            Assert.That("closed", Is.EqualTo(m.Current.Name));
            Assert.That(m.Stack.ToArray(), Is.EquivalentTo(new State<string>[] {}));

            m.Process("c");
            Assert.That("closed", Is.EqualTo(m.Current.Name));
            Assert.That(m.Stack.ToArray(), Is.EquivalentTo(new State<string>[] {}));

            m.Process("o");
            Assert.That("opened", Is.EqualTo(m.Current.Name));
            Assert.That(m.Stack.ToArray(), Is.EquivalentTo(new[] {opened}));
        }

        private void ConsoleOut(object sender, TransitioningValueArgs<string> e)
        {
            Console.Out.WriteLine($"From [{e.From}] with [{e.Input}] to [{e.To}]");
        }

        [Test]
        [Category("StateMachine.Simple")]
        public void SimpleTestWithPop()
        {
            State<string> opened = new State<string>("opened");
            State<string> closed = State<string>.Builder().Name("closed").EndState(true).ClearStack(true).Get();
            State<string> test = new State<string>("pop");
            opened.Add(new Transition<string>("close", "c", closed))
                .Add(new Transition<string>("open", "o", opened))
                .Add(new Transition<string>("push", "p", test));
            test.Add(Transition<string>.Builder().Name("pop").Trigger("p").Target(opened).Pop(true).Get());
            closed.Add(new Transition<string>("open", "o", opened)).Add(new Transition<string>("close", "c", closed));

            Machine<string> m =
                new Machine<string>(opened).AddStateChangedHandler(ConsoleOut);

            m.Process("o");
            Assert.That("opened", Is.EqualTo(m.Current.Name));
            Assert.That(m.Stack.ToArray(), Is.EquivalentTo(new[] {opened, opened}));

            m.Process("p");
            Assert.That("pop", Is.EqualTo(m.Current.Name));
            Assert.That(m.Stack.ToArray(), Is.EquivalentTo(new[] {opened, opened, test}));

            m.Process("p");
            Assert.That("opened", Is.EqualTo(m.Current.Name));
            Assert.That(m.Stack.ToArray(), Is.EquivalentTo(new[] {opened, opened}));

            m.Process("c");
            Assert.That("closed", Is.EqualTo(m.Current.Name));
            Assert.That(m.Stack.ToArray(), Is.EquivalentTo(new State<string>[] {}));

            m.Process("c");
            Assert.That("closed", Is.EqualTo(m.Current.Name));
            Assert.That(m.Stack.ToArray(), Is.EquivalentTo(new State<string>[] {}));

            m.Process("o");
            Assert.That("opened", Is.EqualTo(m.Current.Name));
            Assert.That(m.Stack.ToArray(), Is.EquivalentTo(new[] {opened}));
        }
    }
}
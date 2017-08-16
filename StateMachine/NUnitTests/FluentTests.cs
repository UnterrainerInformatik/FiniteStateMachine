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
    [Category("FluentTests")]
    public class FluentTests
    {
        [Test]
        [Category("FluentTests")]
        public void BuilderTest()
        {
            Transition<string> t1 = new Transition<string> {Name = "name", Trigger = "trigger"};

            Assert.That(t1.Name, Is.EqualTo("name"));
            Assert.That(t1.Trigger, Is.EqualTo("trigger"));

            t1.Set().Name("n1");
            Assert.That(t1.Name, Is.EqualTo("n1"));

            t1.Set().Name("n2").Get();
            Assert.That(t1.Name, Is.EqualTo("n2"));

            Transition<string> t2 = t1;
            Assert.That(t2.Name, Is.EqualTo("n2"));

            t2 = t1.Set().Name("n3").Get();
            Assert.That(t1.Name, Is.EqualTo("n3"));
            Assert.That(t2.Name, Is.EqualTo("n3"));
        }
    }
}
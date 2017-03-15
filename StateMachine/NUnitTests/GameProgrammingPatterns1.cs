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
    [Category("StateMachine.GamingProgrammingPatterns")]
    public class GameProgrammingPatterns1
    {
        [Test]
        [Category("StateMachine.GamingProgrammingPatterns.1")]
        public void GamingProgrammingPatternsTest1()
        {
            State<string> ducking = new State<string>("ducking");
            State<string> standing = State<string>.Builder().Name("standing").ClearStack(true).Get();
            State<string> jumping = new State<string>("jumping");
            State<string> diving = new State<string>("diving");

            ducking.Add(Transition<string>.Builder().Name("stand_up").Trigger("down").Target(standing).Get());
            standing.Add(Transition<string>.Builder().Name("duck").Trigger("down").Target(ducking).Get())
                .Add(Transition<string>.Builder().Name("jump").Trigger("B").Target(jumping).Get());
            jumping.Add(Transition<string>.Builder().Name("dive").Trigger("down").Target(diving).Get());
            
            Machine<string> m =
                new Machine<string>(standing).AddStateChangedHandler(ConsoleOut);
        }

        [Test]
        [Category("StateMachine.GamingProgrammingPatterns.2")]
        public void GamingProgrammingPatternsTest2()
        {
            State<string> ducking = new State<string>("ducking");
            State<string> standing = State<string>.Builder().Name("standing").ClearStack(true).Get();
            State<string> jumping = new State<string>("jumping");
            State<string> diving = new State<string>("diving");
            ducking.Add(Transition<string>.Builder().Name("stand_up").Trigger("down").Target(standing).Get());
            standing.Add(Transition<string>.Builder().Name("duck").Trigger("down").Target(ducking).Get())
                .Add(Transition<string>.Builder().Name("jump").Trigger("B").Target(jumping).Get());
            jumping.Add(Transition<string>.Builder().Name("dive").Trigger("down").Target(diving).Get());

            Machine<string> m =
                new Machine<string>(standing).AddStateChangedHandler(ConsoleOut);

            // Now for the weapons-machine with basic forward- and backward-rotation.
            State<string> emptyHanded = new State<string>("empty_handed");
            State<string> gun = new State<string>("gun");
            State<string> shotgun = new State<string>("shotgun");
            State<string> laserRifle = new State<string>("laser_rifle");
            emptyHanded.Add(Transition<string>.Builder().Name("rotate_weapon").Trigger("tab").Target(gun).Get())
                .Add(
                    Transition<string>.Builder()
                        .Name("rotate_weapon_back")
                        .Trigger("shift-tab")
                        .Target(laserRifle)
                        .Get());
            gun.Add(Transition<string>.Builder().Name("rotate_weapon").Trigger("tab").Target(shotgun).Get())
                .Add(
                    Transition<string>.Builder()
                        .Name("rotate_weapon_back")
                        .Trigger("shift-tab")
                        .Target(emptyHanded)
                        .Get());
            shotgun.Add(Transition<string>.Builder().Name("rotate_weapon").Trigger("tab").Target(laserRifle).Get())
                .Add(Transition<string>.Builder().Name("rotate_weapon_back").Trigger("shift-tab").Target(gun).Get());
            laserRifle.Add(Transition<string>.Builder().Name("rotate_weapon").Trigger("tab").Target(emptyHanded).Get())
                .Add(
                    Transition<string>.Builder().Name("rotate_weapon_back").Trigger("shift-tab").Target(shotgun).Get());

            Machine<string> w =
                new Machine<string>(emptyHanded).AddStateChangedHandler(ConsoleOut);
        }

        private void ConsoleOut(object sender, TransitioningValueArgs<string> e)
        {
            Console.Out.WriteLine($"From [{e.From}] with [{e.Input}] to [{e.To}]");
        }
    }
}
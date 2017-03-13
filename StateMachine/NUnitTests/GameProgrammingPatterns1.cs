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
            State<string> standing = new State<string>("standing").SetClearStack(true);
            State<string> jumping = new State<string>("jumping");
            State<string> diving = new State<string>("diving");
            ducking.Add(new Transition<string>("stand_up", "down", standing));
            standing.Add(new Transition<string>("duck", "down", ducking))
                .Add(new Transition<string>("jump", "B", jumping));
            jumping.Add(new Transition<string>("dive", "down", diving));

            Machine<string> m =
                new Machine<string>(standing).Add(new[] {standing, ducking, jumping, diving})
                    .AddStateChangedHandler(ConsoleOut);
        }

        [Test]
        [Category("StateMachine.GamingProgrammingPatterns.2")]
        public void GamingProgrammingPatternsTest2()
        {
            State<string> ducking = new State<string>("ducking");
            State<string> standing = new State<string>("standing").SetClearStack(true);
            State<string> jumping = new State<string>("jumping");
            State<string> diving = new State<string>("diving");
            ducking.Add(new Transition<string>("stand_up", "down", standing));
            standing.Add(new Transition<string>("duck", "down", ducking));
            standing.Add(new Transition<string>("jump", "B", jumping));
            jumping.Add(new Transition<string>("dive", "down", diving));

            Machine<string> m =
                new Machine<string>(standing).Add(new[] {standing, ducking, jumping, diving})
                    .AddStateChangedHandler(ConsoleOut);

            // Now for the weapons-machine with basic forward- and backward-rotation.
            State<string> emptyHanded = new State<string>("empty_handed");
            State<string> gun = new State<string>("gun");
            State<string> shotgun = new State<string>("shotgun");
            State<string> laserRifle = new State<string>("laser_rifle");
            emptyHanded.Add(new Transition<string>("rotate_weapon", "tab", gun))
                .Add(new Transition<string>("rotate_weapon_back", "shift-tab", laserRifle));
            gun.Add(new Transition<string>("rotate_weapon", "tab", shotgun))
                .Add(new Transition<string>("rotate_weapon_back", "shift-tab", emptyHanded));
            shotgun.Add(new Transition<string>("rotate_weapon", "tab", laserRifle))
                .Add(new Transition<string>("rotate_weapon_back", "shift-tab", gun));
            laserRifle.Add(new Transition<string>("rotate_weapon", "tab", emptyHanded))
                .Add(new Transition<string>("rotate_weapon_back", "shift-tab", shotgun));

            Machine<string> w =
                new Machine<string>(emptyHanded).Add(new[] {emptyHanded, gun, shotgun, laserRifle})
                    .AddStateChangedHandler(ConsoleOut);
        }

        private void ConsoleOut(object sender, TransitioningValueArgs<string> e)
        {
            Console.Out.WriteLine($"From [{e.From}] with [{e.Input}] to [{e.To}]");
        }
    }
}
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
    [Category("StateMachine.GamingProgrammingPatterns")]
    public class GameProgrammingPatterns1
    {
        private enum State
        {
            DUCKING,
            STANDING,
            JUMPING,
            DIVING
        }

        private enum Trigger
        {
            DOWN,
            UP
        }
        
        [Test]
        [Category("StateMachine.GamingProgrammingPatterns.1")]
        public void GamingProgrammingPatternsTest1()
        {
            Fsm<State, Trigger, float>.Builder(State.STANDING)
                .State(State.DUCKING)
                    .TransitionTo(State.STANDING).On(Trigger.DOWN)
                    .TransitionTo(State.STANDING).On(Trigger.UP)
                .State(State.STANDING).ClearsStack()
                    .TransitionTo(State.DUCKING).On(Trigger.DOWN)
                    .TransitionTo(State.JUMPING).On(Trigger.UP)
                .State(State.JUMPING)
                    .TransitionTo(State.DIVING).On(Trigger.DOWN)
                .State(State.DIVING)
                .Build();
        }

        private enum WState
        {
            EMPTY_HANDED,
            GUN,
            SHOTGUN,
            LASER_RIFLE
        }

        private enum WTrigger
        {
            TAB,
            SHIFT_TAB
        }

        [Test]
        [Category("StateMachine.GamingProgrammingPatterns.2")]
        public void GamingProgrammingPatternsTest2()
        {
            // Now for the weapons-machine with basic forward- and backward-rotation.
            Fsm<WState, WTrigger, float>.Builder(WState.EMPTY_HANDED)
                .State(WState.EMPTY_HANDED)
                    .TransitionTo(WState.GUN).On(WTrigger.TAB)
                    .TransitionTo(WState.LASER_RIFLE).On(WTrigger.SHIFT_TAB)
                .State(WState.GUN)
                    .TransitionTo(WState.SHOTGUN).On(WTrigger.TAB)
                    .TransitionTo(WState.EMPTY_HANDED).On(WTrigger.SHIFT_TAB)
                .State(WState.SHOTGUN)
                    .TransitionTo(WState.LASER_RIFLE).On(WTrigger.TAB)
                    .TransitionTo(WState.GUN).On(WTrigger.SHIFT_TAB)
                .State(WState.LASER_RIFLE)
                    .TransitionTo(WState.EMPTY_HANDED).On(WTrigger.TAB)
                    .TransitionTo(WState.SHOTGUN).On(WTrigger.SHIFT_TAB)
                .Build();
        }
    }
}
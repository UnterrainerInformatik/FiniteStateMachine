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
using StateMachine;

namespace NUnitTests
{
	public class Examples
	{
		private enum VState
		{
			DUCKING,
			STANDING,
			JUMPING,
			DESCENDING,
			DIVING
		};

		private enum VTrigger
		{
			DOWN_RELEASED,
			DOWN_PRESSED,
			UP_PRESSED,
			SPACE_PRESSED
		};

		private enum HState
		{
			STANDING,
			RUNNING_LEFT,
			RUNNING_RIGHT,
			WALKING_LEFT,
			WALKING_RIGHT,
			WALKING_DELAY_LEFT,
			WALKING_DELAY_RIGHT
		};

		private enum HTrigger
		{
			LEFT_PRESSED,
			LEFT_RELEASED,
			RIGHT_PRESSED,
			RIGHT_RELEASED,
			SPACE_PRESSED
		};

		private Fsm<VState, VTrigger> verticalMachine;
		private Fsm<HState, HTrigger> horizontalMachine;

		private Hero hero;

		[Test]
		[Ignore("Is output to PlantUML only.")]
		public void HorizontalMachine()
		{
			horizontalMachine = Fsm<HState, HTrigger>.Builder(HState.STANDING)
				.State(HState.STANDING)
				.TransitionTo(HState.WALKING_LEFT)
				.On(HTrigger.LEFT_PRESSED)
				.TransitionTo(HState.WALKING_RIGHT)
				.On(HTrigger.RIGHT_PRESSED)
				.State(HState.WALKING_LEFT)
				.TransitionTo(HState.WALKING_DELAY_LEFT)
				.On(HTrigger.LEFT_RELEASED)
				.State(HState.WALKING_RIGHT)
				.TransitionTo(HState.WALKING_DELAY_RIGHT)
				.On(HTrigger.RIGHT_RELEASED)
				.State(HState.WALKING_DELAY_LEFT)
				.TransitionTo(HState.WALKING_RIGHT)
				.On(HTrigger.RIGHT_PRESSED)
				.TransitionTo(HState.RUNNING_LEFT)
				.On(HTrigger.LEFT_PRESSED)
				.State(HState.WALKING_DELAY_RIGHT)
				.TransitionTo(HState.WALKING_LEFT)
				.On(HTrigger.LEFT_PRESSED)
				.TransitionTo(HState.RUNNING_RIGHT)
				.On(HTrigger.RIGHT_PRESSED)
				.State(HState.RUNNING_LEFT)
				.TransitionTo(HState.STANDING)
				.On(HTrigger.LEFT_RELEASED)
				.State(HState.RUNNING_RIGHT)
				.TransitionTo(HState.STANDING)
				.On(HTrigger.RIGHT_RELEASED)
				.GlobalTransitionTo(HState.STANDING)
				.OnGlobal(HTrigger.SPACE_PRESSED)
				.Build();

			string s = horizontalMachine.GetPlantUml();
		}

		[Test]
		[Ignore("Is output to PlantUML only.")]
		public void VerticalMachine()
		{
			verticalMachine = Fsm<VState, VTrigger>.Builder(VState.STANDING)
				.State(VState.STANDING)
				.TransitionTo(VState.DUCKING)
				.On(VTrigger.DOWN_PRESSED)
				.TransitionTo(VState.JUMPING)
				.On(VTrigger.UP_PRESSED)
				.State(VState.DUCKING)
				.TransitionTo(VState.STANDING)
				.On(VTrigger.DOWN_RELEASED)
				.State(VState.JUMPING)
				.TransitionTo(VState.DIVING)
				.On(VTrigger.DOWN_PRESSED)
				.State(VState.DESCENDING)
				.TransitionTo(VState.DIVING)
				.On(VTrigger.DOWN_PRESSED)
				.State(VState.DIVING)
				.TransitionTo(VState.DESCENDING)
				.On(VTrigger.DOWN_RELEASED)
				.GlobalTransitionTo(VState.STANDING)
				.OnGlobal(VTrigger.SPACE_PRESSED)
				.Build();

			string s = verticalMachine.GetPlantUml();
		}
	}
}
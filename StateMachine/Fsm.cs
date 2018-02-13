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
using System.Collections.Generic;
using System.Text;
using StateMachine.Events;
using StateMachine.Fluent.Api;

namespace StateMachine
{
	public class Fsm<TS, TT>
	{
		private FsmModel<TS, TT> Model { get; } = new FsmModel<TS, TT>();

		public State<TS, TT> Current => Model.Current;
		public State<TS, TT> StartState => Model.StartState;
		public Stack<State<TS, TT>> Stack => Model.Stack;

		public Dictionary<Tuple<TS, TS>, List<Timer<TS>>> AfterEntries { get; set; } =
			new Dictionary<Tuple<TS, TS>, List<Timer<TS>>>();

		public List<Timer<TS>> GlobalAfterEntries { get; set; } = new List<Timer<TS>>();

		/// <exception cref="FsmBuilderException">When the model is null</exception>
		public Fsm(FsmModel<TS, TT> model)
		{
			Model = model ?? throw FsmBuilderException.ModelCannotBeNull();
			if (Model.StackEnabled && !model.Current.ClearStack)
			{
				Model.Stack.Push(model.Current);
			}
		}

		/// <exception cref="FsmBuilderException">When the initial state is null</exception>
		public Fsm(State<TS, TT> current, bool stackEnabled = false)
		{
			Model.StackEnabled = stackEnabled;
			Model.Current = current ?? throw FsmBuilderException.StateCannotBeNull();
			if (Model.StackEnabled && !current.ClearStack)
			{
				Model.Stack.Push(current);
			}
		}

		/// <summary>
		///     Gets you a builder for a Finite-State-Machine (FSM).
		/// </summary>
		/// <param name="startState">The start state's key.</param>
		/// <returns></returns>
		public static BuilderFluent<TS, TT> Builder(TS startState)
			=> new FluentImplementation<TS, TT>(startState);

		/// <exception cref="FsmBuilderException">When the handler is null</exception>
		public Fsm<TS, TT> AddStateChangeHandler(
			EventHandler<StateChangeArgs<TS, TT>> e)
		{
			if (e == null) throw FsmBuilderException.HandlerCannotBeNull();
			Model.StateChanged += e;
			return this;
		}

		/// <exception cref="FsmBuilderException">When the state is null or the state has already been added before</exception>
		public Fsm<TS, TT> Add(State<TS, TT> state)
		{
			if (state == null) throw FsmBuilderException.StateCannotBeNull();
			if (Model.States.ContainsKey(state.Identifier)) throw FsmBuilderException.StateCanOnlyBeAddedOnce(state);

			Model.States.Add(state.Identifier, state);
			return this;
		}

		/// <exception cref="FsmBuilderException">
		///     When the transition is null or another transition already leads to the same
		///     target state
		/// </exception>
		public Fsm<TS, TT> Add(Transition<TS, TT> t)
		{
			if (t == null) throw FsmBuilderException.TransitionCannotBeNull();
			Model.GlobalTransitions.Add(t.Target, t);
			return this;
		}

		public void JumpTo(TS state, bool isPop = false)
		{
			State<TS, TT> s;
			if (Model.States.TryGetValue(state, out s))
			{
				DoTransition(state, default(TT), isPop);
			}
		}

		private void DoTransition(TS state, TT input, bool isPop)
		{
			if (state == null || input == null) return;

			var old = Model.Current;
			if (Model.StackEnabled && isPop)
			{
				Model.Stack.Pop();
				Model.Current = Model.Stack.Peek();
			}
			else
			{
				Model.Current = Model.States[state];
				if (Model.StackEnabled)
					Model.Stack.Push(Model.Current);
			}

			if (Model.StackEnabled && Model.Current.ClearStack)
			{
				Model.Stack.Clear();
			}

			if (Model.Current.Equals(old)) return;

			var args =
				new StateChangeArgs<TS, TT>(this, old, Model.Current, input);
			Exited(old, args);
			Entered(args);
			StateChanged(args);
		}

		private void Entered(StateChangeArgs<TS, TT> args)
		{
			ResetCurrentAfterEntries();
			Model.Current.RaiseEntered(args);
		}

		private void Exited(State<TS, TT> old, StateChangeArgs<TS, TT> args)
		{
			old.RaiseExited(args);
		}

		private void StateChanged(StateChangeArgs<TS, TT> args)
		{
			Model.RaiseStateChanged(args);
		}

		public void Trigger(TT input)
		{
			if (input == null) return;

			foreach (var g in Model.GlobalTransitions.Values)
			{
				if (!g.Process(Model.Current, input)) continue;

				DoTransition(g.Target, input, g.Pop);
				return;
			}

			var t = Model.Current.Process(input);
			if (t != null)
			{
				DoTransition(t.Target, input, t.Pop);
			}
		}

		public void Update(TimeSpan elapsedTime)
		{
			// After-entries on transitions.
			foreach (var k in Current.Model.Transitions.Keys)
			{
				if (!AfterEntries.TryGetValue(new Tuple<TS, TS>(Current.Identifier, k), out var currentAfterEntries))
					continue;
				if (CheckAfterEntries(currentAfterEntries, Current.Model.Transitions, elapsedTime))
					return;
			}

			// Global after-entries.
			if (CheckAfterEntries(GlobalAfterEntries, Model.GlobalTransitions, elapsedTime))
				return;

			Model.Current.RaiseUpdated(new UpdateArgs<TS, TT>(this, Current, elapsedTime));
		}

		private bool CheckAfterEntries(List<Timer<TS>> afterEntries,
			Dictionary<TS, Transition<TS, TT>> transitions, TimeSpan timeSpan)
		{
			for (var i = 0; i < afterEntries.Count; i++)
			{
				var e = afterEntries[i];
				if (!transitions.TryGetValue(e.Target, out var t)) continue;
				if (!t.ConditionsMet(Current.Identifier)) continue;

				var timerMax = e.Time;
				var r = e.Tick(timeSpan.TotalMilliseconds);
				afterEntries[i] = e;

				if (!r.HasValue) continue;

				// It triggered.
				DoTransition(e.Target, default(TT), t.Pop);
				Update(timeSpan.Subtract(TimeSpan.FromMilliseconds(timerMax)));
				return true;
			}

			return false;
		}

		private void ResetCurrentAfterEntries()
		{
			foreach (var k in Current.Model.Transitions.Keys)
			{
				if (!AfterEntries.TryGetValue(new Tuple<TS, TS>(Current.Identifier, k), out var currentAfterEntries)) continue;
				for (var i = 0; i < currentAfterEntries.Count; i++)
				{
					var e = currentAfterEntries[i];
					e.Reset();
					currentAfterEntries[i] = e;
				}
			}
		}

		public string Normalize(string str)
		{
			StringBuilder sb = new StringBuilder();
			foreach (char c in str)
			{
				if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_' || c == '-')
				{
					sb.Append(c);
				}
			}

			return sb.ToString();
		}

		public String GetPlantUml()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("@startuml\n");
			sb.Append($"title {Normalize(GetType().Name)}\n");
			sb.Append("left to right direction\n\n");
			
			WriteStates(sb);
			WriteTransitions(sb);
			WriteStartState(sb);
			WriteEndStates(sb);

			sb.Append("@enduml\n");

			return sb.ToString();
		}

		private void WriteStates(StringBuilder sb)
		{
			foreach (var state in Model.States)
			{
				sb.Append($"state \"{state.Key}\" as {Normalize(state.Key.ToString())}\n");
			}
		}

		private void WriteTransitions(StringBuilder sb)
		{
			foreach (var state in Model.States)
			{
				foreach (var transition in state.Value.Transitions)
				{
					foreach (var trigger in transition.Value.Triggers)
					{
						sb.Append(
							$"{Normalize(transition.Value.Source.ToString())} --> " +
							$"{Normalize(transition.Value.Target.ToString())} : " +
							$"\"{Normalize(trigger.ToString())}\"\n");
					}
				}
			}
		}

		private void WriteStartState(StringBuilder sb)
		{
			sb.Append($"[*] --> {Normalize(StartState.ToString())}\n");
		}

		private void WriteEndStates(StringBuilder sb)
		{
			foreach (var state in Model.States)
			{
				if (state.Value.EndState)
					sb.Append($"{Normalize(state.Key.ToString())} --> [*]\n");
			}
		}
	}
}
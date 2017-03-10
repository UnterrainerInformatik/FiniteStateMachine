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
using JetBrains.Annotations;

namespace StateMachine
{
    [PublicAPI]
    public class State<T>
    {
        public string Name { get; set; }
        public bool EndState { get; set; }
        public bool ClearStack { get; set; }

        private List<Transition<T>> Transitions { get; } = new List<Transition<T>>();


        public State(string name, bool endState = false, bool clearStack = false)
        {
            Name = name;
            EndState = endState;
            ClearStack = clearStack;
        }

        public State<T> Add(Transition<T> t)
        {
            Transitions.Add(t);
            return this;
        }

        public State<T> Remove(Transition<T> t)
        {
            Transitions.Remove(t);
            return this;
        }

        public State<T> Clear()
        {
            Transitions.Clear();
            return this;
        }

        public Transition<T> Process(T input)
        {
            foreach (var t in Transitions)
            {
                if (t.Trigger.Equals(input))
                {
                    return t;
                }
            }
            return null;
        }

        public void Enter(object o)
        {
            Console.Out.WriteLine(AddParametersTo("entering", o));
        }

        public void Exit(object o)
        {
            Console.Out.WriteLine(AddParametersTo("leaving", o));
        }

        private string AddParametersTo(string s, object o)
        {
            return s + " [" + Name + "] with [" + o + "]";
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
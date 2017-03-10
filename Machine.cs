﻿// *************************************************************************** 
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

using System.Collections.Generic;
using JetBrains.Annotations;

namespace StateMachine
{
    [PublicAPI]
    public class Machine<T>
    {
        public State<T> Current { get; set; }
        public Stack<State<T>> Stack { get; } = new Stack<State<T>>();

        private List<State<T>> States { get; } = new List<State<T>>();

        public Machine(State<T> current)
        {
            Current = current;
            if (!current.ClearStack)
            {
                Stack.Push(current);
            }
        }

        public Machine<T> Add(State<T> s)
        {
            States.Add(s);
            return this;
        }

        public Machine<T> Remove(State<T> s)
        {
            States.Remove(s);
            return this;
        }

        public Machine<T> Clear()
        {
            States.Clear();
            return this;
        }

        public void Process(T input)
        {
            State<T> old = Current;
            Transition<T> t = Current.Process(input);

            if (t.Pop)
            {
                Stack.Pop();
                Current = Stack.Peek();
            }
            else
            {
                Current = t.Target;
                Stack.Push(Current);
            }

            if (Current.ClearStack)
            {
                Stack.Clear();
            }

            if (!Current.Equals(old))
            {
                old.Exit(input);
                Current.Enter(input);
            }
        }
    }
}
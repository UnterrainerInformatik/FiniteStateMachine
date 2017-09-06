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

using JetBrains.Annotations;

namespace MonoGameStateMachine
{
    [PublicAPI]
    public struct Timer<TS>
    {
        public double Value { get; set; }
        public TimeUnit Unit { get; set; }
        public double Time { get; set; }
        public TS Target { get; set; }

        public Timer(TS target, double value, TimeUnit unit)
        {
            Target = target;
            Value = value;
            Unit = unit;
            Time = 0;
            Reset();
        }

        private void Reset()
        {
            Time = Value * (long) Unit;
        }

        /// <summary>
        ///     Lets the specified time tick away and subtracts it from the Timer.<br />
        ///     If the timer triggered the time that was left after the Timer triggered is returned.
        /// </summary>
        /// <param name="timeInMillis">The time to tick away.</param>
        /// <returns>Null if the timer didn't trigger, a positive value otherwise.</returns>
        public double? Tick(double timeInMillis)
        {
            Time -= timeInMillis;
            if (Time <= 0D)
            {
                double d = Time * -1D;
                Reset();
                return d;
            }
            return null;
        }
    }
}
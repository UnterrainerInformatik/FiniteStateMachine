using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StateMachine.Fluent.Api
{
    interface TransitionStateFluent<STATE, TRIGGER> : TransitionFluent<STATE, TRIGGER>, StateFluent<STATE, TRIGGER>
    {
    }
}

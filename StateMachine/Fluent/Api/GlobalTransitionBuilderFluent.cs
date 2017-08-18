using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StateMachine.Fluent.Api
{
    interface GlobalTransitionBuilderFluent<STATE, TRIGGER> : GlobalTransitionFluent<STATE, TRIGGER>, BuilderFluent<STATE, TRIGGER>
    {
    }
}

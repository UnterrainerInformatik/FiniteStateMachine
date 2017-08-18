using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StateMachine.Fluent.Api
{
    interface StateFluent<STATE, TRIGGER> : BuilderFluent<STATE, TRIGGER>
    {
        TransitionFluent<STATE, TRIGGER> TransitionTo(STATE state);

        StateFluent<STATE, TRIGGER> OnEnter(Func<STATE, TRIGGER, StateFluent<STATE, TRIGGER>> enter);

        StateFluent<STATE, TRIGGER> OnExit(Func<STATE, TRIGGER, StateFluent<STATE, TRIGGER>> exit);

        StateFluent<STATE, TRIGGER> Update(Func<STATE, float, StateFluent<STATE, TRIGGER>> update);
    }
}

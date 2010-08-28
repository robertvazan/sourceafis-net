using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.Meta;

namespace SourceAFIS.Tuning.Optimization
{
    public sealed class AxisFocusMutation : MutationAdvisor
    {
        protected override MutationMemory Remember(ParameterSet initialExample, ParameterSet mutatedExample)
        {
            string focusPath = mutatedExample.GetDifference(initialExample).FieldPath;

            MutationMemory memory = new MutationMemory();
            memory.Lifetime = memory.ResetLifetime = 10;
            memory.Mutate = delegate(ParameterSet initial)
            {
                ParameterSet mutated = initial.Clone();
                ParameterValue parameter = mutated.Get(focusPath);
                Mutate(parameter);
                return mutated;
            };
            memory.IsRelevant = (initial, mutated) => mutated.GetDifference(initial).FieldPath == focusPath;
            return memory;
        }
    }
}

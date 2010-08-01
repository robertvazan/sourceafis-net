using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.Meta;

namespace SourceAFIS.Tuning.Optimization
{
    public sealed class ManualMutation : MutationAdvisor
    {
        public List<string> ParameterPaths = new List<string>();

        protected override IEnumerable<MutationMemory> InitializeMemory()
        {
            foreach (string parameterPath in ParameterPaths)
            {
                string parameterPathCopy = parameterPath;
                MutationMemory memory = new MutationMemory();
                memory.Lifetime = memory.ResetLifetime = 20;
                memory.Mutate = delegate(ParameterSet initial)
                {
                    ParameterSet mutated = initial.Clone();
                    ParameterValue parameter = mutated.Get(parameterPathCopy);
                    Mutate(parameter);
                    return mutated;
                };
                memory.IsRelevant = (initial, mutated) => mutated.GetDifference(initial).FieldPath == parameterPathCopy;
                yield return memory;
            }
        }
    }
}

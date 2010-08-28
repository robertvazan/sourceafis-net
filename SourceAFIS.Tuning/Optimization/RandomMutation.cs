using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.Meta;

namespace SourceAFIS.Tuning.Optimization
{
    public sealed class RandomMutation : MutationAdvisor
    {
        protected override IEnumerable<MutationMemory> InitializeMemory() { yield return CreateMemory(); }

        protected override MutationMemory Remember(ParameterSet initial, ParameterSet mutated) { return CreateMemory(); }

        MutationMemory CreateMemory()
        {
            MutationMemory memory = new MutationMemory();
            memory.Mutate = Mutate;
            return memory;
        }

        ParameterSet Mutate(ParameterSet initial)
        {
            ParameterSet mutated = initial.Clone();
            ParameterValue parameter = PickParameter(mutated);
            Mutate(parameter);
            return mutated;
        }

        ParameterValue PickParameter(ParameterSet parameters)
        {
            ParameterValue[] all = parameters.AllParameters;
            return all[Random.Next(all.Length)];
        }
    }
}

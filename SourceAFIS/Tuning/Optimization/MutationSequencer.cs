using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.Meta;

namespace SourceAFIS.Tuning.Optimization
{
    public sealed class MutationSequencer
    {
        Random Random = new Random();

        public delegate void MutationEvent(ParameterValue initial, ParameterValue mutated);
        public MutationEvent OnMutation;

        public ParameterSet Mutate(ParameterSet initial)
        {
            ParameterSet mutated = initial.Clone();
            
            ParameterValue[] all = mutated.AllParameters;
            ParameterValue parameter = all[Random.Next(all.Length)];
            ParameterValue savedParameter = parameter.Clone();
            Mutate(parameter);

            if (OnMutation != null)
                OnMutation(savedParameter, parameter);

            return mutated;
        }

        void Mutate(ParameterValue parameter)
        {
            int value = parameter.Value.Normalized;
            int upper = parameter.Upper.Normalized;
            int lower = parameter.Lower.Normalized;

            bool negative = (Random.Next(2) == 1);
            if (!negative && value >= upper)
                negative = true;
            if (negative && value <= lower)
                negative = false;

            int range;
            if (!negative)
                range = upper - value;
            else
                range = value - lower;
            double change = Math.Pow(range, Random.NextDouble());
            int intChange = Convert.ToInt32(change);

            int newvalue;
            if (!negative)
                newvalue = value + intChange;
            else
                newvalue = value - intChange;
            if (newvalue > upper)
                newvalue = upper;
            if (newvalue < lower)
                newvalue = lower;
            
            parameter.Value.Normalized = newvalue;
        }
    }
}

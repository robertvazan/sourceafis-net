using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.Meta;
using SourceAFIS.General;

namespace SourceAFIS.Tuning.Optimization
{
    public sealed class MutationSequencer
    {
        public float ExtractorWeight = 0.2f;

        public delegate void MutationEvent(ParameterValue initial, ParameterValue mutated);
        public MutationEvent OnMutation;

        Random Random = new Random();

        public ParameterSet Mutate(ParameterSet initial)
        {
            ParameterSet mutated = initial.Clone();
            
            ParameterValue parameter = PickParameter(mutated);
            ParameterValue savedParameter = parameter.Clone();
            Mutate(parameter);

            if (OnMutation != null)
                OnMutation(savedParameter, parameter);

            return mutated;
        }

        ParameterValue PickParameter(ParameterSet parameters)
        {
            ParameterValue[] all = parameters.AllParameters;
            
            float[] weights = new float[all.Length];
            for (int i = 0; i < all.Length; ++i)
                if (Calc.BeginsWith(all[i].FieldPath, "Extractor."))
                    weights[i] = ExtractorWeight;
                else
                    weights[i] = 1;

            float totalWeight = 0;
            foreach (float weight in weights)
                totalWeight += weight;
            
            float randomWeight = (float)(Random.NextDouble() * totalWeight);
            for (int i = 0; i < all.Length; ++i)
            {
                randomWeight -= weights[i];
                if (randomWeight < 0)
                    return all[i];
            }
            return all[all.Length - 1];
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

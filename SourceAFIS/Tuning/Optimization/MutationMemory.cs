using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.Meta;

namespace SourceAFIS.Tuning.Optimization
{
    public sealed class MutationMemory
    {
        public float Confidence = 1;
        public int Lifetime = 1;
        public int ResetLifetime = 1;
        public bool IsExpired { get { return Lifetime <= 0; } }

        public delegate ParameterSet MutationDelegate(ParameterSet initial);
        public MutationDelegate Mutate = initial => null;
        public delegate bool RelevancyDelegate(ParameterSet initial, ParameterSet mutated);
        public RelevancyDelegate IsRelevant = (initial, mutated) => true;

        public void Feedback(bool improved)
        {
            if (improved)
                Lifetime = ResetLifetime;
            else
                --Lifetime;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.Meta;
using SourceAFIS.General;

namespace SourceAFIS.Tuning.Optimization
{
    public abstract class MutationAdvisor
    {
        protected abstract MutationMemory Remember(ParameterSet initial, ParameterSet mutated);
        protected virtual IEnumerable<MutationMemory> InitializeMemory() { yield break; }

        protected Random Random = new Random();
        List<MutationMemory> Live = new List<MutationMemory>();
        bool Initialized;

        public IEnumerable<MutationAdvice> Advise(ParameterSet initial)
        {
            Initialize();
            PurgeExpired();

            foreach (MutationMemory memory in Live)
            {
                MutationAdvice advice = new MutationAdvice();
                advice.Mutated = memory.Mutate(initial);
                if (advice.Mutated != null)
                {
                    advice.Initial = initial;
                    advice.Confidence = memory.Confidence;
                    yield return advice;
                }
            }
        }

        public void Feedback(ParameterSet initial, ParameterSet mutated, bool improved)
        {
            foreach (MutationMemory memory in Live)
                if (memory.IsRelevant(initial, mutated))
                {
                    memory.Feedback(improved);
                    return;
                }

            if (improved)
            {
                MutationMemory added = Remember(initial, mutated);
                if (added != null)
                    Live.Add(added);
            }
        }

        void Initialize()
        {
            if (!Initialized)
            {
                Live.AddRange(InitializeMemory());
                Initialized = true;
            }
        }

        void PurgeExpired()
        {
            List<MutationMemory> purged = new List<MutationMemory>();
            foreach (MutationMemory memory in Live)
                if (memory.IsExpired)
                    purged.Add(memory);
            if (purged.Count > 0)
                Live.RemoveAll(delegate(MutationMemory memory) { return purged.Contains(memory); });
        }

        protected void Mutate(ParameterValue parameter)
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

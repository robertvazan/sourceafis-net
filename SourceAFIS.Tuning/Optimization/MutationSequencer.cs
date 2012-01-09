using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using SourceAFIS.Meta;
using SourceAFIS.General;

namespace SourceAFIS.Tuning.Optimization
{
    public sealed class MutationSequencer
    {
        public int MultipleAdvices = 10;
        public float ExtractorWeight = 0.2f;

        public delegate void MutationEvent(ParameterValue initial, ParameterValue mutated);
        public event MutationEvent OnMutation;

        Random Random = new Random();

        public ManualMutation ManualAdvisor = new ManualMutation();
        public AxisFocusMutation AxisFocusAdvisor = new AxisFocusMutation();
        public RandomMutation RandomAdvisor = new RandomMutation();
        
        public IEnumerable<MutationAdvisor> Advisors
        {
            get
            {
                yield return ManualAdvisor;
                yield return AxisFocusAdvisor;
                yield return RandomAdvisor;
            }
        }

        public ParameterSet Mutate(ParameterSet initial)
        {
            var advices = (from advisor in Advisors
                           from repeat in Enumerable.Range(0, MultipleAdvices)
                           from advice in advisor.Advise(initial)
                           select advice).ToList();
            
            AdjustExtractorWeight(advices);

            ParameterSet mutated = PickAdvice(advices).Mutated;

            string mutatedPath = mutated.GetDifference(initial).FieldPath;
            if (OnMutation != null)
                OnMutation(initial.Get(mutatedPath), mutated.Get(mutatedPath));

            return mutated;
        }

        public void Feedback(ParameterSet initial, ParameterSet mutated, bool improved)
        {
            foreach (MutationAdvisor advisor in Advisors)
                advisor.Feedback(initial, mutated, improved);
        }

        void AdjustExtractorWeight(List<MutationAdvice> advices)
        {
            foreach (MutationAdvice advice in advices)
            {
                ParameterValue parameter = advice.Mutated.GetDifference(advice.Initial);
                if (parameter.FieldPath.BeginsWith("Extractor."))
                    advice.Confidence *= ExtractorWeight;
            }
        }

        MutationAdvice PickAdvice(List<MutationAdvice> advices)
        {
            float confidenceSum = advices.Sum(advice => advice.Confidence);

            float randomWeight = (float)(Random.NextDouble() * confidenceSum);
            for (int i = 0; i < advices.Count; ++i)
            {
                randomWeight -= advices[i].Confidence;
                if (randomWeight < 0)
                    return advices[i];
            }
            return advices[advices.Count - 1];
        }
    }
}

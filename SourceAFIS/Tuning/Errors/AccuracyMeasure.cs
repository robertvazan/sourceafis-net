using System;
using System.Collections.Generic;
using System.Text;

namespace SourceAFIS.Tuning.Errors
{
    public sealed class AccuracyMeasure
    {
        public string Name = "AccuracyMeasure";
        public MultiFingerPolicy MultiFingerPolicy = MultiFingerPolicy.Single;
        public ErrorPolicy.Evaluate ErrorPolicyFunction = ErrorPolicy.EER;
        public ScalarErrorMeasure ScalarMeasure = ScalarErrorMeasure.Average;
        public SeparationMeasure Separation = SeparationMeasure.HalfDeviation;

        public static readonly List<AccuracyMeasure> AccuracyLandscape = GenerateDefault();

        static List<AccuracyMeasure> GenerateDefault()
        {
            List<AccuracyMeasure> all = new List<AccuracyMeasure>();
            AccuracyMeasure measure;

            measure = new AccuracyMeasure();
            measure.Name = "Standard";
            all.Add(measure);

            measure = new AccuracyMeasure();
            measure.Name = "PreferFAR";
            measure.ErrorPolicyFunction = ErrorPolicy.PreferFAR;
            all.Add(measure);

            measure = new AccuracyMeasure();
            measure.Name = "ZeroFAR";
            measure.ErrorPolicyFunction = ErrorPolicy.ZeroFAR;
            all.Add(measure);

            measure = new AccuracyMeasure();
            measure.Name = "FAR100";
            measure.ErrorPolicyFunction = ErrorPolicy.FAR100;
            all.Add(measure);

            measure = new AccuracyMeasure();
            measure.Name = "1-of-2";
            measure.MultiFingerPolicy = MultiFingerPolicy.Take1Of2;
            all.Add(measure);

            measure = new AccuracyMeasure();
            measure.Name = "2-of-3";
            measure.MultiFingerPolicy = MultiFingerPolicy.Take2Of3;
            all.Add(measure);

            measure = new AccuracyMeasure();
            measure.Name = "2-of-4";
            measure.MultiFingerPolicy = MultiFingerPolicy.Take2Of4;
            all.Add(measure);

            measure = new AccuracyMeasure();
            measure.Name = "3-of-5";
            measure.MultiFingerPolicy = MultiFingerPolicy.Take3Of5;
            all.Add(measure);

            measure = new AccuracyMeasure();
            measure.Name = "MinMax";
            measure.Separation = SeparationMeasure.MinMax;
            all.Add(measure);

            measure = new AccuracyMeasure();
            measure.Name = "StandardDeviation";
            measure.Separation = SeparationMeasure.StandardDeviation;
            all.Add(measure);

            measure = new AccuracyMeasure();
            measure.Name = "HalfDistance";
            measure.Separation = SeparationMeasure.HalfDistance;
            all.Add(measure);

            return all;
        }
    }
}

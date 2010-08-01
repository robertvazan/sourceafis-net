using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.General;

namespace SourceAFIS.Tuning.Errors
{
    public sealed class ErrorRange
    {
        public ROCPoint Rate;
        public ROCPoint Min;
        public ROCPoint Max;

        public void Compute(ROCCurve ROC, ErrorPolicy.Evaluate policy)
        {
            FindMinMax(ROC, policy, out Min, out Max);
            
            float fraction = FindZeroFraction(policy(Min), policy(Max));
            Rate.FAR = Calc.Interpolate(Min.FAR, Max.FAR, fraction);
            Rate.FRR = Calc.Interpolate(Min.FRR, Max.FRR, fraction);
            Rate.Threshold = Calc.Interpolate(Min.Threshold, Max.Threshold, fraction);
        }

        public void Average(List<ErrorRange> partial)
        {
            Rate.Average(partial.ConvertAll<ROCPoint>(rate => rate.Rate));
            Min.Average(partial.ConvertAll<ROCPoint>(rate => rate.Min));
            Max.Average(partial.ConvertAll<ROCPoint>(rate => rate.Max));
        }

        void FindMinMax(ROCCurve ROC, ErrorPolicy.Evaluate policy, out ROCPoint left, out ROCPoint right)
        {
            left = new ROCPoint();
            right = new ROCPoint();
            if (CheckForNoZero(ROC, policy, ref left, ref right))
                return;
            if (FindZeroPoint(ROC, policy, ref left, ref right))
                return;
            if (FindZeroPair(ROC, policy, ref left, ref right))
                return;
            throw new AssertException();
        }

        bool CheckForNoZero(ROCCurve ROC, ErrorPolicy.Evaluate policy, ref ROCPoint left, ref ROCPoint right)
        {
            float first = policy(ROC.Curve[0]);
            float last = policy(ROC.Curve[ROC.Curve.Count - 1]);
            if (first > 0 && last > 0 || first < 0 && last < 0)
            {
                if (Math.Abs(first) < Math.Abs(last))
                    left = right = ROC.Curve[0];
                else
                    left = right = ROC.Curve[ROC.Curve.Count - 1];
                return true;
            }
            return false;
        }

        bool FindZeroPoint(ROCCurve ROC, ErrorPolicy.Evaluate policy, ref ROCPoint left, ref ROCPoint right)
        {
            for (int i = 0; i < ROC.Curve.Count; ++i)
                if (policy(ROC.Curve[i]) == 0)
                {
                    left = right = ROC.Curve[i];
                    return true;
                }
            return false;
        }

        bool FindZeroPair(ROCCurve ROC, ErrorPolicy.Evaluate policy, ref ROCPoint left, ref ROCPoint right)
        {
            for (int i = 0; i < ROC.Curve.Count - 1; ++i)
                if (policy(ROC.Curve[i]) * policy(ROC.Curve[i + 1]) < 0)
                {
                    left = ROC.Curve[i];
                    right = ROC.Curve[i + 1];
                    return true;
                }
            return false;
        }

        float FindZeroFraction(float min, float max)
        {
            if (min == max)
                return 0;
            else
            {
                if (min > max)
                    Calc.Swap(ref min, ref max);
                return (0 - min) / (max - min);
            }
        }
    }
}

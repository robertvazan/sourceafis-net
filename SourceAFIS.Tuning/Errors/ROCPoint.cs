using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Linq;
using SourceAFIS.General;

namespace SourceAFIS.Tuning.Errors
{
    public struct ROCPoint
    {
        [XmlAttribute]
        public float FAR;
        [XmlAttribute]
        public float FRR;
        [XmlAttribute]
        public float Threshold;

        public void Average(List<ROCPoint> partial)
        {
            FAR = partial.Average(point => point.FAR);
            FRR = partial.Average(point => point.FRR);
            Threshold = partial.Average(point => point.Threshold);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using SourceAFIS.General;
using System.Xml.Linq;
using SourceAFIS.Matching;
using SourceAFIS.Extraction;

namespace SourceAFIS
{
    public sealed class FingerprintTemplate
    {
        const int MaxDistance = 490;
        const int MaxNeighbors = 9;

        public List<FingerprintMinutia> Minutiae = new List<FingerprintMinutia>();
        internal NeighborEdge[][] EdgeTable;

        public FingerprintTemplate() { }

        public FingerprintTemplate(byte[,] image)
        {
            Extractor.Extract(image, this);
            BuildEdgeTable();
        }

        public FingerprintTemplate(XElement xml)
        {
            Minutiae = (from minutia in xml.Elements("Minutia")
                        select new FingerprintMinutia()
                        {
                            Position = new Point(
                                (int)minutia.Attribute("X"),
                                (int)minutia.Attribute("Y")),
                            Direction = (byte)(uint)minutia.Attribute("Direction"),
                            Type = (FingerprintMinutiaType)Enum.Parse(
                                typeof(FingerprintMinutiaType),
                                (string)minutia.Attribute("Type"),
                                false)
                        }).ToList();
            BuildEdgeTable();
        }

        public XElement ToXml()
        {
            return new XElement("FingerprintTemplate",
                from minutia in Minutiae
                select new XElement("Minutia",
                    new XAttribute("X", minutia.Position.X),
                    new XAttribute("Y", minutia.Position.Y),
                    new XAttribute("Direction", minutia.Direction),
                    new XAttribute("Type", minutia.Type.ToString())));
        }

        public override string ToString() { return ToXml().ToString(); }

        void BuildEdgeTable()
        {
            EdgeTable = new NeighborEdge[Minutiae.Count][];
            var edges = new List<NeighborEdge>();
            var allSqDistances = new int[Minutiae.Count];

            for (int reference = 0; reference < EdgeTable.Length; ++reference)
            {
                Point referencePosition = Minutiae[reference].Position;
                int sqMaxDistance = Calc.Sq(MaxDistance);
                if (Minutiae.Count - 1 > MaxNeighbors)
                {
                    for (int neighbor = 0; neighbor < Minutiae.Count; ++neighbor)
                        allSqDistances[neighbor] = Calc.DistanceSq(referencePosition, Minutiae[neighbor].Position);
                    Array.Sort(allSqDistances);
                    sqMaxDistance = allSqDistances[MaxNeighbors];
                }
                for (int neighbor = 0; neighbor < Minutiae.Count; ++neighbor)
                {
                    if (neighbor != reference && Calc.DistanceSq(referencePosition, Minutiae[neighbor].Position) <= sqMaxDistance)
                    {
                        NeighborEdge record = new NeighborEdge();
                        record.Edge = EdgeConstructor.Construct(this, reference, neighbor);
                        record.Neighbor = neighbor;
                        edges.Add(record);
                    }
                }

                edges.Sort(NeighborEdgeComparer.Instance);
                if (edges.Count > MaxNeighbors)
                    edges.RemoveRange(MaxNeighbors, edges.Count - MaxNeighbors);
                EdgeTable[reference] = edges.ToArray();
                edges.Clear();
            }
        }

        class NeighborEdgeComparer : IComparer<NeighborEdge>
        {
            public int Compare(NeighborEdge left, NeighborEdge right)
            {
                int result = Calc.Compare(left.Edge.Length, right.Edge.Length);
                if (result != 0)
                    return result;
                return Calc.Compare(left.Neighbor, right.Neighbor);
            }

            public static NeighborEdgeComparer Instance = new NeighborEdgeComparer();
        }
    }
}

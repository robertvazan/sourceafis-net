// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using SourceAFIS.Engine.Configuration;
using SourceAFIS.Engine.Features;
using SourceAFIS.Engine.Primitives;

namespace SourceAFIS.Engine.Extractor.Skeletons
{
    static class BinaryThinning
    {
        enum NeighborhoodType
        {
            Skeleton,
            Ending,
            Removable
        }
        static NeighborhoodType[] NeighborhoodTypes()
        {
            var types = new NeighborhoodType[256];
            for (uint mask = 0; mask < 256; ++mask)
            {
                bool TL = (mask & 1) != 0;
                bool TC = (mask & 2) != 0;
                bool TR = (mask & 4) != 0;
                bool CL = (mask & 8) != 0;
                bool CR = (mask & 16) != 0;
                bool BL = (mask & 32) != 0;
                bool BC = (mask & 64) != 0;
                bool BR = (mask & 128) != 0;
                uint count = Integers.PopulationCount(mask);
                bool diagonal = !TC && !CL && TL || !CL && !BC && BL || !BC && !CR && BR || !CR && !TC && TR;
                bool horizontal = !TC && !BC && (TR || CR || BR) && (TL || CL || BL);
                bool vertical = !CL && !CR && (TL || TC || TR) && (BL || BC || BR);
                bool end = count == 1;
                if (end)
                    types[mask] = NeighborhoodType.Ending;
                else if (!diagonal && !horizontal && !vertical)
                    types[mask] = NeighborhoodType.Removable;
            }
            return types;
        }
        static bool IsFalseEnding(BooleanMatrix binary, IntPoint ending)
        {
            foreach (var relativeNeighbor in IntPoint.CornerNeighbors)
            {
                var neighbor = ending + relativeNeighbor;
                if (binary[neighbor])
                {
                    int count = 0;
                    foreach (var relative2 in IntPoint.CornerNeighbors)
                        if (binary.Get(neighbor + relative2, false))
                            ++count;
                    return count > 2;
                }
            }
            return false;
        }
        public static BooleanMatrix Thin(BooleanMatrix input, SkeletonType type)
        {
            var neighborhoodTypes = NeighborhoodTypes();
            var size = input.Size;
            var mutable = new BooleanMatrix(size);
            for (int y = 1; y < size.Y - 1; ++y)
                for (int x = 1; x < size.X - 1; ++x)
                    mutable[x, y] = input[x, y];
            var thinned = new BooleanMatrix(size);
            bool removedAnything = true;
            for (int i = 0; i < Parameters.ThinningIterations && removedAnything; ++i)
            {
                removedAnything = false;
                for (int evenY = 0; evenY < 2; ++evenY)
                    for (int evenX = 0; evenX < 2; ++evenX)
                        for (int y = 1 + evenY; y < size.Y - 1; y += 2)
                            for (int x = 1 + evenX; x < size.X - 1; x += 2)
                                if (mutable[x, y] && !thinned[x, y] && !(mutable[x, y - 1] && mutable[x, y + 1] && mutable[x - 1, y] && mutable[x + 1, y]))
                                {
                                    uint neighbors = (mutable[x + 1, y + 1] ? 128u : 0u)
                                        | (mutable[x, y + 1] ? 64u : 0u)
                                        | (mutable[x - 1, y + 1] ? 32u : 0u)
                                        | (mutable[x + 1, y] ? 16u : 0u)
                                        | (mutable[x - 1, y] ? 8u : 0u)
                                        | (mutable[x + 1, y - 1] ? 4u : 0u)
                                        | (mutable[x, y - 1] ? 2u : 0u)
                                        | (mutable[x - 1, y - 1] ? 1u : 0u);
                                    if (neighborhoodTypes[neighbors] == NeighborhoodType.Removable
                                        || neighborhoodTypes[neighbors] == NeighborhoodType.Ending
                                        && IsFalseEnding(mutable, new IntPoint(x, y)))
                                    {
                                        removedAnything = true;
                                        mutable[x, y] = false;
                                    }
                                    else
                                        thinned[x, y] = true;
                                }
            }
            // https://sourceafis.machinezoo.com/transparency/thinned-skeleton
            FingerprintTransparency.Current.Log(type.Prefix() + "thinned-skeleton", thinned);
            return thinned;
        }
    }
}

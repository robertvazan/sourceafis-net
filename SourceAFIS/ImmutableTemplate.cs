// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;

namespace SourceAFIS
{
	class ImmutableTemplate
	{
		const int Prime = 1610612741;

		public static readonly ImmutableTemplate Empty = new ImmutableTemplate();

		public readonly IntPoint Size;
		public readonly ImmutableMinutia[] Minutiae;
		public readonly NeighborEdge[][] Edges;

		ImmutableTemplate() {
			size = new IntPoint(1, 1);
			minutiae = new ImmutableMinutia[0];
			edges = new NeighborEdge[0][];
		}
		public ImmutableTemplate(MutableTemplate mutable) {
			Size = mutable.Size;
			var minutiae =
				from m in mutable.Minutiae
				orderby ((m.Position.X * Prime) + m.Position.Y) * Prime, m.Position.X, m.Position.Y, m.Direction, m.Type
				select m;
			Minutiae = minutiae.ToArray();
			Edges = NeighborEdge.BuildTable(Minutiae);
		}
		public MutableTemplate Mutable() {
			var mutable = new MutableTemplate();
			mutable.Size = Size;
			mutable.Minutiae = (from m in Minutiae select m.Mutable()).ToList();
			return mutable;
		}
	}
}

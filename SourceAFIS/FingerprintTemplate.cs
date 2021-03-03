// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;
using System.Collections.Generic;
using System.Linq;

namespace SourceAFIS
{
	public class FingerprintTemplate
	{
		const int Prime = 1610612741;

		public static readonly FingerprintTemplate Empty = new FingerprintTemplate();

		internal readonly IntPoint Size;
		internal readonly ImmutableMinutia[] Minutiae;
		internal readonly NeighborEdge[][] Edges;

		FingerprintTemplate()
		{
			Size = new IntPoint(1, 1);
			Minutiae = new ImmutableMinutia[0];
			Edges = new NeighborEdge[0][];
		}
		FingerprintTemplate(MutableTemplate mutable)
		{
			Size = mutable.Size;
			var minutiae =
				from m in mutable.Minutiae
				orderby ((m.Position.X * Prime) + m.Position.Y) * Prime, m.Position.X, m.Position.Y, m.Direction, m.Type
				select new ImmutableMinutia(m);
			Minutiae = minutiae.ToArray();
			// https://sourceafis.machinezoo.com/transparency/shuffled-minutiae
			FingerprintTransparency.Current.Log("shuffled-minutiae", () => Mutable());
			Edges = NeighborEdge.BuildTable(Minutiae);
		}
		public FingerprintTemplate(FingerprintImage image) : this(FeatureExtractor.Extract(image.Matrix, image.Dpi)) { }
		public FingerprintTemplate(byte[] serialized) : this(Deserialize(serialized)) { }

		MutableTemplate Mutable()
		{
			var mutable = new MutableTemplate();
			mutable.Size = Size;
			mutable.Minutiae = (from m in Minutiae select m.Mutable()).ToList();
			return mutable;
		}
		public byte[] ToByteArray() { return SerializationUtils.Serialize(new PersistentTemplate(Mutable())); }
		static MutableTemplate Deserialize(byte[] serialized)
		{
			var persistent = SerializationUtils.Deserialize<PersistentTemplate>(serialized);
			persistent.Validate();
			return persistent.Mutable();
		}
	}
}

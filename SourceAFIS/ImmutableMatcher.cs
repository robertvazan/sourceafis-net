// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;
using System.Collections.Generic;

namespace SourceAFIS
{
	class ImmutableMatcher
	{
		public static readonly ImmutableMatcher Null = new ImmutableMatcher();

		public readonly ImmutableTemplate Template;
		public readonly Dictionary<int, List<IndexedEdge>> EdgeHash;

		ImmutableMatcher() {
			Template = ImmutableTemplate.Empty;
			EdgeHash = new Dictionary<int, List<IndexedEdge>>();
		}
		public ImmutableMatcher(ImmutableTemplate template, Dictionary<int, List<IndexedEdge>> edgeHash) {
			Template = template;
			EdgeHash = edgeHash;
		}
	}
}

// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;
using System.Collections.Generic;

namespace SourceAFIS
{
	class MatcherThread
	{
		[ThreadStatic]
		static MatcherThread CurrentInstance;

		public FingerprintTemplate Probe;
		Dictionary<int, List<IndexedEdge>> EdgeHash;
		public FingerprintTemplate Candidate;
		MinutiaPair[] Pool = new MinutiaPair[1];
		int Pooled;
		PriorityQueue<MinutiaPair> Queue = new PriorityQueue<MinutiaPair>(Comparer<MinutiaPair>.Create((a, b) => a.Distance.CompareTo(b.Distance)));
		public int Count;
		public MinutiaPair[] Tree = new MinutiaPair[1];
		MinutiaPair[] ByProbe = new MinutiaPair[1];
		MinutiaPair[] ByCandidate = new MinutiaPair[1];
		MinutiaPair[] Roots = new MinutiaPair[1];
		readonly HashSet<int> Duplicates = new HashSet<int>();
		Score Score = new Score();
		readonly List<MinutiaPair> SupportEdges = new List<MinutiaPair>();
		bool ReportSupport;

		public static MatcherThread Current
		{
			get
			{
				if (CurrentInstance == null)
					CurrentInstance = new MatcherThread();
				return CurrentInstance;
			}
		}

		public void SelectMatcher(FingerprintMatcher matcher)
		{
			Probe = matcher.Template;
			if (Probe.Minutiae.Length > Tree.Length)
			{
				Tree = new MinutiaPair[Probe.Minutiae.Length];
				ByProbe = new MinutiaPair[Probe.Minutiae.Length];
			}
			EdgeHash = matcher.EdgeHash;
		}
		public void SelectCandidate(FingerprintTemplate template)
		{
			Candidate = template;
			if (ByCandidate.Length < Candidate.Minutiae.Length)
				ByCandidate = new MinutiaPair[Candidate.Minutiae.Length];
		}
		public double Match()
		{
			try
			{
				int totalRoots = EnumerateRoots();
				double high = 0;
				int best = -1;
				for (int i = 0; i < totalRoots; ++i)
				{
					double score = TryRoot(Roots[i]);
					if (best < 0 || score > high)
					{
						high = score;
						best = i;
					}
					ClearPairing();
				}
				return high;
			}
			catch (Exception)
			{
				CurrentInstance = new MatcherThread();
				throw;
			}
			finally
			{
			}
		}
		int EnumerateRoots()
		{
			if (Roots.Length < Parameters.MaxTriedRoots)
				Roots = new MinutiaPair[Parameters.MaxTriedRoots];
			int totalLookups = 0;
			int totalRoots = 0;
			int triedRoots = 0;
			Duplicates.Clear();
			foreach (bool shortEdges in new bool[] { false, true })
			{
				for (int period = 1; period < Candidate.Minutiae.Length; ++period)
				{
					for (int phase = 0; phase <= period; ++phase)
					{
						for (int candidateReference = phase; candidateReference < Candidate.Minutiae.Length; candidateReference += period + 1)
						{
							int candidateNeighbor = (candidateReference + period) % Candidate.Minutiae.Length;
							var candidateEdge = new EdgeShape(Candidate.Minutiae[candidateReference], Candidate.Minutiae[candidateNeighbor]);
							if ((candidateEdge.Length >= Parameters.MinRootEdgeLength) ^ shortEdges)
							{
								List<IndexedEdge> matches;
								if (EdgeHash.TryGetValue(HashShape(candidateEdge), out matches))
								{
									foreach (var match in matches)
									{
										if (MatchingShapes(match, candidateEdge))
										{
											int duplicateKey = (match.Reference << 16) | candidateReference;
											if (Duplicates.Add(duplicateKey))
											{
												var pair = Allocate();
												pair.Probe = match.Reference;
												pair.Candidate = candidateReference;
												Roots[totalRoots] = pair;
												++totalRoots;
											}
											++triedRoots;
											if (triedRoots >= Parameters.MaxTriedRoots)
												return totalRoots;
										}
									}
								}
								++totalLookups;
								if (totalLookups >= Parameters.MaxRootEdgeLookups)
									return totalRoots;
							}
						}
					}
				}
			}
			return totalRoots;
		}
		static int HashShape(EdgeShape edge)
		{
			int lengthBin = edge.Length / Parameters.MaxDistanceError;
			int referenceAngleBin = (int)(edge.ReferenceAngle / Parameters.MaxAngleError);
			int neighborAngleBin = (int)(edge.NeighborAngle / Parameters.MaxAngleError);
			return (referenceAngleBin << 24) + (neighborAngleBin << 16) + lengthBin;
		}
		static bool MatchingShapes(EdgeShape probe, EdgeShape candidate)
		{
			int lengthDelta = probe.Length - candidate.Length;
			if (lengthDelta >= -Parameters.MaxDistanceError && lengthDelta <= Parameters.MaxDistanceError)
			{
				double complementaryAngleError = DoubleAngle.Complementary(Parameters.MaxAngleError);
				double referenceDelta = DoubleAngle.Difference(probe.ReferenceAngle, candidate.ReferenceAngle);
				if (referenceDelta <= Parameters.MaxAngleError || referenceDelta >= complementaryAngleError)
				{
					double neighborDelta = DoubleAngle.Difference(probe.NeighborAngle, candidate.NeighborAngle);
					if (neighborDelta <= Parameters.MaxAngleError || neighborDelta >= complementaryAngleError)
						return true;
				}
			}
			return false;
		}
		double TryRoot(MinutiaPair root)
		{
			Queue.Add(root);
			do {
				AddPair(Queue.Remove());
				CollectEdges();
				SkipPaired();
			} while (Queue.Count > 0);
			Score.Compute(this);
			return Score.ShapedScore;
		}
		void ClearPairing()
		{
			for (int i = 0; i < Count; ++i)
			{
				ByProbe[Tree[i].Probe] = null;
				ByCandidate[Tree[i].Candidate] = null;
				Release(Tree[i]);
				Tree[i] = null;
			}
			Count = 0;
			if (ReportSupport)
			{
				foreach (var pair in SupportEdges)
					Release(pair);
				SupportEdges.Clear();
			}
		}
		void CollectEdges()
		{
			var reference = Tree[Count - 1];
			var probeNeighbors = Probe.Edges[reference.Probe];
			var candidateNeigbors = Candidate.Edges[reference.Candidate];
			foreach (var pair in MatchPairs(probeNeighbors, candidateNeigbors))
			{
				pair.ProbeRef = reference.Probe;
				pair.CandidateRef = reference.Candidate;
				if (ByCandidate[pair.Candidate] == null && ByProbe[pair.Probe] == null)
					Queue.Add(pair);
				else
					Support(pair);
			}
		}
		List<MinutiaPair> MatchPairs(NeighborEdge[] probeStar, NeighborEdge[] candidateStar)
		{
			double complementaryAngleError = DoubleAngle.Complementary(Parameters.MaxAngleError);
			var results = new List<MinutiaPair>();
			int start = 0;
			int end = 0;
			for (int candidateIndex = 0; candidateIndex < candidateStar.Length; ++candidateIndex)
			{
				var candidateEdge = candidateStar[candidateIndex];
				while (start < probeStar.Length && probeStar[start].Length < candidateEdge.Length - Parameters.MaxDistanceError)
					++start;
				if (end < start)
					end = start;
				while (end < probeStar.Length && probeStar[end].Length <= candidateEdge.Length + Parameters.MaxDistanceError)
					++end;
				for (int probeIndex = start; probeIndex < end; ++probeIndex)
				{
					var probeEdge = probeStar[probeIndex];
					double referenceDiff = DoubleAngle.Difference(probeEdge.ReferenceAngle, candidateEdge.ReferenceAngle);
					if (referenceDiff <= Parameters.MaxAngleError || referenceDiff >= complementaryAngleError)
					{
						double neighborDiff = DoubleAngle.Difference(probeEdge.NeighborAngle, candidateEdge.NeighborAngle);
						if (neighborDiff <= Parameters.MaxAngleError || neighborDiff >= complementaryAngleError)
						{
							var pair = Allocate();
							pair.Probe = probeEdge.Neighbor;
							pair.Candidate = candidateEdge.Neighbor;
							pair.Distance = candidateEdge.Length;
							results.Add(pair);
						}
					}
				}
			}
			return results;
		}
		void SkipPaired()
		{
			while (Queue.Count > 0 && (ByProbe[Queue.Peek().Probe] != null || ByCandidate[Queue.Peek().Candidate] != null))
				Support(Queue.Remove());
		}
		void AddPair(MinutiaPair pair)
		{
			Tree[Count] = pair;
			ByProbe[pair.Probe] = pair;
			ByCandidate[pair.Candidate] = pair;
			++Count;
		}
		void Support(MinutiaPair pair)
		{
			if (ByProbe[pair.Probe] != null && ByProbe[pair.Probe].Candidate == pair.Candidate)
			{
				++ByProbe[pair.Probe].SupportingEdges;
				++ByProbe[pair.ProbeRef].SupportingEdges;
				if (ReportSupport)
					SupportEdges.Add(pair);
				else
					Release(pair);
			}
			else
				Release(pair);
		}
		MinutiaPair Allocate()
		{
			if (Pooled > 0)
			{
				--Pooled;
				var pair = Pool[Pooled];
				Pool[Pooled] = null;
				return pair;
			}
			else
				return new MinutiaPair();
		}
		void Release(MinutiaPair pair)
		{
			if (Pooled >= Pool.Length)
				Array.Resize(ref Pool, 2 * Pool.Length);
			pair.Probe = 0;
			pair.Candidate = 0;
			pair.ProbeRef = 0;
			pair.CandidateRef = 0;
			pair.Distance = 0;
			pair.SupportingEdges = 0;
			Pool[Pooled] = pair;
		}
	}
}

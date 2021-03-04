// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SourceAFIS
{
	public abstract class FingerprintTransparency : IDisposable
	{
		[ThreadStatic]
		static FingerprintTransparency CurrentInstance;
		class NoFingerprintTransparency : FingerprintTransparency
		{
			public override bool Accepts(String key) { return false; }
		}
		static readonly NoFingerprintTransparency FallbackInstance = new NoFingerprintTransparency();
		internal static FingerprintTransparency Current { get { return CurrentInstance ?? FallbackInstance; } }

		FingerprintTransparency Outer;
		protected FingerprintTransparency()
		{
			Outer = CurrentInstance;
			CurrentInstance = this;
		}
		bool Disposed;
		public void Dispose()
		{
			if (!Disposed) {
				Disposed = true;
				CurrentInstance = Outer;
				Outer = null;
			}
		}

		static string Suffix(string mime)
		{
			switch (mime)
			{
				case "application/cbor":
					return ".cbor";
				case "text/plain":
					return ".txt";
				case "application/json":
					return ".json";
				case "application/xml":
					return ".xml";
				case "image/jpeg":
					return ".jpeg";
				case "image/png":
					return ".png";
				case "image/bmp":
					return ".bmp";
				case "image/tiff":
					return ".tiff";
				case "image/jp2":
					return ".jp2";
				case "image/x-wsq":
					return ".wsq";
				default:
					return ".dat";
			}
		}

		public virtual bool Accepts(string key) { return true; }
		public virtual void Take(string key, string mime, byte[] data) { }

		volatile bool VersionOffered;
		void LogVersion()
		{
			if (!VersionOffered)
			{
				bool offer = false;
				lock (this)
				{
					if (!VersionOffered)
					{
						VersionOffered = true;
						offer = true;
					}
				}
				if (offer && Accepts("version"))
					Take("version", "text/plain", Encoding.UTF8.GetBytes(FingerprintCompatibility.Version()));
			}
		}
		void Log(string key, string mime, Func<byte[]> supplier)
		{
			LogVersion();
			if (Accepts(key))
			{
				// TODO: Remove exception handling once the CBOR library is fixed.
				try
				{
					Take(key, mime, supplier());
				}
				catch (Exception)
				{
				}
			}
		}
		internal void Log<T>(string key, Func<T> supplier) { Log(key, "application/cbor", () => SerializationUtils.Serialize(supplier())); }
		internal void Log(string key, object data) { Log(key, "application/cbor", () => SerializationUtils.Serialize(data)); }

		class CborSkeletonRidge
		{
			public int Start;
			public int End;
			public IList<IntPoint> Points;
		}
		class CborSkeleton
		{
			public int Width;
			public int Height;
			public List<IntPoint> Minutiae;
			public List<CborSkeletonRidge> Ridges;
			public CborSkeleton(Skeleton skeleton)
			{
				Width = skeleton.Size.X;
				Height = skeleton.Size.Y;
				var offsets = new Dictionary<SkeletonMinutia, int>();
				for (int i = 0; i < skeleton.Minutiae.Count; ++i)
					offsets[skeleton.Minutiae[i]] = i;
				Minutiae = (from m in skeleton.Minutiae select m.Position).ToList();
				Ridges = (
					from m in skeleton.Minutiae
					from r in m.Ridges
					where r.Points is CircularList<IntPoint>
					select new CborSkeletonRidge
					{
						Start = offsets[r.Start],
						End = offsets[r.End],
						Points = r.Points
					}).ToList();
			}
		}
		internal void LogSkeleton(string keyword, Skeleton skeleton) { Log(Skeleton.Prefix(skeleton.Type) + keyword, () => new CborSkeleton(skeleton)); }

		class CborHashEntry
		{
			public int Key;
			public List<IndexedEdge> Edges;
		}
		// https://sourceafis.machinezoo.com/transparency/edge-hash
		internal void LogEdgeHash(Dictionary<int, List<IndexedEdge>> hash)
		{
			Log("edge-hash", () => (
				from k in hash.Keys
				orderby k
				select new CborHashEntry()
				{
					Key = k,
					Edges = hash[k]
				}).ToList());
		}

		volatile bool MatcherOffered;
		volatile bool AcceptsRootPairs;
		volatile bool AcceptsPairingFlag;
		volatile bool AcceptsScore;
		volatile bool AcceptsBestMatch;
		void OfferMatcher()
		{
			if (!MatcherOffered)
			{
				AcceptsRootPairs = Accepts("root-pairs");
				AcceptsPairingFlag = Accepts("pairing");
				AcceptsScore = Accepts("score");
				AcceptsBestMatch = Accepts("best-match");
				MatcherOffered = true;
			}
		}

		class CborPair
		{
			public int Probe;
			public int Candidate;
		}
		// https://sourceafis.machinezoo.com/transparency/roots
		internal void LogRootPairs(int count, MinutiaPair[] roots)
		{
			OfferMatcher();
			if (AcceptsRootPairs)
				Log("roots", () => (from p in roots select new CborPair() { Probe = p.Probe, Candidate = p.Candidate }).Take(count).ToList());
		}

		internal bool AcceptsPairing()
		{
			OfferMatcher();
			return AcceptsPairingFlag;
		}
		class CborEdge
		{
			public int ProbeFrom;
			public int ProbeTo;
			public int CandidateFrom;
			public int CandidateTo;
			public CborEdge(MinutiaPair pair)
			{
				ProbeFrom = pair.ProbeRef;
				ProbeTo = pair.Probe;
				CandidateFrom = pair.CandidateRef;
				CandidateTo = pair.Candidate;
			}
		}
		class CborPairing
		{
			public CborPair Root;
			public List<CborEdge> Tree;
			public List<CborEdge> Support;
			public CborPairing(int count, MinutiaPair[] pairs, List<MinutiaPair> support)
			{
				Root = new CborPair() { Probe = pairs[0].Probe, Candidate = pairs[0].Candidate };
				Tree = (from p in pairs select new CborEdge(p)).Take(count).ToList();
				Support = (from p in support select new CborEdge(p)).ToList();
			}
		}
		// https://sourceafis.machinezoo.com/transparency/pairing
		internal void LogPairing(int count, MinutiaPair[] pairs, List<MinutiaPair> support)
		{
			OfferMatcher();
			if (AcceptsPairingFlag)
				Log("pairing", new CborPairing(count, pairs, support));
		}

		// https://sourceafis.machinezoo.com/transparency/score
		internal void LogScore(Score score)
		{
			OfferMatcher();
			if (AcceptsScore)
				Log("score", score);
		}

		// https://sourceafis.machinezoo.com/transparency/best-match
		internal void LogBestMatch(int nth)
		{
			OfferMatcher();
			if (AcceptsBestMatch)
				Take("best-match", "text/plain", Encoding.UTF8.GetBytes(nth.ToString()));
		}
	}
}

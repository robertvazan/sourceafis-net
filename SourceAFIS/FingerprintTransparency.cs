// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SourceAFIS.Features;
using SourceAFIS.Matcher;
using SourceAFIS.Primitives;
using SourceAFIS.Transparency;

namespace SourceAFIS
{
    /// <summary>Algorithm transparency API that can capture all intermediate data structures produced by SourceAFIS algorithm.</summary>
    /// <remarks>
    /// <para>
    /// See <see href="https://sourceafis.machinezoo.com/transparency/">algorithm transparency</see> pages
    /// on SourceAFIS website for more information and a tutorial on how to use this class.
    /// </para>
    /// <para>
    /// Applications can subclass <c>FingerprintTransparency</c> and override
    /// <see cref="Take(string,string,byte[])" /> method to define new transparency data logger.
    /// Applications can control what transparency data gets produced by overriding <see cref="Accepts(string)" />.
    /// </para>
    /// <para>
    /// <c>FingerprintTransparency</c> instance should be created in a <c>using</c> statement.
    /// It will be capturing transparency data from all operations on current thread
    /// between invocation of the constructor and invocation of <see cref="Dispose()" /> method,
    /// which is called automatically by the <c>using</c> statement.
    /// </para>
    /// </remarks>
    /// <seealso href="https://sourceafis.machinezoo.com/transparency/">Algorithm transparency in SourceAFIS</seealso>
    public abstract partial class FingerprintTransparency : IDisposable
    {
        [ThreadStatic]
        static FingerprintTransparency CurrentInstance;
        internal static FingerprintTransparency Current { get { return CurrentInstance ?? NoTransparency.Instance; } }

        FingerprintTransparency Outer;
        /// <summary>Creates an instance of <c>FingerprintTransparency</c> and activates it.</summary>
        /// <remarks>
        /// <para>
        /// Activation places the new <c>FingerprintTransparency</c> instance in thread-local storage,
        /// which causes all operations executed by current thread to log data to this <c>FingerprintTransparency</c> instance.
        /// If activations are nested, data is only logged to the currently innermost <c>FingerprintTransparency</c>.
        /// </para>
        /// <para>
        /// Deactivation happens in <see cref="Dispose()" /> method.
        /// Instances of <c>FingerprintTransparency</c> should be created in <c>using</c> statement
        /// to ensure that <see cref="Dispose()" /> is always called.
        /// </para>
        /// <para>
        /// <c>FingerprintTransparency</c> is an abstract class.
        /// This constructor is only called by subclasses.
        /// </para>
        /// </remarks>
        /// <seealso cref="Dispose()" />
        protected FingerprintTransparency()
        {
            Outer = CurrentInstance;
            CurrentInstance = this;
        }
        bool Disposed;
        /// <summary>Deactivates transparency logging and releases system resources held by this instance if any.</summary>
        /// <remarks>
        /// <para>
        /// This method is normally called automatically when <c>FingerprintTransparency</c> is used in <c>using</c> statement.
        /// </para>
        /// <para>
        /// Deactivation stops transparency data logging to this instance of <c>FingerprintTransparency</c>.
        /// Logging thus takes place between invocation of constructor (<see cref="FingerprintTransparency()" />) and invocation of this method.
        /// If activations were nested, this method reactivates the outer <c>FingerprintTransparency</c>.
        /// </para>
        /// <para>
        /// Subclasses can override this method to perform cleanup.
        /// Default implementation of this method performs deactivation.
        /// It must be called by overriding methods for deactivation to work correctly.
        /// </para>
        /// </remarks>
        /// <seealso cref="FingerprintTransparency()" />
        public void Dispose()
        {
            if (!Disposed)
            {
                Disposed = true;
                CurrentInstance = Outer;
                Outer = null;
            }
        }

        /// <summary>Filters transparency data keys that can be passed to <see cref="Take(string,string,byte[])" />.</summary>
        /// <remarks>
        /// <para>
        /// Default implementation always returns <c>true</c>, i.e. all transparency data is passed to <see cref="Take(string,string,byte[])" />.
        /// Implementation can override this method to filter some keys out, which improves performance.
        /// </para>
        /// <para>
        /// This method should always return the same result for the same key.
        /// Result may be cached and this method might not be called every time something is about to be logged.
        /// </para>
        /// </remarks>
        /// <param name="key">Transparency data key as used in <see cref="Take(string,string,byte[])" />.</param>
        /// <returns>Boolean status indicating whether transparency data under given key should be logged.</returns>
        /// <seealso cref="Take(string,string,byte[])" />
        public virtual bool Accepts(string key) { return true; }
        /// <summary>Records transparency data.</summary>
        /// <remarks>
        /// <para>
        /// Subclasses must override this method, because the default implementation does nothing.
        /// While this <c>FingerprintTransparency</c> object is active (between call to the constructor and call to <see cref="Dispose()" />),
        /// this method is called with transparency data in its parameters.
        /// </para>
        /// <para>
        /// Parameter <paramref name="key" /> specifies the kind of transparency data being logged,
        /// usually corresponding to some stage in the algorithm.
        /// Parameter <paramref name="data" /> then contains the actual transparency data.
        /// This method may be called multiple times with the same <paramref name="key" />
        /// if the algorithm produces that kind of transparency data repeatedly.
        /// See <see href="https://sourceafis.machinezoo.com/transparency/">algorithm transparency</see>
        /// on SourceAFIS website for documentation of the structure of the transparency data.
        /// </para>
        /// <para>
        /// Transparency data is offered only if <see cref="Accepts(string)" /> returns <c>true</c> for the same <paramref name="key" />.
        /// This allows applications to efficiently collect only transparency data that is actually needed.
        /// </para>
        /// <para>
        /// MIME type of the transparency data is provided, which may be useful for generic implementations,
        /// for example transparency data browser app that changes type of visualization based on the MIME type.
        /// Most transparency data is serialized in <see href="https://cbor.io/">CBOR</see> format (MIME application/cbor).
        /// </para>
        /// <para>
        /// Implementations of this method should be synchronized. Although the current SourceAFIS algorithm is single-threaded,
        /// future versions of SourceAFIS might run some parts of the algorithm in parallel, which would result in concurrent calls to this method.
        /// </para>
        /// <para>
        /// If this method throws, exception is propagated through SourceAFIS code.
        /// </para>
        /// </remarks>
        /// <param name="key">Specifies the kind of transparency data being logged.</param>
        /// <param name="mime">MIME type of the transparency data in <paramref name="data" /> parameter.</param>
        /// <param name="data">Transparency data being logged.</param>
        /// <seealso href="https://sourceafis.machinezoo.com/transparency/">Algorithm transparency in SourceAFIS</seealso>
        /// <seealso cref="Accepts(string)" />
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
                    Take("version", "text/plain", Encoding.UTF8.GetBytes(FingerprintCompatibility.Version));
            }
        }
        void Log(string key, string mime, Func<byte[]> supplier)
        {
            LogVersion();
            if (Accepts(key))
            {
                try
                {
                    Take(key, mime, supplier());
                }
                catch (IndexOutOfRangeException)
                {
                    // Workaround for a bug in CBOR serializer that throws exceptions when it encounters empty array.
                }
            }
        }
        internal void Log<T>(string key, Func<T> supplier) { Log(key, "application/cbor", () => SerializationUtils.Serialize(supplier())); }
        internal void Log(string key, object data) { Log(key, "application/cbor", () => SerializationUtils.Serialize(data)); }
        internal void LogSkeleton(string keyword, Skeleton skeleton) { Log(skeleton.Type.Prefix() + keyword, () => new ConsistentSkeleton(skeleton)); }
        // https://sourceafis.machinezoo.com/transparency/edge-hash
        internal void LogEdgeHash(Dictionary<int, List<IndexedEdge>> hash)
        {
            Log("edge-hash", () => (
                from k in hash.Keys
                orderby k
                select new ConsistentHashEntry()
                {
                    Key = k,
                    Edges = hash[k]
                }).ToList());
        }

        volatile bool MatcherOffered;
        volatile bool AcceptingRootPairs;
        volatile bool AcceptingPairing;
        volatile bool AcceptingBestPairing;
        volatile bool AcceptingScore;
        volatile bool AcceptingBestScore;
        volatile bool AcceptingBestMatch;
        void OfferMatcher()
        {
            if (!MatcherOffered)
            {
                AcceptingRootPairs = Accepts("root-pairs");
                AcceptingPairing = Accepts("pairing");
                AcceptingBestPairing = Accepts("best-pairing");
                AcceptingScore = Accepts("score");
                AcceptingBestScore = Accepts("best-score");
                AcceptingBestMatch = Accepts("best-match");
                MatcherOffered = true;
            }
        }
        // Expose fast method to check whether pairing should be logged, so that we can easily skip support edge logging.
        // Do the same for best score, so that we can skip reevaluation of the best pairing.
        internal bool AcceptsPairing()
        {
            OfferMatcher();
            return AcceptingPairing;
        }
        internal bool AcceptsBestPairing()
        {
            OfferMatcher();
            return AcceptingBestPairing;
        }
        internal bool AcceptsBestScore()
        {
            OfferMatcher();
            return AcceptingBestScore;
        }
        // https://sourceafis.machinezoo.com/transparency/roots
        internal void LogRootPairs(int count, MinutiaPair[] roots)
        {
            OfferMatcher();
            if (AcceptingRootPairs)
                Log("roots", () => (from p in roots select new ConsistentMinutiaPair() { Probe = p.Probe, Candidate = p.Candidate }).Take(count).ToList());
        }
        // https://sourceafis.machinezoo.com/transparency/pairing
        internal void LogPairing(PairingGraph pairing)
        {
            OfferMatcher();
            if (AcceptingPairing)
                Log("pairing", new ConsistentPairingGraph(pairing.Count, pairing.Tree, pairing.SupportEdges));
        }
        internal void LogBestPairing(PairingGraph pairing)
        {
            OfferMatcher();
            if (AcceptingBestPairing)
                Log("best-pairing", new ConsistentPairingGraph(pairing.Count, pairing.Tree, pairing.SupportEdges));
        }
        // https://sourceafis.machinezoo.com/transparency/score
        internal void LogScore(ScoringData score)
        {
            OfferMatcher();
            if (AcceptingScore)
                Log("score", score);
        }
        internal void LogBestScore(ScoringData score)
        {
            OfferMatcher();
            if (AcceptingBestScore)
                Log("best-score", score);
        }
        // https://sourceafis.machinezoo.com/transparency/best-match
        internal void LogBestMatch(int nth)
        {
            OfferMatcher();
            if (AcceptingBestMatch)
                Take("best-match", "text/plain", Encoding.UTF8.GetBytes(nth.ToString()));
        }
    }
}

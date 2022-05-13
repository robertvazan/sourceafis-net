// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;
using System.Linq;

namespace SourceAFIS
{
    /// <summary>Biometric description of a fingerprint suitable for efficient matching.</summary>
    /// <remarks>
    /// <para>
    /// Fingerprint template holds high-level fingerprint features, specifically ridge endings and bifurcations (together called minutiae).
    /// Original image is not preserved in the fingerprint template and there is no way to reconstruct the original fingerprint from its template.
    /// </para>
    /// <para>
    /// <see cref="FingerprintImage" /> can be converted to template by calling <see cref="FingerprintTemplate(FingerprintImage)" /> constructor.
    /// </para>
    /// <para>
    /// Since image processing is expensive, applications should cache serialized templates.
    /// Serialization into CBOR format is performed by <see cref="ToByteArray()" /> method.
    /// CBOR template can be deserialized by calling <see cref="FingerprintTemplate(byte[])" /> constructor.
    /// </para>
    /// <para>
    /// Matching is performed by constructing <see cref="FingerprintMatcher" />,
    /// passing probe fingerprint to its <see cref="FingerprintMatcher(FingerprintTemplate)" /> constructor,
    /// and then passing candidate fingerprints to its <see cref="FingerprintMatcher.Match(FingerprintTemplate)" /> method.
    /// </para>
    /// <para>
    /// <c>FingerprintTemplate</c> contains two kinds of data: fingerprint features and search data structures.
    /// Search data structures speed up matching at the cost of some RAM.
    /// Only fingerprint features are serialized. Search data structures are recomputed after every deserialization.
    /// </para>
    /// </remarks>
    /// <seealso href="https://sourceafis.machinezoo.com/java">SourceAFIS for Java tutorial</seealso>
    /// <seealso cref="FingerprintImage" />
    /// <seealso cref="FingerprintMatcher" />
    public class FingerprintTemplate
    {
        const int Prime = 1610612741;

        /// <summary>Gets the empty fallback template with no biometric data.</summary>
        /// <value>Empty template.</value>
        /// <remarks>
        /// Empty template is useful as a fallback to simplify code.
        /// It contains no biometric data and it does not match any other template including itself.
        /// There is only one global instance. This property does not instantiate any new objects.
        /// </remarks>
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
        /// <summary>Creates fingerprint template from fingerprint image.</summary>
        /// <remarks>
        /// This constructor runs an expensive feature extractor algorithm,
        /// which analyzes the image and collects identifiable biometric features from it.
        /// </remarks>
        /// <param name="image">Fingerprint image to process.</param>
        /// <exception cref="NullReferenceException">Thrown when <paramref name="image" /> is <c>null</c>.</exception>
        public FingerprintTemplate(FingerprintImage image) : this(FeatureExtractor.Extract(image.Matrix, image.Dpi)) { }
        /// <summary>Deserializes fingerprint template from byte array.</summary>
        /// <remarks>
        /// <para>
        /// This constructor reads <see href="https://cbor.io/">CBOR</see>-encoded
        /// template produced by <see cref="ToByteArray()" />
        /// and reconstructs an exact copy of the original fingerprint template.
        /// </para>
        /// <para>
        /// Templates produced by previous versions of SourceAFIS may fail to deserialize correctly.
        /// Applications should re-extract all templates from original images when upgrading SourceAFIS.
        /// </para>
        /// </remarks>
        /// <param name="serialized">Serialized fingerprint template in <see href="https://cbor.io/">CBOR</see> format
        /// produced by <see cref="ToByteArray()" />.</param>
        /// <exception cref="NullReferenceException">Thrown when <paramref name="serialized" /> is <c>null</c>.</exception>
        /// <exception cref="Exception">Thrown when <paramref name="serialized" /> is not in the correct format or it is corrupted.</exception>
        public FingerprintTemplate(byte[] serialized) : this(Deserialize(serialized)) { }

        MutableTemplate Mutable()
        {
            var mutable = new MutableTemplate();
            mutable.Size = Size;
            mutable.Minutiae = (from m in Minutiae select m.Mutable()).ToList();
            return mutable;
        }
        /// <summary>Serializes fingerprint template into byte array.</summary>
        /// <remarks>
        /// <para>
        /// Serialized template can be stored in a database or sent over network.
        /// It can be then deserialized by calling <see cref="FingerprintTemplate(byte[])" /> constructor.
        /// Persisting templates alongside fingerprint images allows applications to start faster,
        /// because template deserialization is more than 100x faster than re-extraction from fingerprint image.
        /// </para>
        /// <para>
        /// Serialized template excludes search structures that <c>FingerprintTemplate</c> keeps to speed up matching.
        /// Serialized template is therefore much smaller than in-memory <c>FingerprintTemplate</c>.
        /// </para>
        /// <para>
        /// Serialization format can change with every SourceAFIS version. There is no backward compatibility of templates.
        /// Applications should preserve raw fingerprint images, so that templates can be re-extracted after SourceAFIS upgrade.
        /// Template format for current version of SourceAFIS is
        /// <see href="https://sourceafis.machinezoo.com/template">documented on SourceAFIS website</see>.
        /// </para>
        /// </remarks>
        /// <returns>Serialized fingerprint template in <see href="https://cbor.io/">CBOR</see> format.</returns>
        /// <seealso cref="FingerprintTemplate(byte[])" />
        /// <seealso href="https://sourceafis.machinezoo.com/template">Template format</seealso>
        public byte[] ToByteArray() { return SerializationUtils.Serialize(new PersistentTemplate(Mutable())); }
        static MutableTemplate Deserialize(byte[] serialized)
        {
            var persistent = SerializationUtils.Deserialize<PersistentTemplate>(serialized);
            persistent.Validate();
            return persistent.Mutable();
        }
    }
}

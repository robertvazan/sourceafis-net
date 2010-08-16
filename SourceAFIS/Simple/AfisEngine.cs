using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using SourceAFIS.Extraction;
using SourceAFIS.Extraction.Templates;
using SourceAFIS.Matching;

namespace SourceAFIS.Simple
{
    /// <summary>
    /// Methods and settings of SourceAFIS fingerprint matching engine.
    /// </summary>
    /// <remarks>
    /// <para>
    /// After setting relevant properties (notably <see cref="Threshold"/>), application
    /// can call one of the three main methods (<see cref="Extract"/>, <see cref="Verify"/>, <see cref="Identify"/>)
    /// to perform template extraction and fingerprint matching.
    /// </para>
    /// <para>
    /// <see cref="AfisEngine"/> objects are thread-safe. <see cref="AfisEngine"/> is lightweight,
    /// but application is encouraged to cache AfisEngine instances anyway. Every
    /// <see cref="AfisEngine"/> method utilizes multiple cores automatically. Applications
    /// that wish to execute several methods of <see cref="AfisEngine"/> in parallel should
    /// create multiple <see cref="AfisEngine"/> objects, perhaphs one per thread.
    /// </para>
    /// </remarks>
    public class AfisEngine
    {
        int DpiValue = 500;
        /// <summary>
        /// Get/set DPI setting.
        /// </summary>
        /// <value>
        /// DPI of images submitted for template extraction. Default is 500.
        /// </value>
        /// <remarks>
        /// <para>
        /// DPI of common optical fingerprint readers is 500. For other types of readers
        /// as well as for high-resolution readers, you might need to change this property
        /// to reflect capabilities of your reader. This value is used only during template
        /// extraction (<see cref="Extract"/>). Matching is not affected, because extraction process rescales all
        /// templates to 500dpi internally.
        /// </para>
        /// <para>
        /// Setting DPI causes extractor to adjust its parameters to the DPI. It therefore
        /// helps with accuracy. Correct DPI also allows matching of fingerprints coming from
        /// different readers. When matching children's fingerprints, it is sometimes useful
        /// to fool the extractor with lower DPI setting to deal with the tiny ridges on
        /// fingers of children.
        /// </para>
        /// </remarks>
        /// <seealso cref="Extract"/>
        public int Dpi
        {
            get { lock (this) return DpiValue; }
            set
            {
                lock (this)
                {
                    if (value < 100 || value > 5000)
                        throw new ArgumentOutOfRangeException();
                    DpiValue = value;
                }
            }
        }
        float ThresholdValue = 12;
        /// <summary>
        /// Get/set similarity score threshold.
        /// </summary>
        /// <value>
        /// Similarity score threshold for making match/non-match decisions.
        /// Default value is rather arbitrarily set to 12.
        /// </value>
        /// <remarks>
        /// <para>
        /// Matching algorithm produces similarity score which is a measure of similarity
        /// between two fingerprints. Applications however need clear match/non-match decisions.
        /// <see cref="Threshold"/> is used to turn similarity score into match/non-match decision.
        /// Similarity score at or above <see cref="Threshold"/> is considered match. Lower score is considered
        /// non-match. This property is used by <see cref="Verify"/> and <see cref="Identify"/> methods to make match decisions.
        /// </para>
        /// <para>
        /// Appropriate <see cref="Threshold"/> is application-specific. Application developoer must adjust this
        /// property to reflect differences in fingerprint readers, population, and application requirements.
        /// Start with default threshold. If there are too many false accepts (SourceAFIS
        /// reports match for fingerprints from two different people), increase the <see cref="Threshold"/>.
        /// If there are too many false rejects (SourceAFIS reports non-match for two fingerprints
        /// of the same person), decrease the <see cref="Threshold"/>. Every application eventually arrives
        /// at some reasonable balance between FAR (false accept ratio) and FRR (false reject ratio).
        /// </para>
        /// </remarks>
        /// <seealso cref="Verify"/>
        /// <seealso cref="Identify"/>
        public float Threshold
        {
            get { lock (this) return ThresholdValue; }
            set
            {
                lock (this)
                {
                    if (value < 0)
                        throw new ArgumentOutOfRangeException();
                    ThresholdValue = value;
                }
            }
        }
        int SkipBestMatchesValue = 0;
        /// <summary>
        /// Get/set number of matches to skip during multi-finger matching.
        /// </summary>
        /// <value>
        /// Number of best matches to skip during multi-finger matching.
        /// Default value is 0 (skipping feature is disabled).
        /// </value>
        /// <remarks>
        /// <para>
        /// When there are multiple <see cref="Fingerprint"/>s per <see cref="Person"/>, SourceAFIS compares
        /// every probe <see cref="Fingerprint"/> to every candidate <see cref="Fingerprint"/> and takes the
        /// best match, the one with highest similarity score. This behavior
        /// improves FRR (false reject rate), because low similarity scores caused
        /// by damaged fingerprints are ignored.
        /// </para>
        /// <para>
        /// When <see cref="SkipBestMatches"/> is non-zero, SourceAFIS ignores specified number
        /// of best matches and takes the next best match. This behavior improves
        /// FAR (false accept rate), because it lowers probability that one accidentally
        /// matching <see cref="Fingerprint"/> pair skews match results. In case there aren't
        /// enough <see cref="Fingerprint"/> pairs to skip, SourceAFIS takes the lowest scoring
        /// pair.
        /// </para>
        /// <para>
        /// <see cref="SkipBestMatches"/> can be used to distribute positive effects of multi-finger
        /// matching evenly between FAR and FRR. It is recommended to set <see cref="SkipBestMatches"/>
        /// to 1 in 3-finger and 4-finger matching and to 2 when there are 5 or more
        /// <see cref="Fingerprint"/>s per <see cref="Person"/>.
        /// </para>
        /// </remarks>
        /// <seealso cref="Verify"/>
        /// <seealso cref="Identify"/>
        public int SkipBestMatches
        {
            get { lock (this) return SkipBestMatchesValue; }
            set
            {
                lock (this)
                {
                    if (value < 0)
                        throw new ArgumentOutOfRangeException();
                    SkipBestMatchesValue = value;
                }
            }
        }

        Extractor Extractor = new Extractor();
        Matcher Matcher = new Matcher();

        /// <summary>
        /// Create new SourceAFIS engine.
        /// </summary>
        public AfisEngine()
        {
            Matcher.Initialize();
        }

        /// <summary>
        /// Extract fingerprint template to be used during matching.
        /// </summary>
        /// <param name="fp">Fingerprint object to use for template extraction.</param>
        /// <remarks>
        /// <para>
        /// <see cref="Extract"/> method takes <see cref="Fingerprint.Image"/> from <paramref name="fp"/> and constructs
        /// fingerprint template that it stores in <paramref name="fp"/>'s <see cref="Fingerprint.Template"/>. This step must
        /// be performed before the <see cref="Fingerprint"/> is used in <see cref="Verify"/> or <see cref="Identify"/> method,
        /// because matching is done on fingerprint templates, not on fingerprint images.
        /// </para>
        /// <para>
        /// Fingerprint image can be discarded after extraction, but it is recommended
        /// to keep it in case the <see cref="Fingerprint.Template"/> needs to be regenerated due to SourceAFIS
        /// upgrade or other reason.
        /// </para>
        /// </remarks>
        /// <seealso cref="Dpi"/>
        public void Extract(Fingerprint fp)
        {
            lock (this)
            {
                TemplateBuilder builder = Extractor.Extract(fp.Image, Dpi);
                fp.Decoded = new SerializedFormat().Export(builder);
            }
        }

        /// <summary>
        /// Compute similarity score between two <see cref="Person"/>s.
        /// </summary>
        /// <param name="probe">First of the two persons to compare.</param>
        /// <param name="candidate">Second of the two persons to compare.</param>
        /// <returns>Similarity score indicating similarity between the two persons or 0 if there is no match.</returns>
        /// <remarks>
        /// <para>
        /// <see cref="Verify"/> method compares two <see cref="Person"/>s, <see cref="Fingerprint"/> by <see cref="Fingerprint"/>, and returns
        /// floating-point similarity score that indicates degree of similarity between
        /// the two <see cref="Person"/>s. If this score falls below <see cref="Threshold"/>, <see cref="Verify"/> method returns zero.
        /// </para>
        /// <para>
        /// <see cref="Fingerprint"/>s passed to this method must have valid <see cref="Fingerprint.Template"/>, i.e. they must
        /// have passed through <see cref="Extract"/> method.
        /// </para>
        /// </remarks>
        /// <seealso cref="Threshold"/>
        /// <seealso cref="SkipBestMatches"/>
        /// <seealso cref="Identify"/>
        public float Verify(Person probe, Person candidate)
        {
            lock (this)
            {
                BestMatchSkipper collector = new BestMatchSkipper(1, SkipBestMatches);
                foreach (Fingerprint probeFp in probe)
                {
                    var candidateTemplates = (from candidateFp in candidate
                                              where IsCompatibleFinger(probeFp.Finger, candidateFp.Finger)
                                              select candidateFp.Decoded).ToList();

                    Matcher.Prepare(probeFp.Decoded);
                    foreach (float score in Matcher.Match(candidateTemplates))
                        collector.AddScore(0, score);
                }

                return ApplyThreshold(collector.GetSkipScore(0));
            }
        }

        /// <summary>
        /// Compares one <see cref="Person"/> against a set of other <see cref="Person"/>s and returns the best match.
        /// </summary>
        /// <param name="probe">Person to look up in the collection.</param>
        /// <param name="candidateSource">Collection of persons that will be searched.</param>
        /// <returns>Best matching <see cref="Person"/> in the collection or <see langword="null"/> if there is no match.</returns>
        /// <remarks>
        /// <para>
        /// Compares probe <see cref="Person"/> to all candidate <see cref="Person"/>s and returns the most similar
        /// candidate. Calling <see cref="Identify"/> is conceptually identical to calling <see cref="Verify"/> in a loop
        /// except that <see cref="Identify"/> is significantly faster than loop of <see cref="Verify"/> calls.
        /// If there is no candidate with score at or above <see cref="Threshold"/>, <see cref="Identify"/> returns <see langword="null"/>.
        /// </para>
        /// <para>
        /// <see cref="Fingerprint"/>s passed to this method must have valid <see cref="Fingerprint.Template"/>, i.e. they must
        /// have passed through <see cref="Extract"/> method.
        /// </para>
        /// </remarks>
        /// <seealso cref="Threshold"/>
        /// <seealso cref="SkipBestMatches"/>
        /// <seealso cref="Verify"/>
        public TCandidate Identify<TCandidate>(Person probe, IEnumerable<TCandidate> candidateSource) where TCandidate : Person
        {
            lock (this)
            {
                TCandidate[] candidates = new List<TCandidate>(candidateSource).ToArray();
                BestMatchSkipper collector = new BestMatchSkipper(candidates.Length, SkipBestMatches);
                foreach (Fingerprint probeFp in probe)
                {
                    List<int> personsByFingerprint = new List<int>();
                    List<Template> candidateTemplates = FlattenHierarchy(candidates, probeFp.Finger, out personsByFingerprint);

                    Matcher.Prepare(probeFp.Decoded);
                    float[] scores = Matcher.Match(candidateTemplates);
                    for (int i = 0; i < scores.Length; ++i)
                        collector.AddScore(personsByFingerprint[i], scores[i]);
                }

                int bestPersonIndex;
                float bestScore = collector.GetBestScore(out bestPersonIndex);
                if (bestPersonIndex >= 0 && bestScore >= Threshold)
                    return candidates[bestPersonIndex];
                else
                    return null;
            }
        }

        bool IsCompatibleFinger(Finger first, Finger second)
        {
            return first == second || first == Finger.Any || second == Finger.Any;
        }

        List<Template> FlattenHierarchy(Person[] persons, Finger finger, out List<int> personIndexes)
        {
            List<Template> templates = new List<Template>();
            personIndexes = new List<int>();
            for (int personIndex = 0; personIndex < persons.Length; ++personIndex)
            {
                Person person = persons[personIndex];
                for (int i = 0; i < person.Count; ++i)
                {
                    Fingerprint fingerprint = person[i];
                    if (IsCompatibleFinger(finger, fingerprint.Finger))
                    {
                        templates.Add(fingerprint.Decoded);
                        personIndexes.Add(personIndex);
                    }
                }
            }
            return templates;
        }

        float ApplyThreshold(float score)
        {
            return score >= Threshold ? score : 0;
        }
    }
}

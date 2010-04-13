using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.Extraction;
using SourceAFIS.Extraction.Templates;
using SourceAFIS.Matching;
using SourceAFIS.Visualization;

namespace SourceAFIS.Simple
{
    /// <summary>
    /// Methods and settings of SourceAFIS fingerprint matching engine.
    /// </summary>
    /// <remarks>
    /// Application should create one AfisEngine object for every thread that
    /// needs SourceAFIS functionality, because this class is not thread-safe.
    /// After setting relevant properties (notably Threshold), application
    /// can call one of the three main methods (Extract, Verify, Identify)
    /// to perform template extraction and fingerprint matching.
    /// </remarks>
    public class AfisEngine
    {
        int DpiValue = 500;
        /// <summary>
        /// Get/set DPI setting.
        /// </summary>
        /// <value>
        /// DPI of images submitted for template extraction.
        /// </value>
        public int Dpi
        {
            get { return DpiValue; }
            set
            {
                if (value < 100 || value > 5000)
                    throw new ArgumentOutOfRangeException();
                DpiValue = value;
            }
        }
        float ThresholdValue = 12;
        /// <summary>
        /// Get/set matching score threshold.
        /// </summary>
        /// <value>
        /// Matching score threshold for making match/non-match decisions.
        /// </value>
        public float Threshold
        {
            get { return ThresholdValue; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException();
                ThresholdValue = value;
            }
        }
        int SkipBestScoreValue = 0;
        /// <summary>
        /// Get/set number of matches to skip during multi-finger matching.
        /// </summary>
        /// <value>
        /// Number of best matches to skip during multi-finger matching.
        /// </value>
        public int SkipBestScore
        {
            get { return SkipBestScoreValue; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException();
                SkipBestScoreValue = value;
            }
        }

        Extractor Extractor = new Extractor();
        Matcher Matcher = new Matcher();

        /// <summary>
        /// Create new SourceAFIS engine.
        /// </summary>
        public AfisEngine() { }

        /// <summary>
        /// Extract fingerprint template to be used during matching.
        /// </summary>
        /// <param name="fp">Fingerprint object to use for template extraction.</param>
        public void Extract(Fingerprint fp)
        {
            byte[,] grayscale = PixelFormat.ToByte(ImageIO.GetPixels(fp.Image));
            TemplateBuilder builder = Extractor.Extract(grayscale, Dpi);
            fp.Decoded = new SerializedFormat().Export(builder);
        }

        /// <summary>
        /// Compute similarity score between two persons.
        /// </summary>
        /// <param name="probe">First of the two persons to compare.</param>
        /// <param name="candidate">Second of the two persons to compare.</param>
        /// <returns>Matching score indicating similarity between the two persons or 0 if there is no match.</returns>
        public float Verify(Person probe, Person candidate)
        {
            BestMatchSkipper collector = new BestMatchSkipper(1, SkipBestScore);
            foreach (Fingerprint probeFp in probe)
            {
                List<Template> candidateTemplates = new List<Template>();
                foreach (Fingerprint candidateFp in candidate)
                    if (IsCompatibleFinger(probeFp.Finger, candidateFp.Finger))
                        candidateTemplates.Add(candidateFp.Decoded);

                Matcher.Prepare(probeFp.Decoded);
                foreach (float score in Matcher.Match(candidateTemplates))
                    collector.AddScore(0, score);
            }

            return ApplyThreshold(collector.GetSkipScore(0));
        }

        /// <summary>
        /// Compare one person against a set of other persons and return best match.
        /// </summary>
        /// <param name="probe">Person to look up in the collection.</param>
        /// <param name="candidateSource">Collection of persons that will be searched.</param>
        /// <returns>Best matching person in the collection or null if there is no match.</returns>
        public Person Identify(Person probe, IEnumerable<Person> candidateSource)
        {
            List<Person> candidates = new List<Person>(candidateSource);
            BestMatchSkipper collector = new BestMatchSkipper(candidates.Count, SkipBestScore);
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

        bool IsCompatibleFinger(Finger first, Finger second)
        {
            return first == second || first == Finger.Any || second == Finger.Any;
        }

        List<Template> FlattenHierarchy(List<Person> persons, Finger finger, out List<int> personIndexes)
        {
            List<Template> templates = new List<Template>();
            personIndexes = new List<int>();
            for (int personIndex = 0; personIndex < persons.Count; ++personIndex)
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

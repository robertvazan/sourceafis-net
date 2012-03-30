package sourceafis.simple;

import java.util.ArrayList;
import java.util.List;
import sourceafis.matching.BestMatchSkipper;
import sourceafis.matching.ParallelMatcher;
import sourceafis.templates.Template;

/**
 * Methods and settings of SourceAFIS fingerprint matching engine.
 * 
 * This class is an entry point to core SourceAFIS functionality. After setting
 * relevant properties (notably {@link #setThreshold threshold}), application
 * can call one of the three main methods ({@link #extract extract},
 * {@link #verify verify}, {@link #identify identify}) to perform template
 * extraction and fingerprint matching.
 * <p>
 * AfisEngine objects are thread-safe, i.e. synchronized. AfisEngine is a
 * lightweight object, but application is encouraged to keep only one global
 * AfisEngine instance anyway. Every AfisEngine method utilizes multiple cores
 * automatically. Applications that wish to execute several methods of
 * AfisEngine in parallel should create multiple AfisEngine objects, perhaps one
 * per thread.
 */
public class AfisEngine
{
    int dpiValue = 500;
    int minMatches = 1;
    float threshold = 12;
    private ParallelMatcher Matcher = new ParallelMatcher();
    /**  
     * Create new SourceAFIS engine.
     */
    public AfisEngine()
    {
    }

    /**
     * Gets DPI of images submitted for template extraction.
     * 
     * See {@link #setDpi setDpi} for explanation of DPI setting. This method
     * just returns current DPI setting.
     * 
     * @return current setting of DPI assumed for all images
     * @see #setDpi setDpi
     * @see #extract extract
     */
    public int getDpi(){
        return dpiValue; 
    }

    /**
     * Sets DPI of images submitted for template extraction.
     * 
     * Default is 500.
     * <p>
     * DPI of common optical fingerprint readers is 500. For other types of
     * readers as well as for high-resolution readers, you might need to change
     * this property to reflect capabilities of your reader. This value is used
     * only during template {@link #extract extraction}. Matching is not
     * affected, because extraction process rescales all templates to 500dpi
     * internally.
     * <p>
     * Setting DPI causes extractor to adjust its parameters to the DPI. It
     * therefore helps with accuracy. Correct DPI also allows matching of
     * fingerprints coming from different readers. When matching children's
     * fingerprints, it is sometimes useful to fool the extractor with lower DPI
     * setting to deal with the tiny ridges on fingers of children.
     * 
     * @param value
     *            new DPI assumed to apply to all processed fingerprint images
     * @see #getDpi getDpi
     * @see #extract extract
     */
    public void setDpi(int value){
                 if (value < 100 || value > 5000)
                    throw new RuntimeException("Value out of range");
                dpiValue = value;
    }

    /**
     * Gets similarity score threshold for making match/non-match decisions.
     * 
     * See {@link #setThreshold setThreshold} for explanation of similarity
     * threshold. This method just returns current threshold value.
     * 
     * @return current similarity threshold value
     * @see #setThreshold setThreshold
     * @see #verify verify
     * @see #identify identify
     */
    public float getThreshold(){
      return threshold; 
    }

    /**
     * Sets similarity score threshold for making match/non-match decisions.
     * 
     * Default value is rather arbitrarily set to 12.
     * <p>
     * Matching algorithm produces similarity score which is a measure of
     * similarity between two fingerprints. Applications however need clear
     * match/non-match decisions. Threshold is used to turn similarity score
     * into match/non-match decision. Similarity score at or above the threshold
     * is considered match. Lower score is considered non-match. This property
     * is used by {@link #verify verify} and {@link #identify identify} methods
     * to make match decisions.
     * <p>
     * Appropriate threshold level is application-specific. Application
     * developer must adjust this property to reflect differences in fingerprint
     * readers, population, and application requirements. Start with default
     * threshold. If there are too many false accepts (SourceAFIS reports match
     * for fingerprints from two different people), increase the threshold. If
     * there are too many false rejects (SourceAFIS reports non-match for two
     * fingerprints of the same person), decrease the threshold. Every
     * application eventually arrives at some reasonable balance between FAR
     * (false accept ratio) and FRR (false reject ratio).
     * 
     * @param value
     *            new value of the similarity threshold
     * @see #getThreshold getThreshold
     * @see #verify verify
     * @see #identify identify
     */
    public void  setThreshold(float value){
             this.threshold = value;
    }
    
    /**
     * Gets minimum number of fingerprints that must match in order for a whole
     * person to match.
     * 
     * See {@link #setMinMatches setMinMatches} for explanation. This method
     * just returns current minimum number of matching fingerprints.
     * 
     * @return current minimum number of matching fingerprints
     * @see #setMinMatches setMinMatches
     * @see #verify verify
     * @see #identify identify
     */
    public int getMinMatches(){
    	return minMatches;
    }

    /**
     * Sets minimum number of fingerprints that must match in order for a whole
     * person to match.
     * 
     * Default value is 1 (person matches if any of its fingerprints matches).
     * <p>
     * When there are multiple {@link Fingerprint}s per {@link Person},
     * SourceAFIS compares every probe {@link Fingerprint} to every candidate
     * {@link Fingerprint} and takes the best match, the one with highest
     * similarity score. This behavior improves FRR (false reject rate), because
     * low similarity scores caused by damaged fingerprints are ignored. This
     * happens when {@code MinMatches} is 1 (default).
     * <p>
     * When {@code MinMatches} is 2 or higher, SourceAFIS compares every probe
     * {@link Fingerprint} to every candidate {@link Fingerprint} and records
     * score for every comparison. It then sorts collected partial scores in
     * descending order and picks score that is on position specified by
     * {@code MinMatches} property, e.g. 2nd score if {@code MinMatches} is 2,
     * 3rd score if {@code MinMatches} is 3, etc. When combined with
     * {@link #setThreshold threshold}, this property essentially specifies how
     * many partial scores must be above {@link #setThreshold threshold} in
     * order for the whole {@link Person} to match. As a special case, when
     * there are too few partial scores (less than value of {@code MinMatches}),
     * SourceAFIS picks the lowest score.
     * <p>
     * {@code MinMatches} is useful in some rare cases where there is
     * significant risk that some fingerprint might match randomly with high
     * score due to a broken template or due to some rarely occuring matcher
     * flaw. In these cases, {@code MinMatches} might improve FAR. This is
     * discouraged practice though. Application developers seeking ways to
     * improve FAR would do much better to increase {@link #setThreshold
     * threshold}. {@link #setThreshold Threshold} can be safely raised to
     * levels where FAR is essentially zero as far as fingerprints are of good
     * quality.
     * 
     * @param value
     *            new minimum number of matching fingerprints
     * @see #getMinMatches getMinMatches
     * @see #verify verify
     * @see #identify identify
     */
    public void setMinMatches(int value){
        this.minMatches = value;
    }
    
    /**
     * Extracts fingerprint template(s) to be used during matching (not
     * implemented in java).
     * 
     * <p>
     * The {@code extract} method takes {@link Fingerprint#image} from every
     * {@link Fingerprint} in {@link Person} parameter and constructs
     * fingerprint template that it stores in {@link Fingerprint#getTemplate
     * template} property of the respective {@link Fingerprint}. This step must
     * be performed before the {@link Person} is used in {@link #verify verify}
     * or {@link #identify identify} method, because matching is done on
     * fingerprint templates, not on fingerprint images.
     * <p>
     * Fingerprint image can be discarded after extraction, but it is
     * recommended to keep it in case the {@link Fingerprint#getTemplate
     * template} needs to be regenerated due to SourceAFIS upgrade or other
     * reason.
     * 
     * @param person
     *            {@link Person} object to use for template extraction
     * @see #setDpi setDpi
     */
    public synchronized void extract(Person person)
    {
    	throw new RuntimeException("Extract not implemted in this java version of SourceAFIS");
    }

    /**
     * Computes similarity score between two {@link Person}s.
     * 
     * The {@code verify} method compares two {@link Person}s,
     * {@link Fingerprint} by {@link Fingerprint}, and returns floating-point
     * similarity score that indicates degree of similarity between the two
     * {@link Person}s. If this score falls below {@link #threshold},
     * {@code verify} method returns zero.
     * <p>
     * {@link Person}s passed to this method must have valid
     * {@link Fingerprint#getTemplate template} for every {@link Fingerprint},
     * i.e. they must have passed through {@link #extract extract} method.
     * 
     * @param probe
     *            first of the two persons to compare
     * @param candidate
     *            second of the two persons to compare
     * @return similarity score indicating similarity between the two persons or
     *         0 if there is no match
     * @see #setThreshold setThreshold
     * @see #identify identify
     */
    public synchronized float verify(Person probe, Person candidate)
    {
      //  lock (this)
       // {
            probe.CheckForNulls(); 
            candidate.CheckForNulls(); 
            BestMatchSkipper collector = new BestMatchSkipper(1, minMatches-1);
            /*Parallel.ForEach(probe.Fingerprints, probeFp =>
                {
                    var candidateTemplates = (from candidateFp in candidate.Fingerprints
                                              where IsCompatibleFinger(probeFp.Finger, candidateFp.Finger)
                                              select candidateFp.Decoded).ToList();

                    ParallelMatcher.PreparedProbe probeIndex = Matcher.Prepare(probeFp.Decoded);
                    float[] scores = Matcher.Match(probeIndex, candidateTemplates);

                    lock (collector)
                        foreach (float score in scores)
                            collector.AddScore(0, score);
                });
             */
            for(Fingerprint fp:probe.getFingerprints()){
            	
            	List<Template> candidateTemplates=new ArrayList<Template>();
            	List<Fingerprint> candidatefp =candidate.getFingerprints();
            	
            	for(Fingerprint cfp:candidatefp){
            		if(isCompatibleFinger(fp.getFinger(),cfp.getFinger())){
            			candidateTemplates.add(cfp.decoded);
            		}
            	}
            	
            	ParallelMatcher.PreparedProbe probeIndex = Matcher.prepare(fp.decoded);
            	float[] scores = Matcher.Match(probeIndex, candidateTemplates);
            	
                for (float score :scores){
                	collector.addScore(0, score);
                }
              }
            
            return applyThreshold(collector.getSkipScore(0));
        //}
    }

    /**
     * Compares probe {@link Person} to all candidate {@link Person}s and
     * returns the most similar candidates.
     * 
     * 
     * Calling {@code identify} is conceptually identical to calling
     * {@link #verify verify} in a loop except that {@code identify} is
     * significantly faster than loop of {@link #verify verify} calls. If there
     * is no candidate with score at or above {@link #setThreshold threshold},
     * {@code identify} returns empty collection.
     * <p>
     * Most applications need only the best match, which can be obtained by
     * calling
     * {@link com.google.common.collect.Iterables#getFirst(java.lang.Iterable, T)
     * Iterables.getFirst} method from Guava library. Pass the returned
     * collection as its first parameter and null as its second parameter.
     * Matching score for every returned {@link Person} can be obtained by
     * calling {@link #verify verify} on probe {@link Person} and the matching
     * {@link Person}.
     * <p>
     * {@link Person}s passed to this method must have valid
     * {@link Fingerprint#getTemplate template} for every {@link Fingerprint},
     * i.e. they must have passed through {@link #extract extract} method.
     * 
     * @param probe
     *            person to look up in the collection
     * @param candidates
     *            collection of persons that will be searched
     * @return All matching {@link Person} objects in the collection or an empty
     *         collection if there is no match. Results are sorted by score in
     *         descending order. If you need only one best match, call
     *         {@link com.google.common.collect.Iterables#getFirst(java.lang.Iterable, T)
     *         Iterables.getFirst} method from Guava library. Pass the returned
     *         collection as its first parameter and null as its second
     *         parameter.
     * @see #setThreshold setThreshold
     * @see #verify verify
     */
    public synchronized Iterable<Person> identify(Person probe, Iterable<Person> candidates)
    {
       	    probe.CheckForNulls();
       	    ArrayList<Person> candidateList = new ArrayList<Person>();
       	    for (Person candidate : candidates)
       	    	candidateList.add(candidate);
            Person[] candidateArray = candidateList.toArray(new Person[candidateList.size()]);
            BestMatchSkipper.PersonsSkipScore[] results;
            
            BestMatchSkipper collector = new BestMatchSkipper(candidateArray.length, minMatches - 1);
            
            for(Fingerprint fp:probe.getFingerprints()){
                List<Integer> personsByFingerprint = new ArrayList<Integer>();
                List<Template> candidateTemplates = flattenHierarchy(candidateArray, fp.getFinger(),personsByFingerprint);
                ParallelMatcher.PreparedProbe probeIndex = Matcher.prepare(fp.decoded);
                float[] scores = Matcher.Match(probeIndex, candidateTemplates);
                for (int i = 0; i < scores.length; ++i){
                    collector.addScore(personsByFingerprint.get(i), scores[i]);
                }

            }
            results = collector.getSortedScores();
            return getMatchingCandidates(candidateArray, results);
            /*Parallel.ForEach(probe.Fingerprints, probeFp =>
                {
                    List<int> personsByFingerprint = new List<int>();
                    List<Template> candidateTemplates = FlattenHierarchy(candidateArray, probeFp.Finger, out personsByFingerprint);

                    ParallelMatcher.PreparedProbe probeIndex = Matcher.Prepare(probeFp.Decoded);
                    float[] scores = Matcher.Match(probeIndex, candidateTemplates);

                    lock (collector)
                        for (int i = 0; i < scores.Length; ++i)
                            collector.AddScore(personsByFingerprint[i], scores[i]);
                });
            */  
        
    }
    
    private boolean isCompatibleFinger(Finger first, Finger second)
    {
        return first == second || first == Finger.ANY || second == Finger.ANY;
    }
    
    private List<Template> flattenHierarchy(Person[] persons, Finger finger, List<Integer> personIndexes)
    {
        List<Template> templates = new ArrayList<Template>();
       // personIndexes = new ArrayList<Integer>();
        for (int personIndex = 0; personIndex < persons.length; ++personIndex)
        {
            Person person = persons[personIndex];
            person.CheckForNulls();
            for (int i = 0; i < person.getFingerprints().size(); ++i)
            {
                Fingerprint fingerprint = person.getFingerprints().get(i);
                if (isCompatibleFinger(finger, fingerprint.getFinger()))
                {
                    templates.add(fingerprint.decoded);
                    personIndexes.add(personIndex);
                }
            }
        }
        return templates;
    }
    private List<Person> getMatchingCandidates(Person[] candidateArray, BestMatchSkipper.PersonsSkipScore[] results)
    {
        /*foreach (var match in results)
            if (match.Score >= Threshold)
                yield return candidateArray[match.Person];*/
    	List<Person> matches=new ArrayList<Person>();
    	for(BestMatchSkipper.PersonsSkipScore match:results){
              if(match.score >= threshold) matches.add(candidateArray[match.person]);		
    	}
    	return matches;
    }
    private float applyThreshold(float score)
    {
        return score >= getThreshold() ? score : 0;
    }
}
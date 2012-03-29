package sourceafis.simple;

import java.util.ArrayList;
import java.util.List;
import sourceafis.matching.BestMatchSkipper;
import sourceafis.matching.ParallelMatcher;
import sourceafis.templates.Template;

/**
 * Methods and settings of SourceAFIS fingerprint matching engine. 
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
    * DPI of images submitted for template extraction. Default is 500.
    *  
    * <p>
    * DPI of common optical fingerprint readers is 500. For other types of readers
    * as well as for high-resolution readers, you might need to change this property
    * to reflect capabilities of your reader. This value is used only during template
    * extraction {@link #extract}. Matching is not affected, because extraction process rescales all
    * templates to 500dpi internally.
    * </p>
    * <p>
    * Setting DPI causes extractor to adjust its parameters to the DPI. It therefore
    * helps with accuracy. Correct DPI also allows matching of fingerprints coming from
    * different readers. When matching children's fingerprints, it is sometimes useful
    * to fool the extractor with lower DPI setting to deal with the tiny ridges on
    * fingers of children.
    * </p>
    * @see #extract
    * 
    */
    public int getDpi(){
        return dpiValue; 
    }
    public void setDpi(int value){
                 if (value < 100 || value > 5000)
                    throw new RuntimeException("Value out of range");
                dpiValue = value;
    }

   /**  
    * imilarity score threshold.
    * 
    * <p>
    * Similarity score threshold for making match/non-match decisions.
    * Default value is rather arbitrarily set to 12.
    * </p>
    * <p>
    * Matching algorithm produces similarity score which is a measure of similarity
    * between two fingerprints. Applications however need clear match/non-match decisions.
    * {@link #threshold} is used to turn similarity score into match/non-match decision.
    * Similarity score at or above {@link #threshold} is considered match. Lower score is considered
    * non-match. This property is used by <{@link #verify} and {@link #identify} methods to make match decisions.
    * </p>
    * <p>
    * Appropriate {@link #threshold} is application-specific. Application developoer must adjust this
    * property to reflect differences in fingerprint readers, population, and application requirements.
    * Start with default threshold. If there are too many false accepts (SourceAFIS
    * reports match for fingerprints from two different people), increase the {@link #threshold}.
    * If there are too many false rejects (SourceAFIS reports non-match for two fingerprints
    * of the same person), decrease the {@link #threshold}. Every application eventually arrives
    * at some reasonable balance between FAR (false accept ratio) and FRR (false reject ratio).
    * </p>
    * @see #verify
    * @see #identify
    */
    public float getThreshold(){
      return threshold; 
    }
    public void  setThreshold(float value){
             this.threshold = value;
    }
    
    public int getMinMatches(){
    	return minMatches;
    }
    public void setMinMaches(int value){
        this.minMatches = value;
    }
    
   /** 
    *  Extract fingerprint template(s) to be used during matching.
    * 
    * {@link Person} object to use for template extraction.
    * <p>
    * {@link #extract} method takes {@link Fingerprint#image} from every {@link Fingerprint}
    * in <paramref name="person"/> and constructs fingerprint template that it stores in
    * {@link Fingerprint#getTemplate()} property of the respective {@link Fingerprint}. This step must
    * be performed before the {@link Person} is used in {@link #verify} or {@link #identify} method,
    * because matching is done on fingerprint templates, not on fingerprint images.
    * </p>
    * <p>
    * Fingerprint image can be discarded after extraction, but it is recommended
    * to keep it in case the {@link Fingerprint#getTemplate()} needs to be regenerated due to SourceAFIS
    * upgrade or other reason.
    * </p>
    * @param person
    * 
    */
    public synchronized void extract(Person person)
    {
    	throw new RuntimeException("Extract not implemted in this java version of SourceAFIS");
    }

    /**
    *  Compute similarity score between two {@link Person}s.
    * <p>
    * {@link #verify} method compares two {@link Person}s, {@link Fingerprint} by {@link Fingerprint}, and returns
    * floating-point similarity score that indicates degree of similarity between
    * the two {@link Person}s. If this score falls below {@link #threshold}, {@link #verify} method returns zero.
    * </p>
    * <p>
    * {@link Person}s passed to this method must have valid {@link Fingerprint#getTemplate()}
    * for every {@link Fingerprint}, i.e. they must have passed through {@link #extract} method.
    * </p>
    * @param probe First of the two persons to compare. 
    * @param candidate Second of the two persons to compare.
    * @return float Similarity score indicating similarity between the two persons or 0 if there is no match. 
    * @see #threshold 
    * @see #identify 
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
     *  Compares one {@link Person} against a set of other {@link Person}s and returns the best match.
     *  
     * <p>
     *  Compares probe {@link Person} to all candidate {@link Person}s and returns the most similar
     *  candidate. Calling {@link #identify} is conceptually identical to calling {@link #verify} in a loop
     *  except that {@link #identify} is significantly faster than loop of {@link #verify} calls.
     *  If there is no candidate with score at or above {@link #threshold}, {@link #identify} returns null.
     *  </p>
     * <p>
     * {@link Person}s passed to this method must have valid {@link Fingerprint#getTemplate()} 
     * for every {@link Fingerprint}, i.e. they must have passed through {@link #extract} method.
     * </p>
     *
     * @param probe Person to look up in the collection.
     * @param candidates  Collection of persons that will be searched. 
     * @return Best matching {@link Person} in the collection or "null" if there is no match.
     * @see #threshold
     * @see #verify 
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
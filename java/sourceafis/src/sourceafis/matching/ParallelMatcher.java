package sourceafis.matching;
import java.util.List;
import sourceafis.extraction.templates.Template;
import sourceafis.general.DetailLogger;
import sourceafis.general.Queue;
import sourceafis.matching.minutia.MinutiaMatcher;
import sourceafis.meta.Nested;
public class ParallelMatcher
{
    @Nested
    public MinutiaMatcher MinutiaMatcher = new MinutiaMatcher();

    public class PreparedProbe
    {
        public ProbeIndex ProbeIndex = new ProbeIndex();
    }

    Queue<MinutiaMatcher> Matchers = new Queue<MinutiaMatcher>();
  /*
   * Review the code, it is creaating new matcher everytime
   */
    MinutiaMatcher DequeueMatcher()
    {
        MinutiaMatcher matcher = null;
       // lock (Matchers)
        synchronized(Matchers){
           // if (Matchers.Count > 0)
          if (!Matchers.isEmpty())
                 matcher = Matchers.dequeue();
          // Can be implemented later when using real parallel
           if (matcher == null) {
         //     matcher = ParameterSet.ClonePrototype(MinutiaMatcher);
        	   matcher = new MinutiaMatcher();
        	   DetailLogger.copyHooks(MinutiaMatcher, matcher);
           }
        }
        return matcher;
    }

    MinutiaMatcher DequeueMatcher(PreparedProbe probe)
    {
        MinutiaMatcher matcher = DequeueMatcher();
        matcher.SelectProbe(probe.ProbeIndex);
        return matcher;
    }

    void EnqueueMatcher(MinutiaMatcher matcher)
    {
        //lock (Matchers)
    	synchronized(Matchers){
             Matchers.enqueue(matcher);
    	}
    }

    public PreparedProbe Prepare(Template probe)
    {
        PreparedProbe prepared = new PreparedProbe();
        MinutiaMatcher matcher = DequeueMatcher();
        try
        {
            matcher.BuildIndex(probe, prepared.ProbeIndex);
        }
        finally
        {
            EnqueueMatcher(matcher);
        }
        return prepared;
    }

    public float[] Match(PreparedProbe probe, List<Template> candidates)
    {
        float[] scores = new float[candidates.size()];
        // No Parallel , use Sequencial for now
        for(int x=0;x<scores.length;x++){
          MinutiaMatcher matcher= DequeueMatcher(probe);
          scores[x]=matcher.Match(candidates.get(x));
          EnqueueMatcher(matcher);
        }
        
        /*
        Parallel.For(0, candidates.Count,
            () => DequeueMatcher(probe),
            (i, state, matcher) => { scores[i] = matcher.Match(candidates[i]); 
            return matcher;
            },
            (matcher) => { EnqueueMatcher(matcher); });
         */  
        return scores;
    }
}


package sourceafis.matching.minutia;

import java.util.ArrayList;
import sourceafis.extraction.templates.Template;

public class ExhaustiveRootSelector {
	public ArrayList<MinutiaPair> GetRoots(Template probeTemplate, Template candidateTemplate)
    {
	 	ArrayList<MinutiaPair> list=new ArrayList<MinutiaPair>();
        for (int probe = 0; probe < probeTemplate.Minutiae.length; ++probe)
           for (int candidate = 0; candidate < candidateTemplate.Minutiae.length; ++candidate)
           //     yield return new MinutiaPair(probe, candidate);
        	   list.add(new MinutiaPair(probe, candidate)); 	 
         return list;
    }
}

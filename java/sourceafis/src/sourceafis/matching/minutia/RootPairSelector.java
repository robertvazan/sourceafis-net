package sourceafis.matching.minutia;

import java.util.ArrayList;
import sourceafis.extraction.templates.Template;

public class RootPairSelector {
	public ArrayList<MinutiaPair> GetRoots(Template probeTemplate, Template candidateTemplate)
    {
	 	ArrayList<MinutiaPair> list=new ArrayList<MinutiaPair>();
        for (int probe = 0; probe < probeTemplate.Minutiae.length; ++probe)
           for (int candidate = 0; candidate < candidateTemplate.Minutiae.length; ++candidate) {
               int mixedProbe = (probe + candidate) % probeTemplate.Minutiae.length;
        	   list.add(new MinutiaPair(mixedProbe, candidate));
           }
         return list;
    }
}

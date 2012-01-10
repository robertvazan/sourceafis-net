package sourceafis.matching.minutia;

import java.util.Iterator;
import java.util.NoSuchElementException;

import sourceafis.extraction.templates.Template;
import sourceafis.general.DetailLogger;

public class RootPairSelector implements Iterable<MinutiaPair>, Iterator<MinutiaPair> {
	private Template probeTemplate;
	private Template candidateTemplate;
	private int probe;
	private int candidate;

    public DetailLogger.Hook logger = DetailLogger.off;

	public Iterable<MinutiaPair> getRoots(Template p, Template c) {
		probeTemplate = p;
		candidateTemplate = c;
		probe = 0;
		candidate = 0;
		return this;
	}
	
	public Iterator<MinutiaPair> iterator() { return this; }
	public void remove() { throw new UnsupportedOperationException(); }

	public boolean hasNext() {
		return probe < probeTemplate.Minutiae.length && candidate < candidateTemplate.Minutiae.length;
	}

	public MinutiaPair next() {
		if (!hasNext())
			throw new NoSuchElementException();

		int mixedProbe = (probe + candidate) % probeTemplate.Minutiae.length;
		MinutiaPair result = new MinutiaPair(mixedProbe, candidate);
		
		++candidate;
		if (candidate >= candidateTemplate.Minutiae.length) {
			++probe;
			candidate = 0;
		}
		
		logger.log(result);
		return result;
	}
}

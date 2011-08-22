package sourceafis.matching.minutia;

import java.util.Enumeration;

import sourceafis.extraction.templates.Template;
 
public interface IRootSelector{
       //IEnumerable<MinutiaPair> GetRoots(Template probe, Template candidate);
	   Enumeration<MinutiaPair> GetRoots(Template probe, Template candidate);
}
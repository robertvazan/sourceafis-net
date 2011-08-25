package sourceafis.simple;

import java.io.File;
import java.io.IOException;
import java.util.Arrays;

import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;
import javax.xml.parsers.ParserConfigurationException;

import junit.framework.Assert;

import org.junit.Test;
import org.w3c.dom.Document;
import org.w3c.dom.Element;
import org.w3c.dom.Node;
import org.w3c.dom.NodeList;
import org.xml.sax.SAXException;

import sourceafis.extraction.templates.CompactFormat;
import sourceafis.extraction.templates.Template;
import sourceafis.general.DetailLogger;
import sourceafis.matching.ParallelMatcher;
import sourceafis.matching.minutia.MinutiaPair;
import sourceafis.meta.ObjectTree;
import sun.misc.BASE64Decoder;


public class ConsistencyTest {

	File folderJavaProject = new File(System.getProperty("user.dir"));
	File folderRoot = new File(new File(folderJavaProject, ".."), "..");
	File folderJavaTestData = new File(new File(folderRoot, "Data"), "JavaTestData");

	byte[] findTemplateBytes(NodeList templates, String path) throws IOException {
		for (int i = 0; i < templates.getLength(); ++i) {
			if (templates.item(i).getNodeType() == Node.ELEMENT_NODE) {
				Element template = (Element)templates.item(i);
				if (template.getAttribute("image-path").equals(path)) {
					String base64 = template.getAttribute("compact");
					return new BASE64Decoder().decodeBuffer(base64);
				}
			}
		}
		Assert.fail();
		return null;
	}
	
	Person findPerson(NodeList templates, String path) throws IOException {
		Fingerprint fp = new Fingerprint();
		fp.setTemplate(findTemplateBytes(templates, path));
		return new Person(new Fingerprint[] { fp });
	}
	
	Template findTemplate(NodeList templates, String path) throws IOException {
		return new Template(new CompactFormat().Import(findTemplateBytes(templates, path)));
	}
	
	@Test
	public void testScore()
	throws IOException, SAXException, ParserConfigurationException {
		DocumentBuilder docBuilder = DocumentBuilderFactory.newInstance().newDocumentBuilder();
		Document templateDoc = docBuilder.parse(new File(folderJavaTestData, "templates.xml")); 
		NodeList templates = templateDoc.getDocumentElement().getElementsByTagName("template");
		Document scoreDoc = docBuilder.parse(new File(folderJavaTestData, "score.xml")); 
		NodeList scores = scoreDoc.getDocumentElement().getElementsByTagName("pair");
		AfisEngine afis = new AfisEngine();
		
		for (int i = 0; i < scores.getLength(); ++i) {
			if (scores.item(i).getNodeType() == Node.ELEMENT_NODE) {
				Element score = (Element)scores.item(i);
				String probePath = score.getAttribute("probe");
				Person probe = findPerson(templates, probePath);
				String candidatePath = score.getAttribute("candidate");
				Person candidate = findPerson(templates, candidatePath);
				double javaScore = Math.round((double)afis.Verify(probe, candidate) * 10000) / 10000;
				double csharpScore = Double.parseDouble(score.getAttribute("score"));
				Assert.assertEquals("probe: " + probePath + ", candidate: " + candidatePath
						+ ", C# score: " + csharpScore + ", java score: " + javaScore,
						csharpScore, javaScore);
			}
		}
	}
	
	@Test
	public void testMatcherLog()
	throws IOException, SAXException, ParserConfigurationException {
		DocumentBuilder docBuilder = DocumentBuilderFactory.newInstance().newDocumentBuilder();
		Document templateDoc = docBuilder.parse(new File(folderJavaTestData, "templates.xml")); 
		NodeList templates = templateDoc.getDocumentElement().getElementsByTagName("template");
		Element csLog = docBuilder.parse(new File(folderJavaTestData, "matcher.xml")).getDocumentElement();
		Template probe = findTemplate(templates, csLog.getAttribute("probe")); 
		Template candidate = findTemplate(templates, csLog.getAttribute("probe"));
		
		ParallelMatcher matcher = new ParallelMatcher();
		DetailLogger logger = new DetailLogger();
		logger.attach(new ObjectTree(matcher));
		ParallelMatcher.PreparedProbe prepared = matcher.Prepare(probe);
		matcher.Match(prepared, Arrays.asList(candidate));
		DetailLogger.LogData log = logger.popLog();
		
		MinutiaPair rootPair = (MinutiaPair)log.retrieve("MinutiaMatcher.root");
		if (rootPair != null) {
			Assert.assertEquals(Integer.parseInt(csLog.getAttribute("root-pair-probe")), rootPair.Probe);
			Assert.assertEquals(Integer.parseInt(csLog.getAttribute("root-pair-candidate")), rootPair.Candidate);
		} else
			Assert.assertFalse(csLog.hasAttribute("root-pair-probe"));
	}
}

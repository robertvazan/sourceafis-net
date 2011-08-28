package sourceafis.simple;

import static java.lang.Double.parseDouble;
import static java.lang.Integer.parseInt;
import static junit.framework.Assert.assertEquals;
import static junit.framework.Assert.fail;

import java.io.File;
import java.io.IOException;
import java.util.Arrays;
import java.util.HashSet;

import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;
import javax.xml.parsers.ParserConfigurationException;

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
import sourceafis.meta.ParameterSet;
import sourceafis.meta.ParameterValue;
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
		fail();
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
				double csharpScore = parseDouble(score.getAttribute("score"));
				assertEquals("probe: " + probePath + ", candidate: " + candidatePath
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
		Template candidate = findTemplate(templates, csLog.getAttribute("candidate"));
		
		ParallelMatcher matcher = new ParallelMatcher();
		DetailLogger logger = new DetailLogger();
		logger.attach(new ObjectTree(matcher));
		ParallelMatcher.PreparedProbe prepared = matcher.Prepare(probe);
		matcher.Match(prepared, Arrays.asList(candidate));
		DetailLogger.LogData log = logger.popLog();
		
		NodeList csRoots = csLog.getElementsByTagName("root");
		assertEquals(csRoots.getLength(), log.count("MinutiaMatcher.RootSelector"));
		for (int i = 0; i < csRoots.getLength(); ++i) {
			Element csRoot = (Element)csRoots.item(i);
			MinutiaPair root = (MinutiaPair)log.retrieve("MinutiaMatcher.RootSelector", i);
			assertEquals("offset: " + i, parseInt(csRoot.getAttribute("probe")), root.Probe);
			assertEquals("offset: " + i, parseInt(csRoot.getAttribute("candidate")), root.Candidate);
		}
		assertEquals(parseInt(csLog.getAttribute("best-root")), log.retrieve("MinutiaMatcher.BestRoot"));
	}
	
	@Test
	public void testParameters()
	throws IOException, SAXException, ParserConfigurationException {
		DocumentBuilder docBuilder = DocumentBuilderFactory.newInstance().newDocumentBuilder();
		Document csParamsDoc = docBuilder.parse(new File(folderJavaTestData, "parameters.xml"));
		NodeList csParams = csParamsDoc.getDocumentElement().getElementsByTagName("param");
		
		ParallelMatcher matcher = new ParallelMatcher();
		ParameterSet jParams = new ParameterSet(new ObjectTree(matcher));
		
		HashSet<String> csPaths = new HashSet<String>();
		for (int i = 0; i < csParams.getLength(); ++i)
			csPaths.add(((Element)csParams.item(i)).getAttribute("path").toLowerCase());
		HashSet<String> jPaths = new HashSet<String>();
		for (ParameterValue parameter : jParams.getAllParameters())
			jPaths.add(parameter.fieldPathNoCase);
		assertEquals(csPaths, jPaths);
		
		for (ParameterValue jParam : jParams.getAllParameters()) {
			double csValue = 0;
			for (int i = 0; i < csParams.getLength(); ++i)
				if(((Element)csParams.item(i)).getAttribute("path").toLowerCase().equals(jParam.fieldPathNoCase))
					csValue = parseDouble(((Element)csParams.item(i)).getAttribute("value"));
			assertEquals("param: " + jParam.fieldPath, csValue, jParam.value);
		}
	}
}

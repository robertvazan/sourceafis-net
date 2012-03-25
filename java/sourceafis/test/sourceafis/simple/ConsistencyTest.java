package sourceafis.simple;

import static java.lang.Double.parseDouble;
import static java.lang.Float.parseFloat;
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

import org.apache.commons.codec.binary.Base64;
import org.junit.Test;
import org.w3c.dom.Document;
import org.w3c.dom.Element;
import org.w3c.dom.Node;
import org.w3c.dom.NodeList;
import org.xml.sax.SAXException;

import sourceafis.general.DetailLogger;
import sourceafis.matching.ParallelMatcher;
import sourceafis.matching.minutia.EdgeTable;
import sourceafis.matching.minutia.MinutiaPair;
import sourceafis.matching.minutia.MinutiaPairing;
import sourceafis.matching.minutia.NeighborEdge;
import sourceafis.meta.ObjectTree;
import sourceafis.meta.ParameterSet;
import sourceafis.meta.ParameterValue;
import sourceafis.templates.CompactFormat;
import sourceafis.templates.Template;


public class ConsistencyTest {

	File folderJavaProject = new File(System.getProperty("user.dir"));
	File folderRoot = new File(new File(folderJavaProject, ".."), "..");
	File folderJavaTestData = new File(new File(folderRoot, "Data"), "JavaTestData");

	byte[] findTemplateBytes(NodeList templates, String path) throws IOException {
		for (int i = 0; i < templates.getLength(); ++i) {
			if (templates.item(i).getNodeType() == Node.ELEMENT_NODE) {
				Element template = (Element)templates.item(i);
				if (template.getAttribute("image-path").equals(path))
					return Base64.decodeBase64(template.getAttribute("compact"));
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
		return new Template(new CompactFormat().importTemplate(findTemplateBytes(templates, path)));
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
		afis.setThreshold(0);
		
		for (int i = 0; i < scores.getLength(); ++i) {
			if (scores.item(i).getNodeType() == Node.ELEMENT_NODE) {
				Element score = (Element)scores.item(i);
				String probePath = score.getAttribute("probe");
				Person probe = findPerson(templates, probePath);
				String candidatePath = score.getAttribute("candidate");
				Person candidate = findPerson(templates, candidatePath);
				float javaScore = afis.verify(probe, candidate);
				float csharpScore = parseFloat(score.getAttribute("score"));
				assertEquals("probe: " + probePath + ", candidate: " + candidatePath
						+ ", C# score: " + csharpScore + ", java score: " + javaScore,
						csharpScore, javaScore, 0.0001);
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
		ParallelMatcher.PreparedProbe prepared = matcher.prepare(probe);
		float[] totalScore = matcher.Match(prepared, Arrays.asList(candidate));
		DetailLogger.LogData log = logger.popLog();
		
		NodeList csEdgeTable = csLog.getElementsByTagName("edge-list");
		EdgeTable edgeTable = (EdgeTable)log.retrieve("minutiaMatcher.edgeTablePrototype");
		assertEquals(csEdgeTable.getLength(), edgeTable.Table.length);
		for (int i = 0; i < edgeTable.Table.length; ++i) {
			NodeList csEdgeList = ((Element)csEdgeTable.item(i)).getElementsByTagName("edge");
			NeighborEdge[] edgeList = edgeTable.Table[i];
			assertEquals("from: " + i, csEdgeList.getLength(), edgeList.length);
			for (int j = 0; j < edgeList.length; ++j) {
				Element csEdge = (Element)csEdgeList.item(j);
				String atEdge = "from: " + i + ", edge: " + j;
				assertEquals(atEdge, parseInt(csEdge.getAttribute("length")), edgeList[j].edge.length);
				assertEquals(atEdge, parseInt(csEdge.getAttribute("neighbor")), edgeList[j].neighbor);
			}
		}

		NodeList csRoots = csLog.getElementsByTagName("root");
		assertEquals(csRoots.getLength(), log.count("minutiaMatcher.rootSelector"));
		for (int i = 0; i < csRoots.getLength(); ++i) {
			Element csRoot = (Element)csRoots.item(i);
			MinutiaPair root = (MinutiaPair)log.retrieve("minutiaMatcher.rootSelector", i);
			float score = (Float)log.retrieve("minutiaMatcher.matchScoring", i);
			String offset = "offset: " + i; 
			assertEquals(offset, parseInt(csRoot.getAttribute("probe")), root.probe);
			assertEquals(offset, parseInt(csRoot.getAttribute("candidate")), root.candidate);
			NodeList csPairs = csRoot.getElementsByTagName("pair");
			MinutiaPairing pairing = (MinutiaPairing)log.retrieve("minutiaMatcher.pairing", i);
			assertEquals(csPairs.getLength(), pairing.getCount());
			for (int j = 0; j < pairing.getCount(); ++j) {
				String atPair = offset + ", pair: " + j;
				Element csPair = (Element)csPairs.item(j);
				assertEquals(atPair, parseInt(csPair.getAttribute("probe")), pairing.getPair(j).pair.probe);
				assertEquals(atPair, parseInt(csPair.getAttribute("candidate")), pairing.getPair(j).pair.candidate);
				assertEquals(atPair, parseInt(csPair.getAttribute("support")),pairing.getPair(j).supportingEdges);

			}
			assertEquals(offset, parseFloat(csRoot.getAttribute("score")), score, 0.0001);
		}
		assertEquals(parseInt(csLog.getAttribute("best-root")), log.retrieve("minutiaMatcher.BestRootIndex"));
		assertEquals(parseFloat(csLog.getAttribute("score")), totalScore[0], 0.0001);
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

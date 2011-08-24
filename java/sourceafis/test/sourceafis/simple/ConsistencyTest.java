package sourceafis.simple;

import java.io.File;
import java.io.IOException;

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

import sun.misc.BASE64Decoder;


public class ConsistencyTest {

	File folderJavaProject = new File(System.getProperty("user.dir"));
	File folderRoot = new File(new File(folderJavaProject, ".."), "..");
	File folderJavaTestData = new File(new File(folderRoot, "Data"), "JavaTestData");

	Person findTemplate(NodeList templates, String path) throws IOException {
		for (int i = 0; i < templates.getLength(); ++i) {
			if (templates.item(i).getNodeType() == Node.ELEMENT_NODE) {
				Element template = (Element)templates.item(i);
				String base64 = template.getAttribute("compact");
				byte[] data = new BASE64Decoder().decodeBuffer(base64);
				Fingerprint fp = new Fingerprint();
				fp.setTemplate(data);
				return new Person(new Fingerprint[] { fp });
			}
		}
		Assert.fail();
		return null;
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
				Person probe = findTemplate(templates, probePath);
				String candidatePath = score.getAttribute("candidate");
				Person candidate = findTemplate(templates, candidatePath);
				double javaScore = Math.round((double)afis.Verify(probe, candidate) * 10000) / 10000;
				double csharpScore = Double.parseDouble(score.getAttribute("score"));
				Assert.assertEquals("probe: " + probePath + ", candidate: " + candidatePath
						+ ", C# score: " + csharpScore + ", java score: " + javaScore,
						csharpScore, javaScore);
			}
		}
	}
}

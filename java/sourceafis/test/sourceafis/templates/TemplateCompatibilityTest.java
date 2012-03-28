package sourceafis.templates;

import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.IOException;
import java.util.ArrayList;
import java.util.Arrays;

import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;
import javax.xml.parsers.ParserConfigurationException;

import org.apache.commons.codec.binary.Base64;
import org.custommonkey.xmlunit.Diff;
import org.custommonkey.xmlunit.XMLAssert;
import org.custommonkey.xmlunit.XMLUnit;
import org.junit.Assert;
import org.junit.Test;
import org.w3c.dom.Document;
import org.w3c.dom.Element;
import org.w3c.dom.NodeList;
import org.xml.sax.SAXException;

import sourceafis.general.TestSettings;
import sourceafis.simple.Fingerprint;
import static junit.framework.Assert.assertTrue;

public class TemplateCompatibilityTest {
	static abstract class TemplateLoader {
		public static TemplateLoader natives = new TemplateLoader() {
			public File getPath() { return TestSettings.fileTemplates; }
		};
		
		public static TemplateLoader iso = new TemplateLoader() {
			public File getPath() { return TestSettings.fileIsoTemplates; }
			@Override public TemplateGroup loadTemplate(Element xml) {
				TemplateGroup result = super.loadTemplate(xml);
				result.originalIso = Base64.decodeBase64(xml.getAttribute("iso-original"));
				return result;
			}
		};
		
		public abstract File getPath();
		
		public TemplateGroup loadTemplate(Element xml) {
			TemplateGroup result = new TemplateGroup();
			result.compact = Base64.decodeBase64(xml.getAttribute("compact"));
			result.iso = Base64.decodeBase64(xml.getAttribute("iso"));
			result.xml = (Element)xml.getElementsByTagName("FingerprintTemplate").item(0);
			return result;
		}
		
		public ArrayList<TemplateGroup> loadTemplates()
				throws IOException, SAXException, ParserConfigurationException {
			ArrayList<TemplateGroup> result = new ArrayList<TemplateGroup>();
			DocumentBuilder docBuilder = DocumentBuilderFactory.newInstance().newDocumentBuilder();
			Document templateDoc = docBuilder.parse(getPath()); 
			NodeList templates = templateDoc.getDocumentElement().getElementsByTagName("template");
			for (int i = 0; i < templates.getLength(); ++i)
				result.add(loadTemplate((Element)templates.item(i)));
			return result;
		}
	}
	
	static class TemplateGroup {
		public byte[] compact;
		public byte[] iso;
		public Element xml;
		public byte[] originalIso;
		
		public static TemplateGroup export(Fingerprint fp) {
			TemplateGroup result = new TemplateGroup();
			result.compact = fp.getTemplate();
			result.iso = fp.getIsoTemplate();
			result.xml = fp.getXmlTemplate();
			return result;
		}
		
		public static Document toDocument(Element element)
				throws ParserConfigurationException {
			DocumentBuilder docBuilder = DocumentBuilderFactory.newInstance().newDocumentBuilder();
			Document document = docBuilder.newDocument();
			document.appendChild(document.importNode(element, true));
			return document;
		}
		
		public void compare(TemplateGroup other)
				throws IOException, SAXException, ParserConfigurationException {
			Assert.assertArrayEquals(compact, other.compact);
			Assert.assertArrayEquals(iso, other.iso);
			XMLUnit.setIgnoreWhitespace(true);
			XMLAssert.assertXMLIdentical(new Diff(toDocument(xml), toDocument(other.xml)), true);
		}
	}
	
	@Test
	public void testCompact()
	throws IOException, SAXException, ParserConfigurationException {
		for (TemplateGroup template : TemplateLoader.natives.loadTemplates()) {
			Fingerprint fp = new Fingerprint();
			fp.setTemplate(template.compact);
			template.compare(TemplateGroup.export(fp));
		}
	}
	
	@Test
	public void testXML()
	throws IOException, SAXException, ParserConfigurationException {
		for (TemplateGroup template : TemplateLoader.natives.loadTemplates()) {
			Fingerprint fp = new Fingerprint();
			fp.setXmlTemplate(template.xml);
			template.compare(TemplateGroup.export(fp));
		}
	}
	
	@Test
	public void testISO()
	throws IOException, SAXException, ParserConfigurationException {
		for (TemplateGroup template : TemplateLoader.iso.loadTemplates()) {
			Fingerprint fp = new Fingerprint();
			fp.setIsoTemplate(template.originalIso);
			template.compare(TemplateGroup.export(fp));
		}
	}
}

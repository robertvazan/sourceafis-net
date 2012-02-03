package sourceafis.templates;

import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;
import javax.xml.parsers.ParserConfigurationException;
import javax.xml.transform.OutputKeys;
import javax.xml.transform.Transformer;
import javax.xml.transform.TransformerFactory;
import javax.xml.transform.dom.DOMSource;
import javax.xml.transform.stream.StreamResult;
import org.w3c.dom.Document;
import org.w3c.dom.Element;
import org.w3c.dom.NodeList;
import org.xml.sax.SAXException;
import sourceafis.general.Point;

public class XmlFormat extends TemplateFormatBase<Element> {

	@Override
	public Element Deserialize(InputStream stream) {
		try {
			DocumentBuilder docBuilder;
			docBuilder = DocumentBuilderFactory.newInstance()
					.newDocumentBuilder();
			Document templateDoc = docBuilder.parse(stream);
			Element root = templateDoc.getDocumentElement();
			return root;
		} catch (ParserConfigurationException e) {
			throw new RuntimeException(e);
		} catch (SAXException e) {
			throw new RuntimeException(e);
		} catch (IOException e) {
			throw new RuntimeException(e);
		}
	}

	@Override
	public void Serialize(OutputStream stream, Element template) {
		try {
			Transformer tf = TransformerFactory.newInstance().newTransformer();
			//tf.setOutputProperty(OutputKeys.OMIT_XML_DECLARATION, "yes");
			tf.setOutputProperty(OutputKeys.ENCODING, "UTF-8");
			tf.setOutputProperty(OutputKeys.INDENT, "yes");
			tf.transform(new DOMSource(template), new StreamResult(stream));
		} catch (Exception e) {
			throw new RuntimeException(e);
		}
	}

	@Override
	public Element Export(TemplateBuilder builder) {

		try {
			DocumentBuilderFactory dbf = DocumentBuilderFactory.newInstance();
			DocumentBuilder db = dbf.newDocumentBuilder();
			Document document = db.newDocument();
			Element template = document.createElement("FingerprintTemplate");
			template.setAttribute("Version", "2");
			template.setAttribute("OriginalDpi", Integer
					.toString(builder.OriginalDpi));
			template.setAttribute("OriginalWidth", Integer
					.toString(builder.OriginalWidth));
			template.setAttribute("OriginalHeight", Integer
					.toString(builder.OriginalHeight));
			document.appendChild(template);
			for (Minutia minutia : builder.Minutiae) {

				Element m = document.createElement("Minutia");
				m.setAttribute("X", Integer.toString(minutia.Position.X));
				m.setAttribute("Y", Integer.toString(minutia.Position.Y));
				m.setAttribute("Direction", Integer.toString(minutia.Direction & 0xFF));
				m.setAttribute("Type", minutia.Type.toString());
				template.appendChild(m);
			}
			return template;
		} catch (ParserConfigurationException pce) {
			throw new RuntimeException(pce);
		}
	}

	@Override
	public TemplateBuilder Import(Element template) {
		int version = Integer.parseInt(template.getAttribute("Version"));
		if (version < 1 || version > 2)
			throw new RuntimeException("Unknown template version.");
		TemplateBuilder builder = new TemplateBuilder();

		NodeList list = template.getElementsByTagName("Minutia");
		Point ref=new Point(0, 0);
		for (int x = 0; x < list.getLength(); x++) {
			Element node = (Element) list.item(x);
			String X = node.getAttribute("X");
			String Y = node.getAttribute("Y");
			String D = node.getAttribute("Direction");
			String T = node.getAttribute("Type");
			Minutia m = new Minutia();
			m.Direction = Byte.parseByte(D);
			m.Position = new Point(Integer.parseInt(X), Integer.parseInt(Y));
			m.Type = MinutiaType.valueOf(T);
			if(m.Position.X > ref.X) ref.X = m.Position.X;
			if(m.Position.Y > ref.Y) ref.Y = m.Position.Y;
		}

		if (version >= 2) {
			builder.OriginalDpi = Integer.parseInt(template.getAttribute("OriginalDpi"));
			builder.OriginalWidth = Integer.parseInt(template.getAttribute("OriginalWidth"));
			builder.OriginalHeight = Integer.parseInt(template.getAttribute("OriginalHeight"));
		} else {
			builder.OriginalDpi = 500;
			builder.setStandardDpiWidth(ref.X+1);
			builder.setStandardDpiHeight(ref.Y+1);
		}
		return builder;
	}
}

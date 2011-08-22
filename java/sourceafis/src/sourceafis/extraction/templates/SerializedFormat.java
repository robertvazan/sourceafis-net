package sourceafis.extraction.templates;

import java.io.InputStream;
import java.io.OutputStream;
/*
 * Remove the Assertion Error after implementing BinaryFormatter
 */
public final class SerializedFormat extends TemplateFormatBase<Template>{
    @Override
	public Template Export(TemplateBuilder builder)
    {
        return new Template(builder);
    }

    @Override
    public TemplateBuilder Import(Template template)
    {
        return template.ToTemplateBuilder();
    }
    @Override
    public void Serialize(OutputStream stream, Template template)
    {
       // BinaryFormatter formatter = new BinaryFormatter();
       // formatter.Serialize(stream, template);
        throw new AssertionError();
     }
    @Override
    public Template Deserialize(InputStream stream)
    {
        //BinaryFormatter formatter = new BinaryFormatter();
        //return formatter.Deserialize(stream) as Template;
        throw new AssertionError();
    }
}

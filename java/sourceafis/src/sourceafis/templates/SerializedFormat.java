package sourceafis.templates;

import java.io.IOException;
import java.io.InputStream;
import java.io.ObjectInputStream;
import java.io.ObjectOutputStream;
import java.io.OutputStream;


public final class SerializedFormat extends TemplateFormatBase<Template>{
    @Override
	public Template exportTemplate(TemplateBuilder builder)
    {
        return new Template(builder);
    }

    @Override
    public TemplateBuilder importTemplate(Template template)
    {
        return template.toTemplateBuilder();
    }
    @Override
    public void serialize(OutputStream stream, Template template)
    {
       
		try {
			ObjectOutputStream formatter = new ObjectOutputStream(stream);
			formatter.writeObject(template);
		} catch (IOException e) {
	    	throw new RuntimeException(e);
		}
     }
    @Override
    public Template deserialize(InputStream stream)
    {
    	try {
          ObjectInputStream formatter=new ObjectInputStream(stream);
       	  return (Template)formatter.readObject();
		} catch (ClassNotFoundException e) {
		  throw new RuntimeException(e);
		}catch(IOException e){
          throw new RuntimeException(e);
    	}
    }
}

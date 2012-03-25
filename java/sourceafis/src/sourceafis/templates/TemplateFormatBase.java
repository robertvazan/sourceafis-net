package sourceafis.templates;

import java.io.ByteArrayInputStream;
import java.io.ByteArrayOutputStream;
import java.io.InputStream;
import java.io.OutputStream;

public abstract class TemplateFormatBase<T> implements ITemplateFormat<T> {

	 public abstract T exportTemplate(TemplateBuilder builder);
     public abstract TemplateBuilder importTemplate(T template);
     public abstract void serialize(OutputStream stream, T template);
     public abstract T deserialize(InputStream stream);

     /*
      * MemoryStream is mapped to ByteArrayOutputStream
      * */
     public byte[] serialize(T template)
     {
         ByteArrayOutputStream stream = new ByteArrayOutputStream();
         serialize(stream, template);
         return stream.toByteArray();
     }
     /*
      * MemoryStream is mapped to ByteArrayInputStream
      * */
     public T deserialize(byte[] serialized)
     {
         ByteArrayInputStream stream = new ByteArrayInputStream(serialized);
         return deserialize(stream);
     }

     public byte[] serializeBuilder(TemplateBuilder builder)
     {
         return serialize(exportTemplate(builder));
     }

     public TemplateBuilder deserializeBuilder(byte[] serialized)
     {
         return importTemplate(deserialize(serialized));
     }
}

package sourceafis.extraction.templates;

import java.io.ByteArrayInputStream;
import java.io.ByteArrayOutputStream;
import java.io.InputStream;
import java.io.OutputStream;

public abstract class TemplateFormatBase<T> implements ITemplateFormat<T> {

	 public abstract T Export(TemplateBuilder builder);
     public abstract TemplateBuilder Import(T template);
     public abstract void Serialize(OutputStream stream, T template);
     public abstract T Deserialize(InputStream stream);

     /*
      * MemoryStream is mapped to ByteArrayOutputStream
      * */
     public byte[] Serialize(T template)
     {
         ByteArrayOutputStream stream = new ByteArrayOutputStream();
         Serialize(stream, template);
         return stream.toByteArray();
     }
     /*
      * MemoryStream is mapped to ByteArrayInputStream
      * */
     public T Deserialize(byte[] serialized)
     {
         ByteArrayInputStream stream = new ByteArrayInputStream(serialized);
         return Deserialize(stream);
     }

     public byte[] SerializeBuilder(TemplateBuilder builder)
     {
         return Serialize(Export(builder));
     }

     public TemplateBuilder DeserializeBuilder(byte[] serialized)
     {
         return Import(Deserialize(serialized));
     }
}

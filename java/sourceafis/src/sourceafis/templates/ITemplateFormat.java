package sourceafis.templates;

import java.io.InputStream;
import java.io.OutputStream;

public interface ITemplateFormat<T>{
    T Export(TemplateBuilder builder);
    TemplateBuilder Import(T template);
    /*
     * Stream in .net is converted as OutputStream 
     */
    void Serialize(OutputStream stream, T template);
    /*
     * Stream in .net is converted as InputStream
     */
    T Deserialize(InputStream stream);
    
    byte[] Serialize(T template);
    T Deserialize(byte[] serialized);
    byte[] SerializeBuilder(TemplateBuilder builder);
    TemplateBuilder DeserializeBuilder(byte[] serialized);
}
package sourceafis.templates;

import java.io.InputStream;
import java.io.OutputStream;

public interface ITemplateFormat<T>{
    T exportTemplate(TemplateBuilder builder);
    TemplateBuilder importTemplate(T template);
    void serialize(OutputStream stream, T template);
    T deserialize(InputStream stream);
    byte[] serialize(T template);
    T deserialize(byte[] serialized);
    byte[] serializeBuilder(TemplateBuilder builder);
    TemplateBuilder deserializeBuilder(byte[] serialized);
}
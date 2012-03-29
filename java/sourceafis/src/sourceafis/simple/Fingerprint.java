package sourceafis.simple;
import java.io.Serializable;
import org.w3c.dom.Element;
import sourceafis.templates.CompactFormat;
import sourceafis.templates.IsoFormat;
import sourceafis.templates.SerializedFormat;
import sourceafis.templates.Template;
import sourceafis.templates.XmlFormat;

/**
 * Collection of fingerprint-related information.
 * 
 * <p>This class contains basic information {@link Template} about the fingerprint that
 * is used by SourceAFIS to perform template extraction and fingerprint matching.
 * If you need to attach application-specific information to {@link Fingerprint} object,
 * inherit from this class and add fields as necessary. {@link Fingerprint} objects can be
 * grouped in {@link Person} objects.
 * </p>
 * <p>
 * This class is designed to be easy to serialize in order to be stored in binary format (BLOB)
 * in application database, binary or XML files, or sent over network. You can either serialize
 * the whole object or serialize individual properties. You can set some properties to "null" 
 * to exclude them from serialization.
 * </p>
 * @see Person 
 */
@SuppressWarnings("serial")
public class Fingerprint implements Cloneable, Serializable
{
    public Fingerprint() { }

    static CompactFormat compactFormat = new CompactFormat();
    static SerializedFormat serializedFormat = new SerializedFormat();
    static IsoFormat isoFormat = new IsoFormat();
    static XmlFormat xmlFormat = new XmlFormat();
    private Finger fingerPosition;
    Template decoded;
    private byte[][] image;

    public byte[][] getImage() { return image; }
    public void setImage(byte[][] newImage) { image = newImage; }
    
    /**
     * Fingerprint template.
     * 
     * <p>
     * Fingerprint template generated by {@link AfisEngine#extract}  or other template assigned
     * for example after deserialization. This property is "null" by default.
     * </p>
     * <p> Fingerprint template is an abstract model of the fingerprint that is serialized
     * in a very compact binary format (up to a few KB). Templates are better than fingerprint images,
     * because they require less space and they are easier to match than images. To generate
     * {@link Template}, pass {@link Fingerprint} object with valid  Image to {@link AfisEngine#extract}.
     * {@link Template} is required by {@link AfisEngine#verify} and {@link AfisEngine#identify}.
     * </p>
     * <p>Format of the template may change in later versions of SourceAFIS.
     * Applications are recommended to keep the original Image in order to be able
     * to regenerate the {@link Template}.
     * </p>
     * <p>
     * If you need access to the internal structure of the template, use
     * {@link sourceafis.templates.CompactFormat} to convert it to
     * {@link sourceafis.templates.TemplateBuilder}.
     * </p>
     * @see sourceafis.templates.CompactFormat
     * @see sourceafis.templates.TemplateBuilder
     */
    public byte[] getTemplate(){
         return decoded != null ? compactFormat.exportTemplate(serializedFormat.importTemplate(decoded)) : null; 
    }
    public void  setTemplate(byte[] value) { 
    	decoded = value != null ? serializedFormat.exportTemplate(compactFormat.importTemplate(value)) : null;
    }
    

   /**
    * Fingerprint template in standard ISO format.
    * Value of <see cref="Template"/> converted to standard ISO/IEC 19794-2 (2005) format.
    * This property is  "null"  if @link Template  is "null".
    *<p>
    * Use this property for two-way exchange of fingerprint templates with other biometric
    * systems. For general use in SourceAFIS, use <see cref="Template"/> property which
    * contains native template that is fine-tuned for best accuracy and performance in SourceAFIS.
    *</p> 
    *<p> 
    * SourceAFIS contains partial implementation of ISO/IEC 19794-2 (2005) standard.
    * Multi-fingerprint ISO templates must be split into individual fingerprints before
    * they are used in SourceAFIS. Value of @link Fingerprint#Finger  property is not
    * automatically stored in the ISO template. It must be decoded separately.
    * </p>
    * @see Template 
    * @see AfisEngine#extract
    * @see sourceafis.templates.IsoFormat
    * @see sourceafis.templates.TemplateBuilder 
    */
    public void setIsoTemplate(byte[] value){
         decoded = value != null ? serializedFormat.exportTemplate(isoFormat.importTemplate(value)) : null;
    }
    public byte[] getIsoTemplate(){
    	return decoded != null ? isoFormat.exportTemplate(serializedFormat.importTemplate(decoded)) : null; 
    }
    public void setXmlTemplate(Element value)
    {
        this.decoded = value != null ? serializedFormat.exportTemplate(xmlFormat.importTemplate(value)) : null; 
    }
    public Element getXmlTemplate()
    {
        return decoded != null ? xmlFormat.exportTemplate(serializedFormat.importTemplate(decoded)) : null; 
    }
   
    /**
    * Position of the finger on hand.
    *<p>
    * Finger (thumb to little) and hand (right or left) that was used to create this fingerprint.
    * Default value {@link Finger#ANY} means unspecified finger position.
    *</p>
    * Finger position is used to speed up matching by skipping fingerprint pairs
    * with incompatible finger positions. Check {@link Finger} enumeration for information
    * on how to control this process. Default value {@link Finger#ANY} disables this behavior.
    * 
    * @see Finger
    */
    public Finger getFinger(){
       return fingerPosition; 
    }
    public void setFinger(Finger value){
            fingerPosition = value;
    }

    public Fingerprint clone() {
    		Fingerprint fp=new Fingerprint();
    		fp.decoded=this.decoded.clone();
    		fp.fingerPosition=this.fingerPosition;
    		return fp;
    }
   
}
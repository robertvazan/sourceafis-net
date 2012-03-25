package sourceafis.templates;

import java.io.ByteArrayInputStream;
import java.io.ByteArrayOutputStream;
import java.io.DataInputStream;
import java.io.DataOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.nio.ByteBuffer;
import sourceafis.general.AssertException;

public final class CompactFormat extends TemplateFormatBase<byte[]>
{
    /**
     * Campact Template Format
     * <p>
     *  Template format (all numbers are big-endian):
     *  4B magic
     *  1B version (current = 2)
     *  2B total length (including magic)
     *  2B original DPI (since version 2)
     *  2B original width (since version 2)
     *  2B original height (since version 2)
     *  2B minutia count
     *  N*6B minutia records
     *      2B position X
     *      2B position Y
     *      1B direction
     *      1B type
     *</p>      
	 */

    static byte[] Magic = new byte[] { (byte)0x50, (byte)0xBC, (byte)0xAF, 0x15 }; // read "SorcAFIS"
    @Override
    public  byte[] exportTemplate(TemplateBuilder builder)
    {
         try{
            ByteArrayOutputStream stream=new ByteArrayOutputStream(); 
            DataOutputStream writer = new DataOutputStream(stream);
            // 4B magic
            writer.write(Magic);
            // 1B version (current = 2)
            writer.writeByte(2);
            // 2B total length (including magic), will be filled later
            writer.writeShort(0);
            // 2B original DPI (since version 2)
            writer.writeShort(builder.originalDpi);
            // 2B original width (since version 2)
            writer.writeShort(builder.originalWidth);
            // 2B original height (since version 2)
            writer.writeShort(builder.originalHeight);
            // 2B minutia count
            writer.writeShort(builder.minutiae.size());
            // N*6B minutia records
            for(Minutia minutia: builder.minutiae)
            {
                //      2B position X
                writer.writeShort(minutia.Position.X);
                //      2B position Y
                writer.writeShort(minutia.Position.Y);
                //      1B direction
                writer.writeByte(minutia.Direction);
                //      1B type
                writer.writeByte((byte)minutia.Type.getValue());
            }
            writer.close();
            // update length
            byte[] template = stream.toByteArray();
             ByteBuffer.wrap(template).putShort(5, (short)template.length);
            return template;
    	   }catch(IOException e){
    		   throw new RuntimeException(e);
    	   }
         
    }

   @Override
    public  TemplateBuilder importTemplate(byte[] template)
    {
      try{
	    
    	TemplateBuilder builder = new TemplateBuilder();
        ByteArrayInputStream stream=new ByteArrayInputStream(template);
        DataInputStream reader = new DataInputStream(stream);
        // 4B magic
        for (int i = 0; i < Magic.length; ++i)
            AssertException.Check(reader.readByte() == Magic[i],"This is not an Compact template.");
        // 1B version (current = 2)
        byte version = reader.readByte();
        AssertException.Check(version >= 1 && version <= 2,"Invalid Template Version");
        // 2B total length (including magic)
        reader.readShort();
        if (version >= 2)
        {
            // 2B original DPI (since version 2)
            builder.originalDpi =  reader.readShort();
            // 2B original width (since version 2)
            builder.originalWidth =  reader.readShort();
            // 2B original height (since version 2)
            builder.originalHeight = reader.readShort();
        }
        // 2B minutia count
        int minutiaCount = reader.readShort();
        // N*6B minutia records
        for (int i = 0; i < minutiaCount; ++i)
        {
            Minutia minutia = new  Minutia();
            //2B position X
            minutia.Position.X = reader.readShort();
            //2B position Y
            minutia.Position.Y =  reader.readShort();
            //1B direction
            minutia.Direction = reader.readByte();
            //1B type
            byte type=reader.readByte();
            minutia.Type = MinutiaType.get(type);
            builder.minutiae.add(minutia);
        }
        return builder;
      }catch(IOException e){
    	  throw new RuntimeException(e);
      }
    }
    @Override
    public  void serialize(OutputStream stream, byte[] template)
    {
        try {
			stream.write(template, 0, template.length);
		} catch (IOException e) {
			new RuntimeException(e);
		}
    }

    @Override
    public  byte[] deserialize(InputStream stream)
    {
    	 try {
           byte[] header = new byte[7];
           stream.read(header, 0, 7);
           int length = ByteBuffer.wrap(header).getShort(5);
           byte[] template = new byte[length];
           System.arraycopy(header,0,template, 0,7);
           stream.read(template, 7, length - 7);
           return template;
           } catch (IOException e) {
    			new RuntimeException(e);
           }
           return null;
    }
}
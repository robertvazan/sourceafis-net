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

//import sourceafis.general.BinaryWriter;
public final class IsoFormat extends TemplateFormatBase<byte[]>
{
    // References:
    // http://www.italdata-roma.com/PDF/Norme%20ISO-IEC%20Minutiae%20Data%20Format%2019794-2.pdf
    // https://biolab.csr.unibo.it/fvcongoing/UI/Form/Download.aspx (ISO section, sample ZIP, ISOTemplate.pdf)
    //
    // Format (all numbers are big-endian):
    // 4B magic "FMR\0"
    // 4B version (ignored, set to " 20\0"
    // 4B total length (including header)
    // 2B rubbish (zeroed)
    // 2B image size in pixels X
    // 2B image size in pixels Y
    // 2B rubbish (pixels per cm X, set to 196 = 500dpi)
    // 2B rubbish (pixels per cm Y, set to 196 = 500dpi)
    // 1B rubbish (number of fingerprints, set to 1)
    // 1B rubbish (zeroed)
    // 1B rubbish (finger position, zeroed)
    // 1B rubbish (zeroed)
    // 1B rubbish (fingerprint quality, set to 100)
    // 1B minutia count
    // N*6B minutiae
    //      2B minutia position X in pixels
    //          2b (upper) minutia type (01 ending, 10 bifurcation, 00 other (considered ending))
    //      2B minutia position Y in pixels (upper 2b ignored, zeroed)
    //      1B direction, compatible with SourceAFIS angles
    //      1B quality (ignored, zeroed)
    // 2B rubbish (extra data length, zeroed)
    // N*1B rubbish (extra data)
    @Override
    public byte[] exportTemplate(TemplateBuilder builder)
    {
    	try{
        //MemoryStream stream = new MemoryStream();
    	ByteArrayOutputStream stream = new ByteArrayOutputStream();
    	  
        DataOutputStream writer = new DataOutputStream(stream);

        
            // 4B magic "FMR\0"
            writer.writeBytes("FMR\0");

            // 4B version (ignored, set to " 20\0"
            writer.writeBytes(" 20\0");

            // 4B total length (including header, will be updated later)
            writer.writeInt(0);

            // 2B rubbish (zeroed)
            writer.writeShort((short)0);

            // 2B image size in pixels X
            writer.writeShort((short)builder.getStandardDpiWidth());
        
            // 2B image size in pixels Y
            writer.writeShort((short)builder.getStandardDpiHeight());
        
            // 2B rubbish (pixels per cm X, set to 196 = 500dpi)
            writer.writeShort((short)196);

            // 2B rubbish (pixels per cm Y, set to 196 = 500dpi)
            writer.writeShort((short)196);
            // 1B rubbish (number of fingerprints, set to 1)
            writer.writeByte(1);
            // 1B rubbish (zeroed)
            writer.writeByte(0);
            // 1B rubbish (finger position, zeroed)
            writer.writeByte(0);

            // 1B rubbish (zeroed)
            writer.writeByte(0);
            // 1B rubbish (fingerprint quality, set to 100)
            writer.writeByte(100);

            // 1B minutia count
            writer.writeByte(builder.minutiae.size());
            // N*6B minutiae
            for(Minutia minutia : builder.minutiae)
            {
                // B minutia position X in pixels
                // 2b (upper) minutia type (01 ending, 10 bifurcation, 00 other (considered ending))
            	int x = minutia.Position.X;
                AssertException.Check(x <= 0x3fff, "X position is out of range");
                int type=0;
                switch (minutia.Type){
                 case Ending: type = 0x4000; break;
                 case Bifurcation: type = 0x8000; break;
                 case Other: type = 0; break;
                }
                writer.writeShort( x | type);
                // 2B minutia position Y in pixels (upper 2b ignored, zeroed)
                int y = builder.getStandardDpiHeight() - minutia.Position.Y - 1;
                AssertException.Check(y <= 0x3fff, "Y position is out of range");
                writer.writeShort(y);

                //1B direction, compatible with SourceAFIS angles
                writer.writeByte(minutia.Direction);

                //1B quality (ignored, zeroed)
                writer.writeByte(0);
            }

            // 2B rubbish (extra data length, zeroed)
            // N*1B rubbish (extra data)
            writer.writeShort(0);
            writer.close();

        // update length
        byte[] template = stream.toByteArray();
      //  BitConverter.GetBytes(IPAddress.HostToNetworkOrder(template.Length)).CopyTo(template, 8);
        ByteBuffer.wrap(template).putInt(8, template.length);
        return template;
    	}catch(IOException e){
    		throw new RuntimeException(e);
    	}
    }

    
    @Override 
    public TemplateBuilder importTemplate(byte[] template)
    {
    	String NULL=new String(new byte[]{0x00});
    	try{
        TemplateBuilder builder = new TemplateBuilder();
        builder.originalDpi = 500;
    
        //MemoryStream stream = new MemoryStream(template);
        ByteArrayInputStream stream=new ByteArrayInputStream(template);
        //BinaryReader reader = new BinaryReader(stream);
        DataInputStream reader = new DataInputStream(stream); 
        // 4B magic "FMR\0"
        byte[] magic=new byte[4];
        reader.read(magic);
        
        AssertException.Check(new String(magic) .equals("FMR"+NULL), "This is not an ISO template.");

        // 4B version (ignored, set to " 20\0"
        byte[] version=new byte[4];
        reader.read(version);

        // 4B total length (including header)
        AssertException.Check( reader.readInt() == template.length, "Invalid template length.");

        // 2B rubbish (zeroed)
        reader.readShort();

        // 2B image size in pixels X
        builder.setStandardDpiWidth(reader.readShort());

        // 2B image size in pixels Y
        builder.setStandardDpiHeight(reader.readShort());

        // 2B rubbish (pixels per cm X, set to 196 = 500dpi)
        reader.readShort();

        // 2B rubbish (pixels per cm Y, set to 196 = 500dpi)
        reader.readShort();

        // 1B rubbish (number of fingerprints, set to 1)
        AssertException.Check(reader.readByte() == 1, "Only single-fingerprint ISO templates are supported.");

        // 1B rubbish (zeroed)
        reader.readByte();

        // 1B rubbish (finger position, zeroed)
        reader.readByte();

        // 1B rubbish (zeroed)
        reader.readByte();

        // 1B rubbish (fingerprint quality, set to 100)
        reader.readByte();

        // 1B minutia count
        int minutiaCount = reader.readUnsignedByte();
         // N*6B minutiae
        //16602-F
        //16543-F
        //32957-T
        for (int i = 0; i < minutiaCount; ++i)
        {
            Minutia minutia = new Minutia();

            //      2B minutia position X in pixels
            //      2b (upper) minutia type (01 ending, 10 bifurcation, 00 other (considered ending))
            //Making it int (unsigened short)
            int xPacked = reader.readUnsignedShort();
            minutia.Position.X = xPacked  &  0x3fff;
            switch (xPacked & 0xc000)
             {
                 case 0x4000: minutia.Type=MinutiaType.Ending; break;//need to shift 6 places
                 case 0x8000: minutia.Type=MinutiaType.Bifurcation; break;
                 case 0: minutia.Type=MinutiaType.Other; break;
             } 
            //      2B minutia position Y in pixels (upper 2b ignored, zeroed)
            minutia.Position.Y = builder.getStandardDpiHeight() - 1 - ( reader.readShort() &  0x3fff);
            //      1B direction, compatible with SourceAFIS angles
            minutia.Direction = reader.readByte();

            //      1B quality (ignored, zeroed)
            reader.readByte();

            builder.minutiae.add(minutia);
        }

        // 2B rubbish (extra data length, zeroed)
        // N*1B rubbish (extra data)

        return builder;
    	}catch(IOException e){
    		e.printStackTrace();
    		throw new RuntimeException(e);
	
    	}
    }
    @Override
    public  void serialize(OutputStream stream, byte[] template)
    {
        try {
			stream.write(template, 0, template.length);
		} catch (IOException e) {
			throw new RuntimeException(e);

		}
    }
    @Override
    public  byte[] deserialize(InputStream stream)
    {
    	try {
    	byte[] header = new byte[12];
		stream.read(header, 0, 12);
		
        int length = ByteBuffer.wrap(header).getInt(8);

        byte[] template = new byte[length];
        System.arraycopy(header,0,template, 0, 12);
        stream.read(template, 12, length - 12);
        return template;
    	} catch (IOException e) {
		  		throw new RuntimeException(e);
		}
    }
}
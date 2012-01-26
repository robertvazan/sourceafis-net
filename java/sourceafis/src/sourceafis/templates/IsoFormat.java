package sourceafis.templates;

import java.io.ByteArrayInputStream;
import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.nio.ByteBuffer;

import sourceafis.general.AssertException;
import sourceafis.general.BinaryReader;
import sourceafis.general.BinaryWriter;
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
    public byte[] Export(TemplateBuilder builder)
    {
    	try{
        //MemoryStream stream = new MemoryStream();
    	ByteArrayOutputStream stream = new ByteArrayOutputStream();
    	  
        BinaryWriter writer = new BinaryWriter(stream);

        
            // 4B magic "FMR\0"
            writer.Write("FMR\0".toCharArray());

            // 4B version (ignored, set to " 20\0"
            writer.Write(" 20\0".toCharArray());

            // 4B total length (including header, will be updated later)
            writer.Write(0);

            // 2B rubbish (zeroed)
            writer.Write((short)0);

            // 2B image size in pixels X
            writer.Write((short)builder.getStandardDpiWidth());
        
            // 2B image size in pixels Y
            writer.Write((short)builder.getStandardDpiHeight());
        
            // 2B rubbish (pixels per cm X, set to 196 = 500dpi)
            writer.Write((short)196);

            // 2B rubbish (pixels per cm Y, set to 196 = 500dpi)
            writer.Write((short)196);
            // 1B rubbish (number of fingerprints, set to 1)
            writer.Write((byte)1);
            // 1B rubbish (zeroed)
            writer.Write((byte)0);
            // 1B rubbish (finger position, zeroed)
            writer.Write((byte)0);

            // 1B rubbish (zeroed)
            writer.Write((byte)0);

            // 1B rubbish (fingerprint quality, set to 100)
            writer.Write((byte)100);

            // 1B minutia count
            writer.Write((byte)builder.Minutiae.size());

            // N*6B minutiae
            for(Minutia minutia : builder.Minutiae)
            {
                //      2B minutia position X in pixels
                //          2b (upper) minutia type (01 ending, 10 bifurcation, 00 other (considered ending))
                int x = minutia.Position.X;
                AssertException.Check(x <= 0x3fff, "X position is out of range");
                int type = minutia.Type == MinutiaType.Ending ? 0x4000 : 0x8000;
                writer.Write( (short)(x | type));
                //      2B minutia position Y in pixels (upper 2b ignored, zeroed)
                int y = builder.getStandardDpiHeight() - minutia.Position.Y - 1;
                AssertException.Check(y <= 0x3fff, "Y position is out of range");
                writer.Write((short)y);

                //      1B direction, compatible with SourceAFIS angles
                writer.Write(minutia.Direction);

                //      1B quality (ignored, zeroed)
                writer.Write((byte)0);
            }

            // 2B rubbish (extra data length, zeroed)
            // N*1B rubbish (extra data)
            writer.Write((short)0);
        

        writer.Close();

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
    public TemplateBuilder Import(byte[] template)
    {
    	String NULL=new String(new byte[]{0x00});
    	try{
        TemplateBuilder builder = new TemplateBuilder();
        builder.OriginalDpi = 500;
    
        //MemoryStream stream = new MemoryStream(template);
        ByteArrayInputStream stream=new ByteArrayInputStream(template);
        BinaryReader reader = new BinaryReader(stream);

        // 4B magic "FMR\0"
        
        AssertException.Check(reader.readString(4) .equals("FMR"+NULL), "This is not an ISO template.");

        // 4B version (ignored, set to " 20\0"
        reader.readString(4);

        // 4B total length (including header)
        AssertException.Check( reader.ReadInt32() == template.length, "Invalid template length.");

        // 2B rubbish (zeroed)
        reader.ReadInt16();

        // 2B image size in pixels X
        builder.setStandardDpiWidth(reader.ReadInt16());

        // 2B image size in pixels Y
        builder.setStandardDpiHeight(reader.ReadInt16());

        // 2B rubbish (pixels per cm X, set to 196 = 500dpi)
        reader.ReadInt16();

        // 2B rubbish (pixels per cm Y, set to 196 = 500dpi)
        reader.ReadInt16();

        // 1B rubbish (number of fingerprints, set to 1)
        AssertException.Check(reader.ReadByte() == 1, "Only single-fingerprint ISO templates are supported.");

        // 1B rubbish (zeroed)
        reader.ReadByte();

        // 1B rubbish (finger position, zeroed)
        reader.ReadByte();

        // 1B rubbish (zeroed)
        reader.ReadByte();

        // 1B rubbish (fingerprint quality, set to 100)
        reader.ReadByte();

        // 1B minutia count
        int minutiaCount = reader.ReadByte();
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
            int xPacked = reader.ReadInt16();
             
            minutia.Position.X = xPacked &  0x3fff;
            minutia.Type = (xPacked & (short)0xc000) == 0x8000 ? MinutiaType.Bifurcation : MinutiaType.Ending;
            //      2B minutia position Y in pixels (upper 2b ignored, zeroed)
            minutia.Position.Y = builder.getStandardDpiHeight() - 1 - ( reader.ReadInt16() &  0x3fff);
            //      1B direction, compatible with SourceAFIS angles
            minutia.Direction = reader.ReadByte();

            //      1B quality (ignored, zeroed)
            reader.ReadByte();

            builder.Minutiae.add(minutia);
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
    public  void Serialize(OutputStream stream, byte[] template)
    {
        try {
			stream.write(template, 0, template.length);
		} catch (IOException e) {
			throw new RuntimeException(e);

		}
    }
    @Override
    public  byte[] Deserialize(InputStream stream)
    {
    	try {
    	byte[] header = new byte[12];
		stream.read(header, 0, 12);
		
        int length = ByteBuffer.wrap(header).getInt(8);

        byte[] template = new byte[length];
        //header.CopyTo(template, 0);
        System.arraycopy(header,0,template, 0, 12);
        stream.read(template, 12, length - 12);
        return template;
    	} catch (IOException e) {
		  		throw new RuntimeException(e);
		}
    }
}
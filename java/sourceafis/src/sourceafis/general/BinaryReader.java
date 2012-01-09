package sourceafis.general;

import java.io.EOFException;
import java.io.IOException;
import java.io.InputStream;

/*
 * 
 */
public class BinaryReader {
	InputStream is;

	public BinaryReader(InputStream is) {
		super();
		this.is = is;
	}

	public final byte ReadByte() throws IOException {
		int ch = is.read();
		if (ch < 0)
			throw new EOFException();
		return (byte) (ch);
	}

	public final int ReadInt16() throws IOException {
		int ch1 = is.read();
		int ch2 = is.read();
		if ((ch1 | ch2) < 0)
			throw new EOFException();
		return  ((ch1 << 8) + (ch2 << 0));
	}
 	public final int ReadInt32() throws IOException {
		int ch1 = is.read();
		int ch2 = is.read();
		int ch3 = is.read();
		int ch4 = is.read();
		if ((ch1 | ch2 | ch3 | ch4) < 0)
			throw new EOFException();
		return ((ch1 << 24) + (ch2 << 16) + (ch3 << 8) + (ch4 << 0));
	}

	public String readString(int length) throws IOException {
		byte[] bs = new byte[length];
		is.read(bs);
		return new String(bs, "UTF-8");
	}

	public final int read(byte b[]) throws IOException {
		return is.read(b, 0, b.length);
	}

	public void close() throws IOException {
		is.close();
	}

}

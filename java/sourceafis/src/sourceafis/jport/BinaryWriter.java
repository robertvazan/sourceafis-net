package sourceafis.jport;

import java.io.IOException;
import java.io.OutputStream;
import java.nio.charset.Charset;

/**
 * Similar to Data output Stream
 */
public class BinaryWriter {
	static Charset cs = Charset.forName("UTF-8");
	OutputStream out;

	public BinaryWriter(OutputStream out) {
		super();
		this.out = out;
	}

	public final void Write(char[] s) throws IOException {
		int len = s.length;
		for (int i = 0; i < len; i++) {
			out.write((byte) s[i]);
		}
	}

	public void Write(byte[] bs) throws IOException {
		out.write(bs);
	}

	public void Write(byte v) throws IOException {
		out.write(v);
	}

	public void Write(short v) throws IOException {
		out.write((v >>> 8) & 0xFF);
		out.write((v >>> 0) & 0xFF);
	}

	public void Write(int v) throws IOException {
		out.write((v >>> 24) & 0xFF);
		out.write((v >>> 16) & 0xFF);
		out.write((v >>> 8) & 0xFF);
		out.write((v >>> 0) & 0xFF);
	}

	public void Close() throws IOException {
		out.close();
	}

	public final void writeBytes(String s) throws IOException {
		int len = s.length();
		for (int i = 0; i < len; i++) {
			out.write((byte) s.charAt(i));
		}
	}

	public void write(byte[] bs) throws IOException {
		out.write(bs);
	}

	/**
	 * Write a two byte long unsigned number
	 * 
	 * @param x
	 * @throws IOException
	 */
	public void writeShort(int v) throws IOException {
		out.write((v >>> 8) & 0xFF);
		out.write((v >>> 0) & 0xFF);
	}

	/**
	 * Write a single byte
	 * 
	 * @param x
	 * @throws IOException
	 */
	public void writeByte(int v) throws IOException {
		// out.write(x&0xFF);
		out.write(v);
	}

	/**
	 * Write a four byte long unsigned number
	 * 
	 * @param x
	 * @throws IOException
	 */
	public void writeInt(int v) throws IOException {
		out.write((v >>> 24) & 0xFF);
		out.write((v >>> 16) & 0xFF);
		out.write((v >>> 8) & 0xFF);
		out.write((v >>> 0) & 0xFF);
	}

}

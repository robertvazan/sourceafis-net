package sourceafis.general;

import java.io.IOException;

public class BitConverter {
	public static void Write(byte[] array,int v,int offset) throws IOException {
	    array[offset]=(byte)((v >>> 24) & 0xFF);
	    array[offset+1]=(byte)((v >>> 16) & 0xFF);
	    array[offset+2]=(byte)((v >>>  8) & 0xFF);
	    array[offset+3]=(byte)((v >>>  0) & 0xFF);
	}
	public static void Write(byte[] array,short v,int offset) throws IOException {
	    array[offset]=(byte)((v >>>  8) & 0xFF);
	    array[offset+1]=(byte)((v >>>  0) & 0xFF);
	}
	
	public static int ToInt32(byte[] data, int offset){
		    int ch1 = data[offset];
	        int ch2 = data[offset+1];
	        int ch3 = data[offset+2];
	        int ch4 = data[offset+3];
	        return ((ch1 << 24) + (ch2 << 16) + (ch3 << 8) + (ch4 << 0));
	}
	public static int ToInt16(byte[] data, int offset){
	    int ch1 = data[offset];
        int ch2 = data[offset+1];
        return (  (ch1 << 8) + (ch2 << 0));
}
}

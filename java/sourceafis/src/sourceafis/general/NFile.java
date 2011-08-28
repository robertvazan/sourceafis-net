package sourceafis.general;

import java.io.FileInputStream;

public class NFile {
 public static byte[] ReadAllBytes(String path){
	 try{
		 FileInputStream fis=new FileInputStream(path);
		 int size=fis.available();
		 byte[] out=new byte[size];
		 fis.read(out);
		 return out;
	 }catch(Exception e){
		 return new byte[]{};
	 }
 }
}

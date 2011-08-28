package sourceafis.general;


public class AssertException extends ApplicationException {
	
 	private static final long serialVersionUID = 3555131911211024044L;

	public AssertException() { }

     public AssertException(String message){
       super(message);
     }

	public static void Check(boolean condition,String message){
		if(!condition) throw new RuntimeException(message);
	}

}

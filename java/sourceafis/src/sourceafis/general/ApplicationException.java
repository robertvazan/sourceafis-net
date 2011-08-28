package sourceafis.general;

public class ApplicationException extends RuntimeException {

	private static final long serialVersionUID = -6044438103679791642L;
	public ApplicationException(){
		
	}
	public ApplicationException(String msg){
		super(msg);
	}

}

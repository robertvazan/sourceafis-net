package sourceafis.general;

public class InvalidOperationException extends RuntimeException {

	private static final long serialVersionUID = -7967236929644172860L;

	public InvalidOperationException(){
		super();
	}
	public InvalidOperationException(String msg){
		super(msg);
	}

}

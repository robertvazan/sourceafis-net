package sourceafis.templates;

import java.util.EnumSet;
import java.util.HashMap;
import java.util.Map;

/*
 *  
 */
public enum MinutiaType // : byte
{
	Ending(0), Bifurcation(1), Other(2);
	private final byte value;

	MinutiaType(int value) {
		this.value = (byte) value;
	}
	
	 private static final Map<Byte,MinutiaType> lookup 
	    = new HashMap<Byte,MinutiaType>();
     /*
      * Supporting Reverse LookUp
      */
	 static {
	 for(MinutiaType s : EnumSet.allOf(MinutiaType.class))
	      lookup.put(s.getValue(), s);
	 }
	 public byte getValue() {
			return value;
     }
	 public static MinutiaType get(byte code) { 
	        return lookup.get(code); 
	 }
	 
}

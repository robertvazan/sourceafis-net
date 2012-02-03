package sourceafis.simple;

import java.io.Serializable;
import java.util.ArrayList;
import java.util.List;

 /**
  * Collection of {@link Fingerprint}s belonging to one person.
  * <p>
  * This class is primarily a way to group multiple {@link Fingerprint}s belonging to one person.
  * This is very convenient feature when there are multiple fingerprints per person, because
  * it is possible to match two {@link Person}s directly instead of iterating over their {@link Fingerprint}s.
  * </p>
  * <p>
  * <tt>Id</tt> property is provided as simple means to bind {@link Person} objects to application-specific
  * information. If you need more flexibility, inherit from {@link Person} class and add
  * application-specific fields as necessary.
  * </p>
  * <p>
  * This class is designed to be easy to serialize in order to be stored in binary format (BLOB)
  * in application database, binary or XML files, or sent over network. You can either serialize
  * the whole {@link Person} or serialize individual {@link Fingerprint}s.
  * </p>
  * @see Fingerprint 
  */
 
@SuppressWarnings("serial")
public class Person implements Cloneable,Serializable
{
    /**  
      * Application-assigned ID for the <see cref="Person"/>.
      * <p>
      * SourceAFIS doesn't use this property. It is provided for applications as an easy means
      * to link {@link Person} objects back to application-specific data. Applications can store any
      * integer ID in this field, for example database table key or an array index.
      * </p>
      * <p>
      * Applications that need to attach more detailed information to the person should
      * inherit from <see cref="Person"/> class and add fields as necessary.
      * </p>
      */ 
    public int Id; // { get; set; }

    private List<Fingerprint> fingerprints = new ArrayList<Fingerprint>();

    

     /*  
      * Creates empty {@link Person} object.
      */ 
    public Person()
    {
    }

    /**  
      * Creates new {@link Person} object and initializes it with
      * a list of {@link Fingerprint}s.
      * 
      * @param fingerprints
      * @see  Fingerprint objects to add to the new {@link Person}.
      *
      */
    public Person(Fingerprint... fingerprints)
    {
    	for(Fingerprint fp:fingerprints){
    		this.fingerprints.add(fp);
    	}
        //this.Fingerprints =  fingerprints.ToList();
    }
    public int getId() {
		return Id;
	}
    
    public void setId(int id) {
		Id = id;
	}
    /**  
     * List of {@link Fingerprint}s belonging to the {@link Person}.
     * </summary>
     * <remarks>
     * This collection is initially empty. Add <see cref="Fingerprint"/> objects
     * here. You can also assign the whole collection.
     */ 
     public List<Fingerprint> getFingerprints(){
         return fingerprints; 
     }
     public void setFingerprint(List<Fingerprint> fingerprints) {
 		this.fingerprints = fingerprints;
 	 }

     /**
      * This method clones all {@link Fingerprint} objects contained
      * in this {@link Person}
      * @return a clone of this <tt>Person</tt> instance
      *
      **/  
    public Person clone() {
    	try {
    		Person copy = (Person)super.clone();
    		copy.fingerprints = new ArrayList<Fingerprint>(fingerprints);
    		return copy;
    	} catch (CloneNotSupportedException e) {
    		throw new RuntimeException(e);
    	}
    }

    void CheckForNulls()
    {
        for (Fingerprint fp : fingerprints)
            if (fp == null)
                throw new RuntimeException("Person contains null Fingerprint references.");
    }

	 
	

	
}
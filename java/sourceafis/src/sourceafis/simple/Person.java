package sourceafis.simple;

import java.io.Serializable;
import java.util.ArrayList;
import java.util.List;

 /**
  * Collection of {@link Fingerprint}s belonging to one person.
  * 
  * This class is primarily a way to group multiple {@link Fingerprint}s belonging to one person.
  * This is very convenient feature when there are multiple fingerprints per person, because
  * it is possible to match two {@link Person}s directly instead of iterating over their {@link Fingerprint}s.
  * <p>
  * {@link #setId Id} property is provided as a simple means to bind {@link Person} objects to application-specific
  * information. If you need more flexibility, inherit from {@link Person} class and add
  * application-specific fields as necessary.
  * <p>
  * This class is designed to be easy to serialize in order to be stored in binary format (BLOB)
  * in application database, binary or XML files, or sent over network. You can either serialize
  * the whole {@link Person} or serialize individual {@link Fingerprint}s.
  * 
  * @see Fingerprint 
  * @serial exclude
  */
 
@SuppressWarnings("serial")
public class Person implements Cloneable,Serializable
{
    private int Id;
    private List<Fingerprint> fingerprints = new ArrayList<Fingerprint>();

    /**
     * Creates an empty {@code Person} object.
     */
    public Person() { }

    /**
     * Creates new {@code Person} object and initializes it with a list of
     * {@link Fingerprint}s.
     * 
     * @param fingerprints
     *            {@link Fingerprint} objects to add to the new {@code Person}
     */
    public Person(Fingerprint... fingerprints) {
    	for(Fingerprint fp:fingerprints){
    		this.fingerprints.add(fp);
    	}
        //this.Fingerprints =  fingerprints.ToList();
    }

    /**
     * Gets application-defined ID for the Person.
     * 
     * See {@link #setId setId} for explanation. This method just returns
     * previously set ID.
     * 
     * @return ID that was previously set via {@link #setId setId}
     * @see #setId setId
     */
    public int getId() {
		return Id;
	}
    
    /**
     * Sets application-defined ID for the {@code Person}.
     * 
     * SourceAFIS doesn't use this ID. It is provided for applications as an
     * easy means to link {@code Person} objects back to application-specific
     * data. Applications can store any integer ID in this field, for example
     * database table key or an array index.
     * <p>
     * Applications that need to attach more detailed information to the person
     * should inherit from {@code Person} class and add fields as necessary.
     * 
     * @param id
     *            arbitrary application-defined ID
     * @see #getId getId
     */
    public void setId(int id) {
        Id = id;
    }
    
    /**
     * Gets list of {@link Fingerprint}s belonging to the {@link Person}.
     * 
     * This collection is initially empty. Add {@link Fingerprint} objects
     * to the returned collection.
     * 
     * @see #setFingerprints setFingerprints
     * @see Fingerprint
     */
    public List<Fingerprint> getFingerprints() {
        return fingerprints;
    }

    /**
     * Sets list of {@link Fingerprint}s belonging to the {@link Person}.
     * 
     * You can assign the whole collection using this method. Individual
     * {@link Fingerprint}s can be added to the collection returned from
     * {@link #getFingerprints getFingerprints}.
     * 
     * @param fingerprints
     *            new list of {@link Fingerprint}s for this {@code Person}
     * @see #getFingerprints getFingerprints
     * @see Fingerprint
     */
    public void setFingerprints(List<Fingerprint> fingerprints) {
        this.fingerprints = fingerprints;
    }

    /**
     * Creates deep copy of the Person.
     * 
     * This method clones all {@link Fingerprint} objects contained in this
     * {@code Person}.
     * 
     * @return deep copy of the {@code Person}
     */
    public Person clone() {
	    Person copy = new Person();
	    copy.Id = Id;
	    for (Fingerprint fingerprint : fingerprints)
	        copy.fingerprints.add(fingerprint.clone());
		return copy;
    }

    void CheckForNulls()
    {
        for (Fingerprint fp : fingerprints)
            if (fp == null)
                throw new RuntimeException("Person contains null Fingerprint references.");
    }
}

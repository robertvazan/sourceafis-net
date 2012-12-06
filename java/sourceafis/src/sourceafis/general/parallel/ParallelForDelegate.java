/**
 * @author Veaceslav Dubenco
 * @since 14.10.2012
 */
package sourceafis.general.parallel;

/**
 * 
 */
public interface ParallelForDelegate<T> {
	public T delegate(int i, T input);

	public T combineResults(T result1, T result2);
}

/**
 * @author Veaceslav Dubenco
 * @since 14.10.2012
 */
package sourceafis.general.parallel;

/**
 * 
 */
public interface ParallelForEachDelegate<T> {
	public T delegate(T item);

	public T combineResults(T result1, T result2);
}

/**
 * @author Veaceslav Dubenco
 * @since 14.10.2012
 */
package sourceafis.general.parallel;

import java.util.ArrayList;
import java.util.List;
import java.util.concurrent.RecursiveTask;

/**
 * Utility class containing static methods for running parallel processes
 */
public final class Parallel {
	public static <T> T For(int start, int end,
			ParallelForDelegate<T> delegate, T input) {
		T res = null;
		for (int i = start; i < end; i++) {
			res = delegate.combineResults(res, delegate.delegate(i, input));
		}
		return res;
	}

	public static <T> T ForEach(Iterable<T> items,
			ParallelForEachDelegate<T> delegate) {
		T res = null;
		for (T item : items) {
			res = delegate.combineResults(res, delegate.delegate(item));
		}
		return null;
	}

	/*public static <T> T For(int start, int end,
			ParallelForDelegate<T> delegate, T input) {
		ForkJoinPool forkJoinPool = new ForkJoinPool();
		return forkJoinPool.invoke(new ParallelForTask<T>(start, end, delegate,
				input));
	}

	public static <T> T ForEach(Iterable<T> items,
			ParallelForEachDelegate<T> delegate) {
		ForkJoinPool forkJoinPool = new ForkJoinPool();
		return forkJoinPool.invoke(new ParallelForEachTask<T>(items, delegate));
	}*/
}

class ParallelForTask<T> extends RecursiveTask<T> {
	private static final long serialVersionUID = 1L;
	int start;
	int end;
	T input;
	ParallelForDelegate<T> delegate;

	public ParallelForTask(int start, int end, ParallelForDelegate<T> delegate,
			T input) {
		this.start = start;
		this.end = end;
		this.delegate = delegate;
		this.input = input;
	}

	@Override
	protected T compute() {
		@SuppressWarnings("unchecked")
		ParallelForIterationTask<T>[] forks = new ParallelForIterationTask[end
				- start];
		for (int i = start; i < end; i++) {
			ParallelForIterationTask<T> iterAction = new ParallelForIterationTask<T>(
					i, delegate, input);
			forks[i - start] = iterAction;
			iterAction.fork();
		}
		T result = null;
		if (forks.length > 0) {
			result = forks[0].join();
			for (int i = 1; i < forks.length; i++) {
				result = delegate.combineResults(result, forks[i].join());
			}
		}

		return result;
	}
}

class ParallelForIterationTask<T> extends RecursiveTask<T> {
	private static final long serialVersionUID = 1L;
	int i;
	T input;
	ParallelForDelegate<T> delegate;

	public ParallelForIterationTask(int i, ParallelForDelegate<T> delegate,
			T input) {
		this.i = i;
		this.delegate = delegate;
		this.input = input;
	}

	@Override
	protected T compute() {
		return delegate.delegate(i, input);
	}
}

class ParallelForEachTask<T> extends RecursiveTask<T> {
	private static final long serialVersionUID = 1L;
	Iterable<T> items;
	ParallelForEachDelegate<T> delegate;

	public ParallelForEachTask(Iterable<T> items,
			ParallelForEachDelegate<T> delegate) {
		this.items = items;
		this.delegate = delegate;
	}

	@Override
	protected T compute() {
		List<ParallelForEachIterationTask<T>> forks = new ArrayList<ParallelForEachIterationTask<T>>();
		for (T item : items) {
			ParallelForEachIterationTask<T> iterAction = new ParallelForEachIterationTask<T>(
					delegate, item);
			forks.add(iterAction);
			iterAction.fork();
		}
		T result = null;
		if (forks.size() > 0) {
			result = forks.get(0).join();
			for (int i = 0; i < forks.size(); i++) {
				result = delegate.combineResults(result, forks.get(i).join());
			}
		}

		return result;
	}
}

class ParallelForEachIterationTask<T> extends RecursiveTask<T> {
	private static final long serialVersionUID = 1L;
	T input;
	ParallelForEachDelegate<T> delegate;

	public ParallelForEachIterationTask(ParallelForEachDelegate<T> delegate,
			T input) {
		this.delegate = delegate;
		this.input = input;
	}

	@Override
	protected T compute() {
		return delegate.delegate(input);
	}
}

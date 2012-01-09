using System;
using System.Collections.Generic;

namespace SourceAFIS.Dummy
{
    public static class Parallel
    {
        public static void For(int begin, int end, Action<int> action)
        {
            for (int i = begin; i < end; ++i)
                action(i);
        }

        public static void For<T>(int begin, int end, Func<T> initializer, Func<int, ParallelLoopState, T, T> action, Action<T> finalizer)
        {
            T context = initializer();
            try
            {
                for (int i = begin; i < end; ++i)
                    context = action(i, null, context);
            }
            finally
            {
                finalizer(context);
            }
        }

        public static void ForEach<T>(IEnumerable<T> list, Action<T> action)
        {
            foreach (T item in list)
                action(item);
        }

        public static void Invoke(params Action[] actions)
        {
            foreach (var action in actions)
                action();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace Berrysoft.Data
{
    /// <summary>
    /// Provides a set of <see langword="static"/> methods for querying objects.
    /// </summary>
    public static partial class Enumerable
    {
        /// <summary>
        /// Projects each element of a sequence into a new form, based on a predicate.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by <paramref name="predicate"/>.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{T}"/> to filter and invoke a transform function on.</param>
        /// <param name="predicate">A function to test each source element for a condition and tramsform each element.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> whose elements are the result of invoking the transform function on each element of source.</returns>
        /// <exception cref="ArgumentNullException">When <paramref name="source"/> or <paramref name="predicate"/> is <see langword="null"/>.</exception>
        public static IEnumerable<TResult> SelectWhen<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, (bool Select, TResult Result)> predicate)
            => SelectWhenIterator(source ?? throw ExceptionHelper.ArgumentNull(nameof(source)), predicate ?? throw ExceptionHelper.ArgumentNull(nameof(predicate)));
        /// <summary>
        /// Get an iterator projects each element of a sequence into a new form, based on a predicate.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by <paramref name="predicate"/>.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{T}"/> to filter and invoke a transform function on.</param>
        /// <param name="predicate">A function to test each source element for a condition and tramsform each element.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> whose elements are the result of invoking the transform function on each element of source.</returns>
        private static IEnumerable<TResult> SelectWhenIterator<TSource, TResult>(IEnumerable<TSource> source, Func<TSource, (bool Select, TResult Result)> predicate)
        {
            foreach (TSource item in source)
            {
                var (Select, Result) = predicate(item);
                if (Select)
                {
                    yield return Result;
                }
            }
        }
        /// <summary>
        /// Performs the specified action on each element of the <see cref="IEnumerable{TSource}"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{TSource}"/> to perform.</param>
        /// <param name="action">The <see cref="Action{TSource}"/> delegate to perform on each element of the <see cref="IEnumerable{TSource}"/>.</param>
        /// <returns>Each element of <paramref name="source"/> is yield immediately after the <paramref name="action"/> performs.</returns>
        /// <exception cref="ArgumentNullException">When <paramref name="source"/> is <see langword="null"/>.</exception>
        public static IEnumerable<TSource> ForEach<TSource>(this IEnumerable<TSource> source, Action<TSource> action)
        {
            switch (source ?? throw ExceptionHelper.ArgumentNull(nameof(source)))
            {
                case TSource[] array:
                    return ForEachArrayIterator(array, action);
                case IList<TSource> collection:
                    return ForEachListIterator(collection, action);
                default:
                    return ForEachIterator(source, action);
            }
        }
        /// <summary>
        /// An iterator performs the specified action on each element of the array.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">An array to perform.</param>
        /// <param name="action">The <see cref="Action{TSource}"/> delegate to perform on each element of the <see cref="IEnumerable{TSource}"/>.</param>
        /// <returns>Each element of <paramref name="source"/> is yield immediately after the <paramref name="action"/> performs.</returns>
        private static IEnumerable<TSource> ForEachArrayIterator<TSource>(TSource[] source, Action<TSource> action)
        {
            int n = source.Length;
            for (int i = 0; i < n; i++)
            {
                action(source[i]);
                yield return source[i];
            }
        }
        /// <summary>
        /// An iterator performs the specified action on each element of the <see cref="IList{TSource}"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">An <see cref="IList{TSource}"/> to perform.</param>
        /// <param name="action">The <see cref="Action{TSource}"/> delegate to perform on each element of the <see cref="IEnumerable{TSource}"/>.</param>
        /// <returns>Each element of <paramref name="source"/> is yield immediately after the <paramref name="action"/> performs.</returns>
        private static IEnumerable<TSource> ForEachListIterator<TSource>(IList<TSource> source, Action<TSource> action)
        {
            int n = source.Count;
            for (int i = 0; i < n; i++)
            {
                action(source[i]);
                yield return source[i];
            }
        }
        /// <summary>
        /// An iterator performs the specified action on each element of the <see cref="IEnumerable{TSource}"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{TSource}"/> to perform.</param>
        /// <param name="action">The <see cref="Action{TSource}"/> delegate to perform on each element of the <see cref="IEnumerable{TSource}"/>.</param>
        /// <returns>Each element of <paramref name="source"/> is yield immediately after the <paramref name="action"/> performs.</returns>
        private static IEnumerable<TSource> ForEachIterator<TSource>(IEnumerable<TSource> source, Action<TSource> action)
        {
            foreach (TSource item in source)
            {
                action(item);
                yield return item;
            }
        }
        /// <summary>
        /// Performs the specified action on each element of the <see cref="IEnumerable{TSource}"/>. Each element's index is used in the logic of the action.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{TSource}"/> to perform.</param>
        /// <param name="action">The <see cref="Action{TSource}"/> delegate to perform on each element of the <see cref="IEnumerable{TSource}"/>.</param>
        /// <returns>Each element of <paramref name="source"/> is yield immediately after the <paramref name="action"/> performs.</returns>
        /// <exception cref="ArgumentNullException">When <paramref name="source"/> is <see langword="null"/>.</exception>
        public static IEnumerable<TSource> ForEach<TSource>(this IEnumerable<TSource> source, Action<TSource, int> action)
        {
            switch (source ?? throw ExceptionHelper.ArgumentNull(nameof(source)))
            {
                case TSource[] array:
                    return ForEachArrayIterator(array, action);
                case IList<TSource> collection:
                    return ForEachListIterator(collection, action);
                default:
                    return ForEachIterator(source, action);
            }
        }
        /// <summary>
        /// An iterator performs the specified action on each element of the array. Each element's index is used in the logic of the action.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">An array to perform.</param>
        /// <param name="action">The <see cref="Action{TSource}"/> delegate to perform on each element of the <see cref="IEnumerable{TSource}"/>.</param>
        /// <returns>Each element of <paramref name="source"/> is yield immediately after the <paramref name="action"/> performs.</returns>
        private static IEnumerable<TSource> ForEachArrayIterator<TSource>(TSource[] source, Action<TSource, int> action)
        {
            int n = source.Length;
            for (int i = 0; i < n; i++)
            {
                action(source[i], i);
                yield return source[i];
            }
        }
        /// <summary>
        /// An iterator performs the specified action on each element of the <see cref="IList{TSource}"/>. Each element's index is used in the logic of the action.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">An <see cref="IList{TSource}"/> to perform.</param>
        /// <param name="action">The <see cref="Action{TSource}"/> delegate to perform on each element of the <see cref="IEnumerable{TSource}"/>.</param>
        /// <returns>Each element of <paramref name="source"/> is yield immediately after the <paramref name="action"/> performs.</returns>
        private static IEnumerable<TSource> ForEachListIterator<TSource>(IList<TSource> source, Action<TSource, int> action)
        {
            int n = source.Count;
            for (int i = 0; i < n; i++)
            {
                action(source[i], i);
                yield return source[i];
            }
        }
        /// <summary>
        /// An iterator performs the specified action on each element of the <see cref="IEnumerable{TSource}"/>. Each element's index is used in the logic of the action.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{TSource}"/> to perform.</param>
        /// <param name="action">The <see cref="Action{TSource}"/> delegate to perform on each element of the <see cref="IEnumerable{TSource}"/>.</param>
        /// <returns>Each element of <paramref name="source"/> is yield immediately after the <paramref name="action"/> performs.</returns>
        private static IEnumerable<TSource> ForEachIterator<TSource>(IEnumerable<TSource> source, Action<TSource, int> action)
        {
            int i = 0;
            foreach (TSource item in source)
            {
                action(item, i);
                yield return item;
                i++;
            }
        }
        /// <summary>
        /// Enumerate the sequence in a random order.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{TSource}"/> to enumerate.</param>
        /// <returns>An <see cref="IEnumerable{TSource}"/> in random order.</returns>
        public static IEnumerable<TSource> Random<TSource>(this IEnumerable<TSource> source)
            => RandomIterator(source ?? throw ExceptionHelper.ArgumentNull(nameof(source)));
        /// <summary>
        /// An iterator to enumerate the sequence in a random order.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{TSource}"/> to enumerate.</param>
        /// <returns>An <see cref="IEnumerable{TSource}"/> in random order.</returns>
        private static IEnumerable<TSource> RandomIterator<TSource>(IEnumerable<TSource> source)
        {
            Random random = new Random();
            List<TSource> list = new List<TSource>(source);
            int n = list.Count;
            for (int i = 0; i < n; i++)
            {
                int index = random.Next(list.Count - 1);
                yield return list[index];
                list.RemoveAt(index);
            }
        }
    }
}

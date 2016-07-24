#region Extensions Attribute

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace System.Runtime.CompilerServices
{
	/// <remarks>
	/// This attribute allows us to define extension methods without 
	/// requiring .NET Framework 3.5. For more information, see the section,
	/// <a href="http://msdn.microsoft.com/en-us/magazine/cc163317.aspx#S7">Extension Methods in .NET Framework 2.0 Apps</a>,
	/// of <a href="http://msdn.microsoft.com/en-us/magazine/cc163317.aspx">Basic Instincts: Extension Methods</a>
	/// column in <a href="http://msdn.microsoft.com/msdnmag/">MSDN Magazine</a>, 
	/// issue <a href="http://msdn.microsoft.com/en-us/magazine/cc135410.aspx">Nov 2007</a>.
	/// </remarks>

	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Assembly)]
	sealed class ExtensionAttribute : Attribute { }
}

#endregion


namespace System
{
	public delegate TResult Func<out TResult>();
    public delegate TResult Func<in T, out TResult>(T a);
    public delegate TResult Func<in T1, in T2, out TResult>(T1 arg1, T2 arg2);
    public delegate TResult Func<in T1, in T2, in T3, out TResult>(T1 arg1, T2 arg2, T3 arg3);
    public delegate TResult Func<in T1, in T2, in T3, in T4, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4);

    public delegate void Action();
    delegate void Action<in T1, in T2>(T1 arg1, T2 arg2);
    delegate void Action<in T1, in T2, in T3>(T1 arg1, T2 arg2, T3 arg3);
    delegate void Action<in T1, in T2, in T3, in T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
}


namespace ZLinq
{
	public static class Enumerable
	{
		/// <summary>
        /// Computes the sum of a sequence of nullable <see cref="System.Int32" /> values.
        /// </summary>
		public static int Sum(this IEnumerable<int> source)
        {
            if (source == null) throw new ArgumentNullException("source");

            var sum = 0;
            foreach (var num in source)
            {
	            sum = checked(sum + num);
            }

            return sum;
        }

        /// <summary>
        /// Computes the sum of a sequence of nullable <see cref="System.Int32" /> 
        /// values that are obtained by invoking a transform function on 
        /// each element of the input sequence.
        /// </summary>
        public static int Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector)
        {
            return source.Select(selector).Sum();
        }


		/// <summary>
        /// Creates a <see cref="Dictionary{TKey,TValue}" /> from an 
        /// <see cref="IEnumerable{T}" /> according to a specified key 
        /// selector function.
        /// </summary>
        public static Dictionary<TKey, TSource> ToDictionary<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            return source.ToDictionary(keySelector, /* comparer */ null);
        }

        /// <summary>
        /// Creates a <see cref="Dictionary{TKey,TValue}" /> from an 
        /// <see cref="IEnumerable{T}" /> according to a specified key 
        /// selector function and key comparer.
        /// </summary>
        public static Dictionary<TKey, TSource> ToDictionary<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            return source.ToDictionary(keySelector, e => e, comparer);
        }

        /// <summary>
        /// Creates a <see cref="Dictionary{TKey,TValue}" /> from an 
        /// <see cref="IEnumerable{T}" /> according to specified key 
        /// selector and element selector functions.
        /// </summary>
        public static Dictionary<TKey, TElement> ToDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
        {
            return source.ToDictionary(keySelector, elementSelector, /* comparer */ null);
        }

        /// <summary>
        /// Creates a <see cref="Dictionary{TKey,TValue}" /> from an 
        /// <see cref="IEnumerable{T}" /> according to a specified key 
        /// selector function, a comparer, and an element selector function.
        /// </summary>
        public static Dictionary<TKey, TElement> ToDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (keySelector == null) throw new ArgumentNullException("keySelector");
            if (elementSelector == null) throw new ArgumentNullException("elementSelector");

            var dict = new Dictionary<TKey, TElement>(comparer);

            foreach (var item in source)
            {
                dict.Add(keySelector(item), elementSelector(item));
            }

            return dict;
        }


		/// <summary>
		/// Returns the input typed as <see cref="IEnumerable{T}"/>.
		/// </summary>
		public static IEnumerable<TSource> AsEnumerable<TSource>(this IEnumerable<TSource> source)
		{
			return source;
		}

		/// <summary>
		/// Returns an empty <see cref="IEnumerable{T}"/> that has the 
		/// specified type argument.
		/// </summary>
		public static IEnumerable<TResult> Empty<TResult>()
		{
			return Sequence<TResult>.Empty;
		}

		/// <summary>
		/// Converts the elements of an <see cref="IEnumerable"/> to the 
		/// specified type.
		/// </summary>
		public static IEnumerable<TResult> Cast<TResult>(this IEnumerable source)
		{
			if (source == null) throw new ArgumentNullException("source");

			return CastYield<TResult>(source);
		}

		private static IEnumerable<TResult> CastYield<TResult>(IEnumerable source)
		{
			foreach (var item in source)
				yield return (TResult)item;
		}

		/// <summary>
		/// Filters the elements of an <see cref="IEnumerable"/> based on a specified type.
		/// </summary>
		public static IEnumerable<TResult> OfType<TResult>(this IEnumerable source)
		{
			if (source == null) throw new ArgumentNullException("source");

			return OfTypeYield<TResult>(source);
		}

		private static IEnumerable<TResult> OfTypeYield<TResult>(IEnumerable source)
		{
			foreach (var item in source)
				if (item is TResult)
					yield return (TResult)item;
		}

		/// <summary>
		/// Generates a sequence of integral numbers within a specified range.
		/// </summary>
		/// <param name="start">The value of the first integer in the sequence.</param>
		/// <param name="count">The number of sequential integers to generate.</param>
		public static IEnumerable<int> Range(int start, int count)
		{
			if (count < 0)
				throw new ArgumentOutOfRangeException("count", count, null);

			var end = (long)start + count;
			if (end - 1 >= int.MaxValue)
				throw new ArgumentOutOfRangeException("count", count, null);

			return RangeYield(start, end);
		}

		private static IEnumerable<int> RangeYield(int start, long end)
		{
			for (var i = start; i < end; i++)
				yield return i;
		}

		/// <summary>
		/// Generates a sequence that contains one repeated value.
		/// </summary>
		public static IEnumerable<TResult> Repeat<TResult>(TResult element, int count)
		{
			if (count < 0) throw new ArgumentOutOfRangeException("count", count, null);

			return RepeatYield(element, count);
		}

		private static IEnumerable<TResult> RepeatYield<TResult>(TResult element, int count)
		{
			for (var i = 0; i < count; i++)
				yield return element;
		}

		/// <summary>
		/// Filters a sequence of values based on a predicate.
		/// </summary>
		public static IEnumerable<TSource> Where<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			if (predicate == null) throw new ArgumentNullException("predicate");

			return source.Where((item, i) => predicate(item));
		}

		/// <summary>
		/// Filters a sequence of values based on a predicate. 
		/// Each element's index is used in the logic of the predicate function.
		/// </summary>
		public static IEnumerable<TSource> Where<TSource>(this IEnumerable<TSource> source, Func<TSource, int, bool> predicate)
		{
			if (source == null) throw new ArgumentNullException("source");
			if (predicate == null) throw new ArgumentNullException("predicate");

			return WhereYield(source, predicate);
		}

		private static IEnumerable<TSource> WhereYield<TSource>(IEnumerable<TSource> source, Func<TSource, int, bool> predicate)
		{
			var i = 0;
			foreach (var item in source)
				if (predicate(item, i++))
					yield return item;
		}

		/// <summary>
		/// Projects each element of a sequence into a new form.
		/// </summary>
		public static IEnumerable<TResult> Select<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
		{
			if (selector == null) throw new ArgumentNullException("selector");

			return source.Select((item, i) => selector(item));
		}

		/// <summary>
		/// Projects each element of a sequence into a new form by 
		/// incorporating the element's index.
		/// </summary>
		public static IEnumerable<TResult> Select<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, int, TResult> selector)
		{
			if (source == null) throw new ArgumentNullException("source");
			if (selector == null) throw new ArgumentNullException("selector");

			return SelectYield(source, selector);
		}

		private static IEnumerable<TResult> SelectYield<TSource, TResult>(IEnumerable<TSource> source, Func<TSource, int, TResult> selector)
		{
			var i = 0;
			foreach (var item in source)
				yield return selector(item, i++);
		}

		private static class Futures<T>
		{
			public static readonly Func<T> Default = () => default(T);
			public static readonly Func<T> Undefined = () => { throw new InvalidOperationException(); };
		}

		/// <summary>
        /// Projects each element of a sequence to an <see cref="IEnumerable{T}" /> 
        /// and flattens the resulting sequences into one sequence.
        /// </summary>
        public static IEnumerable<TResult> SelectMany<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, IEnumerable<TResult>> selector)
        {
            if (selector == null)
				throw new ArgumentNullException("selector");

            return source.SelectMany((item, i) => selector(item));
        }

        /// <summary>
        /// Projects each element of a sequence to an <see cref="IEnumerable{T}" />, and flattens
        /// the resulting sequences into one sequence. The index of each source element is used in the projected form of  that element.
        /// </summary>
        public static IEnumerable<TResult> SelectMany<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, int, IEnumerable<TResult>> selector)
        {
            if (selector == null)
				throw new ArgumentNullException("selector");

            return source.SelectMany(selector, (item, subitem) => subitem);
        }

        /// <summary>
        /// Projects each element of a sequence to an <see cref="IEnumerable{T}" />, flattens the resulting sequences
        /// into one sequence, and invokes a result selector function on each element therein.
        /// </summary>
        public static IEnumerable<TResult> SelectMany<TSource, TCollection, TResult>(
            this IEnumerable<TSource> source,
            Func<TSource, IEnumerable<TCollection>> collectionSelector,
            Func<TSource, TCollection, TResult> resultSelector)
        {
            if (collectionSelector == null)
				throw new ArgumentNullException("collectionSelector");

            return source.SelectMany((item, i) => collectionSelector(item), resultSelector);
        }

        /// <summary>
        /// Projects each element of a sequence to an <see cref="IEnumerable{T}" />, flattens the resulting sequences
        /// into one sequence, and invokes a result selector function on each element therein. The index of each source
        /// element is used in the intermediate projected form of that element.
        /// </summary>
        public static IEnumerable<TResult> SelectMany<TSource, TCollection, TResult>(
            this IEnumerable<TSource> source,
            Func<TSource, int, IEnumerable<TCollection>> collectionSelector,
            Func<TSource, TCollection, TResult> resultSelector)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (collectionSelector == null) throw new ArgumentNullException("collectionSelector");
            if (resultSelector == null) throw new ArgumentNullException("resultSelector");

            return SelectManyYield(source, collectionSelector, resultSelector);
        }

        private static IEnumerable<TResult> SelectManyYield<TSource, TCollection, TResult>(
            this IEnumerable<TSource> source,
            Func<TSource, int, IEnumerable<TCollection>> collectionSelector,
            Func<TSource, TCollection, TResult> resultSelector)
        {
            var i = 0;
            foreach (var item in source)
                foreach (var subitem in collectionSelector(item, i++))
                    yield return resultSelector(item, subitem);
        }

		/// <summary>
		/// Base implementation of First operator.
		/// </summary>
		private static TSource FirstImpl<TSource>(this IEnumerable<TSource> source, Func<TSource> empty)
		{
			if (source == null) throw new ArgumentNullException("source");
			Debug.Assert(empty != null);

			var list = source as IList<TSource>;    // optimized case for lists
			if (list != null)
				return list.Count > 0 ? list[0] : empty();

			using (var e = source.GetEnumerator())  // fallback for enumeration
				return e.MoveNext() ? e.Current : empty();
		}

		/// <summary>
		/// Returns the first element of a sequence.
		/// </summary>
		public static TSource First<TSource>(this IEnumerable<TSource> source)
		{
			return source.FirstImpl(Futures<TSource>.Undefined);
		}

		/// <summary>
		/// Returns the first element in a sequence that satisfies a specified condition.
		/// </summary>
		public static TSource First<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			return First(source.Where(predicate));
		}

		/// <summary>
		/// Returns the first element of a sequence, or a default value if 
		/// the sequence contains no elements.
		/// </summary>
		public static TSource FirstOrDefault<TSource>(this IEnumerable<TSource> source)
		{
			return source.FirstImpl(Futures<TSource>.Default);
		}

		/// <summary>
		/// Returns the first element of the sequence that satisfies a 
		/// condition or a default value if no such element is found.
		/// </summary>
		public static TSource FirstOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			return FirstOrDefault(source.Where(predicate));
		}

		/// <summary>
		/// Base implementation of Last operator.
		/// </summary>
		private static TSource LastImpl<TSource>(this IEnumerable<TSource> source, Func<TSource> empty)
		{
			if (source == null) throw new ArgumentNullException("source");

			var list = source as IList<TSource>;    // optimized case for lists
			if (list != null)
				return list.Count > 0 ? list[list.Count - 1] : empty();

			using (var e = source.GetEnumerator())
			{
				if (!e.MoveNext())
					return empty();

				var last = e.Current;
				while (e.MoveNext())
					last = e.Current;

				return last;
			}
		}

		/// <summary>
		/// Returns the last element of a sequence.
		/// </summary>
		public static TSource Last<TSource>(this IEnumerable<TSource> source)
		{
			return source.LastImpl(Futures<TSource>.Undefined);
		}

		/// <summary>
		/// Returns the last element of a sequence that satisfies a 
		/// specified condition.
		/// </summary>
		public static TSource Last<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			return Last(source.Where(predicate));
		}

		/// <summary>
		/// Returns the last element of a sequence, or a default value if 
		/// the sequence contains no elements.
		/// </summary>
		public static TSource LastOrDefault<TSource>(this IEnumerable<TSource> source)
		{
			return source.LastImpl(Futures<TSource>.Default);
		}

		/// <summary>
		/// Returns the last element of a sequence that satisfies a 
		/// condition or a default value if no such element is found.
		/// </summary>
		public static TSource LastOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			return LastOrDefault(source.Where(predicate));
		}

		/// <summary>
		/// Base implementation of Single operator.
		/// </summary>
		private static TSource SingleImpl<TSource>(this IEnumerable<TSource> source, Func<TSource> empty)
		{
			if (source == null) throw new ArgumentNullException("source");

			using (var e = source.GetEnumerator())
			{
				if (e.MoveNext())
				{
					var single = e.Current;
					if (!e.MoveNext())
						return single;

					throw new InvalidOperationException();
				}

				return empty();
			}
		}

		/// <summary>
		/// Returns the only element of a sequence, and throws an exception 
		/// if there is not exactly one element in the sequence.
		/// </summary>
		public static TSource Single<TSource>(this IEnumerable<TSource> source)
		{
			return source.SingleImpl(Futures<TSource>.Undefined);
		}

		/// <summary>
		/// Returns the only element of a sequence that satisfies a 
		/// specified condition, and throws an exception if more than one 
		/// such element exists.
		/// </summary>
		public static TSource Single<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			return Single(source.Where(predicate));
		}

		/// <summary>
		/// Returns the only element of a sequence, or a default value if 
		/// the sequence is empty; this method throws an exception if there 
		/// is more than one element in the sequence.
		/// </summary>
		public static TSource SingleOrDefault<TSource>(this IEnumerable<TSource> source)
		{
			return source.SingleImpl(Futures<TSource>.Default);
		}

		/// <summary>
		/// Returns the only element of a sequence that satisfies a 
		/// specified condition or a default value if no such element 
		/// exists; this method throws an exception if more than one element 
		/// satisfies the condition.
		/// </summary>
		public static TSource SingleOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			return SingleOrDefault(source.Where(predicate));
		}

		/// <summary>
		/// Inverts the order of the elements in a sequence.
		/// </summary>
		public static IEnumerable<TSource> Reverse<TSource>(this IEnumerable<TSource> source)
		{
			if (source == null) throw new ArgumentNullException("source");

			return ReverseYield(source);
		}

		private static IEnumerable<TSource> ReverseYield<TSource>(IEnumerable<TSource> source)
		{
			var stack = new Stack<TSource>();
			foreach (var item in source)
				stack.Push(item);

			foreach (var item in stack)
				yield return item;
		}


		/// <summary>
		/// Returns the number of elements in a sequence.
		/// </summary>
		public static int Count<TSource>(this IEnumerable<TSource> source)
		{
			if (source == null) throw new ArgumentNullException("source");

			var collection = source as ICollection;
			return collection != null
				 ? collection.Count
				 : source.Aggregate(0, (count, item) => checked(count + 1));
		}

		/// <summary>
		/// Returns a number that represents how many elements in the 
		/// specified sequence satisfy a condition.
		/// </summary>
		public static int Count<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			return Count(source.Where(predicate));
		}

		/// <summary>
		/// Creates a <see cref="List{T}"/> from an <see cref="IEnumerable{T}"/>.
		/// </summary>
		public static List<TSource> ToList<TSource>(this IEnumerable<TSource> source)
		{
			if (source == null) throw new ArgumentNullException("source");

			return new List<TSource>(source);
		}

		/// <summary>
		/// Creates an array from an <see cref="IEnumerable{T}"/>.
		/// </summary>
		public static TSource[] ToArray<TSource>(this IEnumerable<TSource> source)
		{
			return source.ToList().ToArray();
		}

		/// <summary>
		/// Returns distinct elements from a sequence by using the default 
		/// equality comparer to compare values.
		/// </summary>
		public static IEnumerable<TSource> Distinct<TSource>(this IEnumerable<TSource> source)
		{
			return Distinct(source, /* comparer */ null);
		}

		/// <summary>
		/// Returns distinct elements from a sequence by using a specified 
		/// <see cref="IEqualityComparer{T}"/> to compare values.
		/// </summary>
		public static IEnumerable<TSource> Distinct<TSource>(this IEnumerable<TSource> source, IEqualityComparer<TSource> comparer)
		{
			if (source == null) throw new ArgumentNullException("source");

			return DistinctYield(source, comparer);
		}

		private static IEnumerable<TSource> DistinctYield<TSource>(IEnumerable<TSource> source, IEqualityComparer<TSource> comparer)
		{
			var set = new Dictionary<TSource, object>(comparer);
			var gotNull = false;

			foreach (var item in source)
			{
				if (item == null)
				{
					if (gotNull)
						continue;
					gotNull = true;
				}
				else
				{
					if (set.ContainsKey(item))
						continue;
					set.Add(item, null);
				}

				yield return item;
			}
		}

		/// <summary>
		/// Bypasses a specified number of elements in a sequence and then 
		/// returns the remaining elements.
		/// </summary>
		public static IEnumerable<TSource> Skip<TSource>(this IEnumerable<TSource> source, int count)
		{
			return source.SkipWhile((item, i) => i < count);
		}

		public static IEnumerable<TSource> SkipWhile<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			if (predicate == null) throw new ArgumentNullException("predicate");

			return source.SkipWhile((item, i) => predicate(item));
		}

		/// <summary>
		/// Bypasses elements in a sequence as long as a specified condition 
		/// is true and then returns the remaining elements. The element's 
		/// index is used in the logic of the predicate function.
		/// </summary>
		public static IEnumerable<TSource> SkipWhile<TSource>(this IEnumerable<TSource> source, Func<TSource, int, bool> predicate)
		{
			if (source == null) throw new ArgumentNullException("source");
			if (predicate == null) throw new ArgumentNullException("predicate");

			return SkipWhileYield(source, predicate);
		}

		private static IEnumerable<TSource> SkipWhileYield<TSource>(IEnumerable<TSource> source, Func<TSource, int, bool> predicate)
		{
			using (var e = source.GetEnumerator())
			{
				for (var i = 0; ; i++)
				{
					if (!e.MoveNext())
						yield break;

					if (!predicate(e.Current, i))
						break;
				}

				do { yield return e.Current; } while (e.MoveNext());
			}
		}

		/// <summary>
		/// Applies an accumulator function over a sequence.
		/// </summary>
		public static TSource Aggregate<TSource>(this IEnumerable<TSource> source, Func<TSource, TSource, TSource> func)
		{
			if (source == null) throw new ArgumentNullException("source");
			if (func == null) throw new ArgumentNullException("func");

			using (var e = source.GetEnumerator())
			{
				if (!e.MoveNext())
					throw new InvalidOperationException();

				return e.Renumerable().Skip(1).Aggregate(e.Current, func);
			}
		}

		/// <summary>
		/// Applies an accumulator function over a sequence. The specified 
		/// seed value is used as the initial accumulator value.
		/// </summary>
		public static TAccumulate Aggregate<TSource, TAccumulate>(this IEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func)
		{
			return Aggregate(source, seed, func, r => r);
		}

		/// <summary>
		/// Applies an accumulator function over a sequence. The specified 
		/// seed value is used as the initial accumulator value, and the 
		/// specified function is used to select the result value.
		/// </summary>
		public static TResult Aggregate<TSource, TAccumulate, TResult>(this IEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func, Func<TAccumulate, TResult> resultSelector)
		{
			if (source == null) throw new ArgumentNullException("source");
			if (func == null) throw new ArgumentNullException("func");
			if (resultSelector == null) throw new ArgumentNullException("resultSelector");

			var result = seed;

			foreach (var item in source)
				result = func(result, item);

			return resultSelector(result);
		}

		/// <summary>
		/// Returns the elements of the specified sequence or the type 
		/// parameter's default value in a singleton collection if the 
		/// sequence is empty.
		/// </summary>
		public static IEnumerable<TSource> DefaultIfEmpty<TSource>(this IEnumerable<TSource> source)
		{
			return source.DefaultIfEmpty(default(TSource));
		}

		/// <summary>
		/// Returns the elements of the specified sequence or the specified 
		/// value in a singleton collection if the sequence is empty.
		/// </summary>
		public static IEnumerable<TSource> DefaultIfEmpty<TSource>(this IEnumerable<TSource> source, TSource defaultValue)
		{
			if (source == null) throw new ArgumentNullException("source");

			return DefaultIfEmptyYield(source, defaultValue);
		}

		private static IEnumerable<TSource> DefaultIfEmptyYield<TSource>(IEnumerable<TSource> source, TSource defaultValue)
		{
			using (var e = source.GetEnumerator())
			{
				if (!e.MoveNext())
					yield return defaultValue;
				else
					do { yield return e.Current; } while (e.MoveNext());
			}
		}

		/// <summary>
		/// Determines whether all elements of a sequence satisfy a condition.
		/// </summary>
		public static bool All<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			if (source == null) throw new ArgumentNullException("source");
			if (predicate == null) throw new ArgumentNullException("predicate");

			foreach (var item in source)
				if (!predicate(item))
					return false;

			return true;
		}

		/// <summary>
		/// Determines whether a sequence contains any elements.
		/// </summary>
		public static bool Any<TSource>(this IEnumerable<TSource> source)
		{
			if (source == null) throw new ArgumentNullException("source");

			using (var e = source.GetEnumerator())
				return e.MoveNext();
		}

		/// <summary>
		/// Determines whether any element of a sequence satisfies a 
		/// condition.
		/// </summary>
		public static bool Any<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			return source.Where(predicate).Any();
		}

		/// <summary>
		/// Determines whether a sequence contains a specified element by 
		/// using the default equality comparer.
		/// </summary>
		public static bool Contains<TSource>(this IEnumerable<TSource> source, TSource value)
		{
			return source.Contains(value, /* comparer */ null);
		}

		/// <summary>
		/// Determines whether a sequence contains a specified element by 
		/// using a specified <see cref="IEqualityComparer{T}" />.
		/// </summary>
		public static bool Contains<TSource>(this IEnumerable<TSource> source, TSource value, IEqualityComparer<TSource> comparer)
		{
			if (source == null) throw new ArgumentNullException("source");

			if (comparer == null)
			{
				var collection = source as ICollection<TSource>;
				if (collection != null)
					return collection.Contains(value);
			}

			comparer = comparer ?? EqualityComparer<TSource>.Default;
			return source.Any(item => comparer.Equals(item, value));
		}

		/// <summary>
		/// Base implementation for Min/Max operator.
		/// </summary>
		private static TSource MinMaxImpl<TSource>(this IEnumerable<TSource> source, Func<TSource, TSource, bool> lesser)
		{
			if (source == null) throw new ArgumentNullException("source");
			Debug.Assert(lesser != null);

			if (typeof(TSource).IsClass) // ReSharper disable CompareNonConstrainedGenericWithNull                
				source = source.Where(e => e != null).DefaultIfEmpty(); // ReSharper restore CompareNonConstrainedGenericWithNull

			return source.Aggregate((a, item) => lesser(a, item) ? a : item);
		}

		/// <summary>
		/// Returns the minimum value in a generic sequence.
		/// </summary>
		public static TSource Min<TSource>(this IEnumerable<TSource> source)
		{
			var comparer = Comparer<TSource>.Default;
			return source.MinMaxImpl((x, y) => comparer.Compare(x, y) < 0);
		}

		/// <summary>
		/// Invokes a transform function on each element of a generic 
		/// sequence and returns the minimum resulting value.
		/// </summary>
		public static TResult Min<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
		{
			return source.Select(selector).Min();
		}

		/// <summary>
		/// Returns the maximum value in a generic sequence.
		/// </summary>
		public static TSource Max<TSource>(this IEnumerable<TSource> source)
		{
			var comparer = Comparer<TSource>.Default;
			return source.MinMaxImpl((x, y) => comparer.Compare(x, y) > 0);
		}

		/// <summary>
		/// Invokes a transform function on each element of a generic 
		/// sequence and returns the maximum resulting value.
		/// </summary>
		public static TResult Max<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
		{
			return source.Select(selector).Max();
		}

		/// <summary>
		/// Makes an enumerator seen as enumerable once more.
		/// </summary>
		/// <remarks>
		/// The supplied enumerator must have been started. The first element
		/// returned is the element the enumerator was on when passed in.
		/// DO NOT use this method if the caller must be a generator. It is
		/// mostly safe among aggregate operations.
		/// </remarks>
		private static IEnumerable<T> Renumerable<T>(this IEnumerator<T> e)
		{
			Debug.Assert(e != null);

			do { yield return e.Current; } while (e.MoveNext());
		}

		/// <summary>
		/// Sorts the elements of a sequence in ascending order according to a key.
		/// </summary>
		public static IOrderedEnumerable<TSource> OrderBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
		{
			return source.OrderBy(keySelector, /* comparer */ null);
		}

		/// <summary>
		/// Sorts the elements of a sequence in ascending order by using a 
		/// specified comparer.
		/// </summary>
		public static IOrderedEnumerable<TSource> OrderBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
		{
			if (source == null) throw new ArgumentNullException("source");
			if (keySelector == null) throw new ArgumentNullException("keySelector");

			return new OrderedEnumerable<TSource, TKey>(source, keySelector, comparer, /* descending */ false);
		}

		/// <summary>
		/// Sorts the elements of a sequence in descending order according to a key.
		/// </summary>
		public static IOrderedEnumerable<TSource> OrderByDescending<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
		{
			return source.OrderByDescending(keySelector, /* comparer */ null);
		}

		/// <summary>
		///  Sorts the elements of a sequence in descending order by using a 
		/// specified comparer. 
		/// </summary>
		public static IOrderedEnumerable<TSource> OrderByDescending<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
		{
			if (source == null) throw new ArgumentNullException("source");
			if (source == null) throw new ArgumentNullException("keySelector");

			return new OrderedEnumerable<TSource, TKey>(source, keySelector, comparer, /* descending */ true);
		}

		private static class Sequence<T>
		{
			public static readonly IEnumerable<T> Empty = new T[0];
		}

		internal sealed class OrderedEnumerable<T, K> : IOrderedEnumerable<T>
		{
			private readonly IEnumerable<T> _source;
			private readonly Func<T[], IComparer<int>, IComparer<int>> _comparerComposer;

			public OrderedEnumerable(IEnumerable<T> source,
				Func<T, K> keySelector, IComparer<K> comparer, bool descending) :
				this(source, (_, next) => next, keySelector, comparer, descending) { }

			private OrderedEnumerable(IEnumerable<T> source,
				Func<T[], IComparer<int>, IComparer<int>> parent,
				Func<T, K> keySelector, IComparer<K> comparer, bool descending)
			{
				if (source == null) throw new ArgumentNullException("source");
				if (keySelector == null) throw new ArgumentNullException("keySelector");
				Debug.Assert(parent != null);

				_source = source;

				comparer = comparer ?? Comparer<K>.Default;
				var direction = descending ? -1 : 1;

				_comparerComposer = (items, next) =>
				{
					Debug.Assert(items != null);
					Debug.Assert(next != null);

					var keys = new K[items.Length];
					for (var i = 0; i < items.Length; i++)
						keys[i] = keySelector(items[i]);

					return parent(items, new DelegatingComparer<int>((i, j) =>
					{
						var result = direction * comparer.Compare(keys[i], keys[j]);
						return result != 0 ? result : next.Compare(i, j);
					}));
				};
			}

			public IOrderedEnumerable<T> CreateOrderedEnumerable<KK>(
				Func<T, KK> keySelector, IComparer<KK> comparer, bool descending)
			{
				return new OrderedEnumerable<T, KK>(_source, _comparerComposer, keySelector, comparer, descending);
			}

			public IEnumerator<T> GetEnumerator()
			{
				//
				// Sort using Array.Sort but docs say that it performs an 
				// unstable sort. LINQ, on the other hand, says OrderBy performs 
				// a stable sort. Use the item position then as a tie 
				// breaker when all keys compare equal, thus making the sort 
				// stable.
				//

				var items = _source.ToArray();
				var positionComparer = new DelegatingComparer<int>((i, j) => i.CompareTo(j));
				var comparer = _comparerComposer(items, positionComparer);
				var keys = new int[items.Length];
				for (var i = 0; i < keys.Length; i++)
					keys[i] = i;
				Array.Sort(keys, items, comparer);
				return ((IEnumerable<T>)items).GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}
		}


		/// <summary>
		/// Represents a sorted sequence.
		/// </summary>
		public interface IOrderedEnumerable<TElement> : IEnumerable<TElement>
		{
			/// <summary>
			/// Performs a subsequent ordering on the elements of an 
			/// <see cref="IOrderedEnumerable{T}"/> according to a key.
			/// </summary>

			IOrderedEnumerable<TElement> CreateOrderedEnumerable<TKey>(
				Func<TElement, TKey> keySelector, IComparer<TKey> comparer, bool descending);
		}


		/// <remarks>
		/// This type is not intended to be used directly from user code.
		/// It may be removed or changed in a future version without notice.
		/// </remarks>
		sealed class DelegatingComparer<T> : IComparer<T>
		{
			private readonly Func<T, T, int> _comparer;

			public DelegatingComparer(Func<T, T, int> comparer)
			{
				if (comparer == null) throw new ArgumentNullException("comparer");
				_comparer = comparer;
			}

			public int Compare(T x, T y) { return _comparer(x, y); }
		}

		/// <remarks>
		/// This type is not intended to be used directly from user code.
		/// It may be removed or changed in a future version without notice.
		/// </remarks>
		struct Key<T>
		{
			public Key(T value) : this() { Value = value; }
			public T Value { get; private set; }
		}
	}
}
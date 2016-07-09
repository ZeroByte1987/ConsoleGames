using System;
using System.Collections.Generic;



namespace ZFC.Data
{
	/// <summary>
	/// This class defines the set of methods for converting purposes.
	/// </summary>
	public static class ZConvert
	{
		//	(u)Long -> double (u)Ints   &   double (u)Ints -> (u)Long
		#region
		/// <summary>
		/// Converts a long value to array with two uint values.
		/// </summary>
		/// <param name="N">Source long value.</param>
		/// <returns>Returns the resulting array with two uint values.</returns>
		public static uint[]		LongToDoubleUInt(long N)
		{	return LongToDoubleUInt((ulong)N);	}
		/// <summary>
		/// Converts a ulong value to array with two uint values.
		/// </summary>
		/// <param name="N">Source ulong value.</param>
		/// <returns>Returns the resulting array with two uint values.</returns>
		public static uint[]		LongToDoubleUInt(ulong N)
		{
			uint a1 = (uint)(N & uint.MaxValue);
			uint a2 = (uint)(N >> 32);
			return new uint[] { a1, a2 };
		}
		/// <summary>
		/// Converts a long value to array with two int values.
		/// </summary>
		/// <param name="N">Source long value.</param>
		/// <returns>Returns the resulting array with two int values.</returns>
		public static int[]			LongToDoubleInt(long N)
		{	return LongToDoubleInt((ulong)N);	}
		/// <summary>
		/// Converts a ulong value to array with two int values.
		/// </summary>
		/// <param name="N">Source ulong value.</param>
		/// <returns>Returns the resulting array with two int values.</returns>
		public static int[]			LongToDoubleInt(ulong N)
		{
			int a1 = (int)(N & uint.MaxValue);
			int a2 = (int)(N >> 32);
			return new int[] { a1, a2 };
		}

		/// <summary>
		/// Converts double int value to long value.
		/// </summary>
		/// <param name="N1">Lesser significant int value.</param>
		/// <param name="N2">Most significant int value.</param>
		/// <returns>Returns the resulting long value.</returns>
		public static long			DoubleIntToLong(int N1, int N2)
		{	return (N2 << 32) + N1;		}
		/// <summary>
		/// Converts double uint value to long value.
		/// </summary>
		/// <param name="N1">Lesser significant uint value.</param>
		/// <param name="N2">Most significant uint value.</param>
		/// <returns>Returns the resulting long value.</returns>
		public static long			DoubleIntToLong(uint N1, uint N2)
		{	return (N2 << 32) + N1;		}
		/// <summary>
		/// Converts double int value to ulong value.
		/// </summary>
		/// <param name="N1">Lesser significant int value.</param>
		/// <param name="N2">Most significant int value.</param>
		/// <returns>Returns the resulting ulong value.</returns>
		public static ulong			DoubleIntToULong(int N1, int N2)
		{	return (ulong)((N2 << 32) + N1);		}
		/// <summary>
		/// Converts double uint value to ulong value.
		/// </summary>
		/// <param name="N1">Lesser significant uint value.</param>
		/// <param name="N2">Most significant uint value.</param>
		/// <returns>Returns the resulting ulong value.</returns>
		public static ulong			DoubleIntToULong(uint N1, uint N2)
		{	return (N2 << 32) + N1;		}
		#endregion
	}
}
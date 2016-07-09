namespace ZFC.Maths
{
	using System.Collections.Generic;


	/// <summary>
	/// This class defines the set of methods for math operations.
	/// </summary>
	public static class ZMath
	{
		#region Misc. like GetEven, GetBool, GetByte, GetBound

		/// <summary>
		/// Gets whether the specified value belongs to a specified range.
		/// </summary>
		/// <param name="value">Value to check.</param>
		/// <param name="minValue">Minimum possible value.</param>
		/// <param name="maxValue">Maximum possible value.</param>
		/// <returns>Returns TRUE if specified value is in a specified range, otherwise returns FALSE.</returns>
		public static bool		IsInRange(int value, int minValue, int maxValue)
		{
			return value >= minValue  &&  value <= maxValue;
		}

		/// <summary>
		/// Gets the value bounded by specified range.
		/// </summary>
		/// <param name="value">Value to bound.</param>
		/// <param name="minValue">Minimum possible value.</param>
		/// <param name="maxValue">Maximum possible value.</param>
		/// <returns>A value bounded by specified range.</returns>
		public static int		GetBound(int value, int minValue, int maxValue)
		{
			return (value < minValue) ? minValue : (value > maxValue) ? maxValue : value;
		}
		/// <summary>
		/// Gets the float value bounded by specified range.
		/// </summary>
		/// <param name="value">Float value to bound.</param>
		/// <param name="minValue">Minimum possible value.</param>
		/// <param name="maxValue">Maximum possible value.</param>
		/// <returns>A float value bounded by specified range.</returns>
		public static float		GetBound(float value, float minValue, float maxValue)
		{
			return (value < minValue) ? minValue : (value > maxValue) ? maxValue : value;
		}

		/// <summary>
		/// Gets the closest equal or greater even number to the specified one.
		/// </summary>
		/// <param name="value">Source integer value.</param>
		/// <returns>Returns the closest equal or greater even number to the specified one.</returns>
		public static int		GetEven(int value)
		{
			return value + (value % 2);
		}
		/// <summary>
		/// Gets the boolean value from integer.
		/// </summary>
		/// <param name="value">Source integer value.</param>
		/// <returns>Returns TRUE if Value is not 0, otherwise returns FALSE.</returns>
		public static bool		GetBool(int value)
		{
			return (value != 0);
		}
		/// <summary>
		/// Gets the byte value from boolean value.
		/// </summary>
		/// <param name="value">Source boolean value.</param>
		/// <returns>Returns 1 if Value is true, otherwise returns 0.</returns>
		public static byte		GetByte(bool value)
		{
			return value ? (byte)1 : (byte)0;
		}

		/// <summary>
		/// Gets the specified value aligned by block size of 4 (so 7=8, 9=12, etc).
		/// </summary>
		/// <param name="value">The value to align.</param>
		/// <returns>Returns the specified value aligned by block size of 4.</returns>
		public static long		Align(long value)
		{
			return Align(value, 4);
		}
		/// <summary>
		/// Gets the specified value aligned by specified block size.
		/// </summary>
		/// <param name="value">The value to align.</param>
		/// <param name="blockSize">The block size to align to.</param>
		/// <returns>Returns the specified value aligned by specified block size.</returns>
		public static long		Align(long value, int blockSize)
		{
			if ((value % blockSize) != 0)	
				value += (blockSize - (value % blockSize));
			return value;
		}

		#endregion


		#region Sum arrays and listsc

		/// <summary>
		/// Returns the sum of a given array on bytes.
		/// </summary>
		/// <param name="arrayToSum">The array of bytes to sum.</param>
		/// <returns>Returns the sum of all values in this array.</returns>
		public static int		GetArraySum(byte[] arrayToSum)
		{
			int result = 0;
			for (int i = 0; i < arrayToSum.Length; i++)
				result += arrayToSum[i];
			return result;
		}
		/// <summary>
		/// Returns the sum of a given array on integer values.
		/// </summary>
		/// <param name="arrayToSum">The array of integers to sum.</param>
		/// <returns>Returns the sum of all values in this array.</returns>
		public static long		GetArraySum(int[] arrayToSum)
		{
			long result = 0;
			for (int i = 0; i < arrayToSum.Length; i++)
				result += arrayToSum[i];
			return result;
		}
		/// <summary>
		/// Returns the sum of a given array on float values.
		/// </summary>
		/// <param name="arrayToSum">The array of float values to sum.</param>
		/// <returns>Returns the sum of all values in this array.</returns>
		public static float		GetArraySum(float[] arrayToSum)
		{
			float result = 0;
			for (int i = 0; i < arrayToSum.Length; i++)
				result += arrayToSum[i];
			return result;
		}

		/// <summary>
		/// Returns the sum of a given list on bytes.
		/// </summary>
		/// <param name="listToSum">The list of bytes to sum.</param>
		/// <returns>Returns the sum of all values in this list.</returns>
		public static long		Sum_List(List<byte> listToSum)
		{
			return GetArraySum(listToSum.ToArray());
		}
		/// <summary>
		/// Returns the sum of a given list on integer values.
		/// </summary>
		/// <param name="listToSum">The list of integers to sum.</param>
		/// <returns>Returns the sum of all values in this list.</returns>
		public static long		Sum_List(List<int> listToSum)
		{
			return GetArraySum(listToSum.ToArray());
		}
		/// <summary>
		/// Returns the sum of a given list on float values.
		/// </summary>
		/// <param name="listToSum">The list of float values to sum.</param>
		/// <returns>Returns the sum of all values in this list.</returns>
		public static float		Sum_List(List<float> listToSum)
		{
			return GetArraySum(listToSum.ToArray());
		}

		#endregion


		#region Covnert Nullable to non-Nullable

		/// <summary>
		/// Gets the value from nullable boolean value.
		/// </summary>
		/// <param name="val">Nullable boolean value.</param>
		/// <returns>Returns the non-nullable boolean value.</returns>
		public static bool		Get_Val(bool? val)		{	return val != null  &&  val.Value;	}

		/// <summary>
		/// Gets the value from nullable sbyte value.
		/// </summary>
		/// <param name="val">Nullable sbyte value.</param>
		/// <returns>Returns the non-nullable sbyte value.</returns>
		public static sbyte		Get_Val(sbyte? val)		{	return (sbyte) (val != null ? val.Value : 0);	}
		/// <summary>
		/// Gets the value from nullable byte value.
		/// </summary>
		/// <param name="val">Nullable byte value.</param>
		/// <returns>Returns the non-nullable byte value.</returns>
		public static byte		Get_Val(byte? val)		{	return (byte)  (val != null ? val.Value : 0);	}
		/// <summary>
		/// Gets the value from nullable short value.
		/// </summary>
		/// <param name="val">Nullable short value.</param>
		/// <returns>Returns the non-nullable short value.</returns>
		public static short		Get_Val(short? val)		{	return (short) (val != null ? val.Value : 0);	}
		/// <summary>
		/// Gets the value from nullable ushort value.
		/// </summary>
		/// <param name="val">Nullable ushort value.</param>
		/// <returns>Returns the non-nullable ushort value.</returns>
		public static ushort	Get_Val(ushort? val)	{	return (ushort)(val != null ? val.Value : 0);	}
		/// <summary>
		/// Gets the value from nullable integer value.
		/// </summary>
		/// <param name="val">Nullable integer value.</param>
		/// <returns>Returns the non-nullable integer value.</returns>
		public static int		Get_Val(int? val)		{	return val != null ? val.Value : 0;		}

		/// <summary>
		/// Gets the value from nullable uint value.
		/// </summary>
		/// <param name="val">Nullable uint value.</param>
		/// <returns>Returns the non-nullable uint value.</returns>
		public static uint		Get_Val(uint? val)		{	return val != null ? val.Value : 0;		}
		/// <summary>
		/// Gets the value from nullable long value.
		/// </summary>
		/// <param name="val">Nullable long value.</param>
		/// <returns>Returns the non-nullable long value.</returns>
		public static long		Get_Val(long? val)		{	return val != null ? val.Value : 0;		}
		/// <summary>
		/// Gets the value from nullable ulong value.
		/// </summary>
		/// <param name="val">Nullable ulong value.</param>
		/// <returns>Returns the non-nullable ulong value.</returns>
		public static ulong		Get_Val(ulong? val)		{	return val != null ? val.Value : 0;		}
		/// <summary>
		/// Gets the value from nullable float value.
		/// </summary>
		/// <param name="val">Nullable float value.</param>
		/// <returns>Returns the non-nullable float value.</returns>
		public static float		Get_Val(float? val)		{	return val != null ? val.Value : 0;		}
		/// <summary>
		/// Gets the value from nullable double value.
		/// </summary>
		/// <param name="val">Nullable double value.</param>
		/// <returns>Returns the non-nullable double value.</returns>
		public static double	Get_Val(double? val)	{	return val != null ? val.Value : 0;		}

		#endregion
	}
}

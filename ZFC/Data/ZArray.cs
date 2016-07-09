namespace ZFC.Data
{
	using System;


	/// <summary>
	/// This class defines the set of static methods for array operations.
	/// </summary>
	public static class ZArray
	{
		#region Get byte array from everything
		
		private static byte[]	get_Bytes(Array sourceArray, int itemSize)
		{
			var resultArray = new byte[sourceArray.Length*itemSize];
			Buffer.BlockCopy(sourceArray, 0, resultArray, 0, resultArray.Length);
			return resultArray;
		}


		/// <summary>
		/// Get byte array from array of short integers.
		/// </summary>
		/// <param name="sourceArray">Source array.</param>
		/// <returns>Returns resulting byte array.</returns>
		public static byte[]	Get_Bytes(short[] sourceArray)
		{
			return get_Bytes(sourceArray, 2);
		}
		/// <summary>
		/// Get byte array from array of unsigned short integers.
		/// </summary>
		/// <param name="sourceArray">Source array.</param>
		/// <returns>Returns resulting byte array.</returns>
		public static byte[]	Get_Bytes(ushort[] sourceArray)
		{
			return get_Bytes(sourceArray, 2);
		}
		/// <summary>
		/// Get byte array from array of unsigned integers.
		/// </summary>
		/// <param name="sourceArray">Source array.</param>
		/// <returns>Returns resulting byte array.</returns>
		public static byte[]	Get_Bytes(uint[] sourceArray)
		{
			return get_Bytes(sourceArray, 4);
		}
		/// <summary>
		/// Get byte array from array of integers.
		/// </summary>
		/// <param name="sourceArray">Source array.</param>
		/// <returns>Returns resulting byte array.</returns>
		public static byte[]	Get_Bytes(int[] sourceArray)
		{
			return get_Bytes(sourceArray, 4);
		}
		/// <summary>
		/// Get byte array from array of float values.
		/// </summary>
		/// <param name="sourceArray">Source array.</param>
		/// <returns>Returns resulting byte array.</returns>
		public static byte[]	Get_Bytes(float[] sourceArray)
		{
			return get_Bytes(sourceArray, 4);
		}
		/// <summary>
		/// Get byte array from array of double values.
		/// </summary>
		/// <param name="sourceArray">Source array.</param>
		/// <returns>Returns resulting byte array.</returns>
		public static byte[]	Get_Bytes(double[] sourceArray)
		{
			return get_Bytes(sourceArray, 8);
		}
		/// <summary>
		/// Get byte array from array of chars.
		/// </summary>
		/// <param name="sourceArray">Source array.</param>
		/// <returns>Returns resulting byte array.</returns>
		public static byte[]	Get_Bytes(char[] sourceArray)
		{
			return get_Bytes(sourceArray, 2);
		}
		/// <summary>
		/// Get byte array from string.
		/// </summary>
		/// <param name="sourceArray">Source array.</param>
		/// <returns>Returns resulting byte array.</returns>
		public static byte[]	Get_Bytes(string sourceArray)
		{
			return get_Bytes(sourceArray.ToCharArray(), 2);
		}

		#endregion


		#region Write a value in the middle of given array

		/// <summary>
		/// Writes the unsigned integer value into byte array at specified index.
		/// </summary>
		/// <param name="destArray">Destination byte array.</param>
		/// <param name="index">Index at which the specified value should be written.</param>
		/// <param name="value">Integer value to write into array.</param>
		public static void		Write_ToArray(ref byte[] destArray, int index, uint value)
		{
			byte[] cv = BitConverter.GetBytes(value);
			for (int i = 0; i < 4; i++)		
				destArray[i+index] = cv[i];
		}
		/// <summary>
		/// Writes the integer value into byte array at specified index.
		/// </summary>
		/// <param name="destArray">Destination byte array.</param>
		/// <param name="index">Index at which the specified value should be written.</param>
		/// <param name="value">Integer value to write into array.</param>
		public static void		Write_ToArray(ref byte[] destArray, int index, int value)
		{
			Write_ToArray(ref destArray, index, (uint)value);
		}
		/// <summary>
		/// Writes the unsigned short integer value into byte array at specified index.
		/// </summary>
		/// <param name="destArray">Destination byte array.</param>
		/// <param name="index">Index at which the specified value should be written.</param>
		/// <param name="value">Integer value to write into array.</param>
		public static void		Write_ToArray(ref byte[] destArray, int index, ushort value)
		{
			byte[] cv = BitConverter.GetBytes(value);
			for (int i = 0; i < 2; i++)		
				destArray[i+index] = cv[i];
		}
		/// <summary>
		/// Writes the short integer value into byte array at specified index.
		/// </summary>
		/// <param name="destArray">Destination byte array.</param>
		/// <param name="index">Index at which the specified value should be written.</param>
		/// <param name="value">Integer value to write into array.</param>
		public static void		Write_ToArray(ref byte[] destArray, int index, short value)
		{
			Write_ToArray(ref destArray, index, (ushort)value);
		}
		/// <summary>
		/// Writes the byte array into another byte array at specified index.
		/// </summary>
		/// <param name="destArray">Destination byte array.</param>
		/// <param name="index">Index at which the specified value should be written.</param>
		/// <param name="value">Source byte array.</param>
		public static void		Write_ToArray(ref byte[] destArray, int index, byte[] value)
		{
			Array.Copy(value, index, destArray, 0, value.Length);
		}

		#endregion


		#region Copy Arrays

		/// <summary>
		/// Copy a byte array instance.
		/// </summary>
		/// <param name="sourceArray">Source byte array.</param>
		/// <returns>Returns a copy of specified byte array.</returns>
		public static byte[]	Copy_Array(byte[] sourceArray)
		{
			if (sourceArray == null)
				return null;
			return Copy_Array(sourceArray, 0, sourceArray.Length);
		}
		/// <summary>
		/// Copy a byte array instance from specified index.
		/// </summary>
		/// <param name="sourceArray">Source byte array.</param>
		/// <param name="index">Index at which copying should be started.</param>
		/// <returns>Returns a copy of specified byte array from specified index.</returns>
		public static byte[]	Copy_Array(byte[] sourceArray, int index)
		{
			if (sourceArray == null)
				return null;
			return Copy_Array(sourceArray, index, sourceArray.Length - index);
		}
		/// <summary>
		/// Copy a byte array instance with specified index and length.
		/// </summary>
		/// <param name="sourceArray">Source byte array.</param>
		/// <param name="index">Index at which copying should be started.</param>
		/// <param name="length">Count of bytes to copy.</param>
		/// <returns>Returns a copy of specified piece in byte array.</returns>
		public static byte[]	Copy_Array(byte[] sourceArray, int index, int length)
		{
			if (sourceArray == null)
				return null;
			var resultArray = new byte[length];
			Array.Copy(sourceArray, index, resultArray, 0, length);
			return resultArray;
		}

		/// <summary>
		/// Copy an object array instance.
		/// </summary>
		/// <param name="sourceArray">Source object array.</param>
		/// <returns>Returns a copy of specified object array.</returns>
		public static T[]		Copy_Array<T>(T[] sourceArray)
		{
			if (sourceArray == null)		return null;
			return Copy_Array(sourceArray, 0, sourceArray.Length);
		}
		/// <summary>
		/// Copy an object array instance with specified index and length.
		/// </summary>
		/// <param name="sourceArray">Source object array.</param>
		/// <param name="index">Index at which copying should be started.</param>
		/// <returns>Returns a copy of specified piece in object array.</returns>
		public static T[]		Copy_Array<T>(T[] sourceArray, int index)
		{
			if (sourceArray == null)		return null;
			return Copy_Array(sourceArray, index, sourceArray.Length-index);
		}
		/// <summary>
		/// Copy an object array instance with specified index and length.
		/// </summary>
		/// <param name="sourceArray">Source object array.</param>
		/// <param name="index">Index at which copying should be started.</param>
		/// <param name="length">Count of elements to copy.</param>
		/// <returns>Returns a copy of specified piece in object array.</returns>
		public static T[]		Copy_Array<T>(T[] sourceArray, int index, int length)
		{
			if (sourceArray == null)		return null;
			var B = new T[sourceArray.Length];
			sourceArray.CopyTo(B, 0);
			return B;
		}
		#endregion


		#region Misc.

		/// <summary>
		/// Get integer from byte array.
		/// </summary>
		/// <param name="sourceArray">Byte array to convert.</param>
		/// <returns>Returns result integer value.</returns>
		public static uint			Get_Number(byte[] sourceArray)
		{
			uint resultValue = 0;
			switch (sourceArray.Length)
			{
				case 1:	resultValue = sourceArray[0];							break;
				case 2:	resultValue = BitConverter.ToUInt16(sourceArray, 0);	break;
				case 3: resultValue = (uint)(sourceArray[0] + BitConverter.ToUInt16(sourceArray, 1) * 256);	break;
				case 4:	resultValue = BitConverter.ToUInt32(sourceArray, 0);	break;
			}
			return resultValue;
		}

		#endregion
	}
}

using System;
using System.Collections;



namespace ZFC.Data
{
	/// <summary>
	/// This class defines the set of methods for bitwise operations.
	/// </summary>
	public static class	ZBits
	{
		//	Get bits
		#region
		/// <summary>
		/// Gets whether the bit with specified index is set.
		/// </summary>
		/// <param name="Val">Integer value to check.</param>
		/// <param name="BitIndex">Index of the bit to check.</param>
		/// <returns>Returns TRUE if specified bit is set, otherwise returns FALSE.</returns>
		public static bool		GetBit(int Val,		int BitIndex)
		{ return GetBit((uint)Val, BitIndex); }
		/// <summary>
		/// Gets whether the bit with specified index is set.
		/// </summary>
		/// <param name="Val">Unsigned integer value to check.</param>
		/// <param name="BitIndex">Index of the bit to check.</param>
		/// <returns>Returns TRUE if specified bit is set, otherwise returns FALSE.</returns>
		public static bool		GetBit(uint Val,	int BitIndex)
		{
			return ((Val >> BitIndex) & 1) == 1;
		}

		/// <summary>
		/// Gets the integer value for a specified group of bits in specified unsigned integer value.
		/// </summary>
		/// <param name="Val">The source unsigned integer value to get bits from.</param>
		/// <param name="StartIndex">Index of the first useful bit.</param>
		/// <param name="Count">Count of bits to get.</param>
		/// <returns>Returns the integer value for a specified group of bits.</returns>
		public static int		GetBits(uint Val,	int StartIndex, int Count)
		{
			Val = Val >> StartIndex;
			uint mask = (uint)(1 << Count);
			return (int)(Val & --mask);
		}
		/// <summary>
		/// Gets the integer value for a specified group of bits in specified signed integer value.
		/// </summary>
		/// <param name="Val">>The source integer value to get bits from.</param>
		/// <param name="StartIndex">Index of the first useful bit.</param>
		/// <param name="Count">Count of bits to get.</param>
		/// <returns>Returns the integer value for a specified group of bits.</returns>
		public static int		GetBits(int Val,	int StartIndex, int Count)
		{ return GetBits((uint)Val, StartIndex, Count); }
		/// <summary>
		/// Gets the short integer value for a specified group of bits in specified unsigned integer value.
		/// </summary>
		/// <param name="Val">The source unsigned integer value to get bits from.</param>
		/// <param name="StartIndex">Index of the first useful bit.</param>
		/// <param name="Count">Count of bits to get.</param>
		/// <returns>Returns the short integer value for a specified group of bits.</returns>
		public static short		GetBitsS(uint Val,	int StartIndex, int Count)
		{
			Val = Val >> StartIndex;
			uint mask = (uint)(1 << Count);
			return (short)(Val & --mask);
		}
		/// <summary>
		/// Gets the short integer value for a specified group of bits in specified signed integer value.
		/// </summary>
		/// <param name="Val">>The source integer value to get bits from.</param>
		/// <param name="StartIndex">Index of the first useful bit.</param>
		/// <param name="Count">Count of bits to get.</param>
		/// <returns>Returns the short integer value for a specified group of bits.</returns>
		public static short		GetBitsS(int Val,	int StartIndex, int Count)
		{ return GetBitsS((uint)Val, StartIndex, Count); }
		/// <summary>
		/// Gets the number value for a specified group of bits in specified unsigned integer value.
		/// </summary>
		/// <param name="Val">The source unsigned integer value to get bits from.</param>
		/// <param name="StartIndex">Index of the first useful bit.</param>
		/// <param name="Count">Count of bits to get.</param>
		/// <returns>Returns the byte value for a specified group of bits.</returns>
		public static byte		GetBitsB(uint Val,	int StartIndex, int Count)
		{
			Val = Val >> StartIndex;
			uint mask = (uint)(1 << Count);
			return (byte)(Val & --mask);
		}
		/// <summary>
		/// Gets the number value for a specified group of bits in specified signed integer value.
		/// </summary>
		/// <param name="Val">>The source integer value to get bits from.</param>
		/// <param name="StartIndex">Index of the first useful bit.</param>
		/// <param name="Count">Count of bits to get.</param>
		/// <returns>Returns the byte value for a specified group of bits.</returns>
		public static byte		GetBitsB(int Val,	int StartIndex, int Count)
		{ return GetBitsB((uint)Val, StartIndex, Count); }
		#endregion

		//	Toggle / Clear bits
		#region
		/// <summary>
		/// Toggles the specified bit in specified byte value (0->1 , 1->0).
		/// </summary>
		/// <param name="DestVal">The destination byte value.</param>
		/// <param name="BitIndex">Index of the bit to toggle.</param>
		/// <returns>Returns the result byte value.</returns>
		public static byte		ToggleBit(byte DestVal, int BitIndex)
		{ return (byte)(DestVal ^ (byte)(1 << BitIndex)); }
		/// <summary>
		/// Toggles the specified bit in specified ushort value (0->1 , 1->0).
		/// </summary>
		/// <param name="DestVal">The destination ushort value.</param>
		/// <param name="BitIndex">Index of the bit to toggle.</param>
		/// <returns>Returns the result ushort value.</returns>
		public static ushort	ToggleBit(ushort DestVal, int BitIndex)
		{ return (ushort)(DestVal ^ (ushort)(1 << BitIndex)); }
		/// <summary>
		/// Toggles the specified bit in specified uint value (0->1 , 1->0).
		/// </summary>
		/// <param name="DestVal">The destination uint value.</param>
		/// <param name="BitIndex">Index of the bit to toggle.</param>
		/// <returns>Returns the result uint value.</returns>
		public static uint		ToggleBit(uint DestVal, int BitIndex)
		{ return (uint)(DestVal ^ (uint)(1 << BitIndex)); }

		/// <summary>
		/// Clears the specified bit in specified byte value.
		/// </summary>
		/// <param name="DestVal">The destination byte value.</param>
		/// <param name="BitIndex">Index of the bit to clear.</param>
		/// <returns>Returns the result byte value.</returns>
		public static byte		ClearBit(byte DestVal, int BitIndex)
		{ return (byte)(DestVal & (byte)(~(1 << BitIndex))); }
		/// <summary>
		/// Clears the specified bit in specified ushort value.
		/// </summary>
		/// <param name="DestVal">The destination ushort value.</param>
		/// <param name="BitIndex">Index of the bit to clear.</param>
		/// <returns>Returns the result ushort value.</returns>
		public static ushort	ClearBit(ushort DestVal, int BitIndex)
		{ return (ushort)(DestVal & (ushort)(~(1 << BitIndex))); }
		/// <summary>
		/// Clears the specified bit in specified uint value.
		/// </summary>
		/// <param name="DestVal">The destination uint value.</param>
		/// <param name="BitIndex">Index of the bit to clear.</param>
		/// <returns>Returns the result uint value.</returns>
		public static uint		ClearBit(uint DestVal, int BitIndex)
		{ return (uint)(DestVal & (uint)(~(1 << BitIndex))); }
		#endregion

		//	Set bits
		#region
		/// <summary>
		/// Sets the specified bit in specified byte value.
		/// </summary>
		/// <param name="DestVal">The destination byte value.</param>
		/// <param name="BitIndex">Index of the bit to set.</param>
		/// <returns>Returns the result byte value.</returns>
		public static byte		SetBit(byte DestVal,	int BitIndex)
		{ return (byte)(DestVal | (1 << BitIndex)); }
		/// <summary>
		/// Sets the specified bit in specified ushort value.
		/// </summary>
		/// <param name="DestVal">The destination ushort value.</param>
		/// <param name="BitIndex">Index of the bit to set.</param>
		/// <returns>Returns the result ushort value.</returns>
		public static ushort	SetBit(ushort DestVal,	int BitIndex)
		{ return (ushort)(DestVal | (1 << BitIndex)); }
		/// <summary>
		/// Sets the specified bit in specified uint value.
		/// </summary>
		/// <param name="DestVal">The destination uint value.</param>
		/// <param name="BitIndex">Index of the bit to set.</param>
		/// <returns>Returns the result uint value.</returns>
		public static uint		SetBit(uint DestVal,	int BitIndex)
		{ return (uint)(DestVal | (uint)(1 << BitIndex)); }

		/// <summary>
		/// Sets the specified bit in specified byte value.
		/// </summary>
		/// <param name="DestVal">The destination byte value.</param>
		/// <param name="BitIndex">Index of the bit to set.</param>
		/// <param name="BitValue">The bit value to set.</param>
		/// <returns>Returns the result byte value.</returns>
		public static byte		SetBit(byte DestVal,	int BitIndex, bool BitValue)
		{
			DestVal &= (byte)(~(1 << BitIndex));
			return (byte)(DestVal | (1 << BitIndex));
		}
		/// <summary>
		/// Sets the specified bit in specified ushort value.
		/// </summary>
		/// <param name="DestVal">The destination ushort value.</param>
		/// <param name="BitIndex">Index of the bit to set.</param>
		/// <param name="BitValue">The bit value to set.</param>
		/// <returns>Returns the result ushort value.</returns>
		public static ushort	SetBit(ushort DestVal,	int BitIndex, bool BitValue)
		{
			DestVal &= (ushort)(~(1 << BitIndex));
			return (ushort)(DestVal | (1 << BitIndex));
		}
		/// <summary>
		/// Sets the specified bit in specified uint value.
		/// </summary>
		/// <param name="DestVal">The destination uint value.</param>
		/// <param name="BitIndex">Index of the bit to set.</param>
		/// <param name="BitValue">The bit value to set.</param>
		/// <returns>Returns the result uint value.</returns>
		public static uint		SetBit(uint DestVal,	int BitIndex, bool BitValue)
		{
			DestVal &= (uint)(~(1 << BitIndex));
			return (uint)(DestVal | (uint)(1 << BitIndex));
		}

		/// <summary>
		/// Sets the specified count of bits in specified byte value.
		/// </summary>
		/// <param name="DestVal">The destination byte value.</param>
		/// <param name="BitIndex">Index of the bit to set.</param>
		/// <param name="Count">Count of bits to set.</param>
		/// <returns>Returns the result byte value.</returns>
		public static byte		SetBits(byte DestVal,	int BitIndex, int Count)
		{
			uint mask = (uint)(1 << Count) - 1;
			return (byte)(DestVal | (mask << BitIndex));
		}
		/// <summary>
		/// Sets the specified count of bits in specified ushort value.
		/// </summary>
		/// <param name="DestVal">The destination ushort value.</param>
		/// <param name="BitIndex">Index of the bit to set.</param>
		/// <param name="Count">Count of bits to set.</param>
		/// <returns>Returns the result ushort value.</returns>
		public static ushort	SetBits(ushort DestVal, int BitIndex, int Count)
		{
			uint mask = (uint)(1 << Count) - 1;
			return (ushort)(DestVal | (mask << BitIndex));
		}
		/// <summary>
		/// Sets the specified count of bits in specified uint value.
		/// </summary>
		/// <param name="DestVal">The destination uint value.</param>
		/// <param name="BitIndex">Index of the bit to set.</param>
		/// <param name="Count">Count of bits to set.</param>
		/// <returns>Returns the result uint value.</returns>
		public static uint		SetBits(uint DestVal,	int BitIndex, int Count)
		{
			uint mask = (uint)(1 << Count) - 1;
			return (uint)(DestVal | (uint)(mask << BitIndex));
		}
		#endregion

		//	ToString
		#region
		/// <summary>
		/// Gets or sets the current bit delimiter char for ToString() methods.
		/// </summary>
		public static char		BitDelimiterChar	= '\'';

		/// <summary>
		/// Returns the binary string representation of specified unsigned integer value.
		/// </summary>
		/// <param name="Value">Source uint value.</param>
		/// <returns>Returns the binary string representation of specified unsigned integer value.</returns>
		public static string	ToString(uint Value)
		{	return ToString(Value, 32, ZBitDelimiter.None);		}
		/// <summary>
		/// Returns the binary string representation of specified unsigned integer value.
		/// </summary>
		/// <param name="Value">Source uint value.</param>
		/// <param name="Length">Length of the bit array.</param>
		/// <returns>Returns the binary string representation of specified unsigned integer value with specified length.</returns>
		public static string	ToString(uint Value, int Length)
		{	return ToString(Value, 32, ZBitDelimiter.None);	}
		/// <summary>
		/// Returns the binary string representation of specified unsigned integer value.
		/// </summary>
		/// <param name="Value">Source uint value.</param>
		/// <param name="BitDelimiter">Sets whether some kind of bit delimiter should be used.</param>
		/// <returns>Returns the binary string representation of specified unsigned integer value.</returns>
		public static string	ToString(uint Value, ZBitDelimiter BitDelimiter)
		{	return ToString(Value, 32, BitDelimiter);	}
		/// <summary>
		/// Returns the binary string representation of specified unsigned integer value.
		/// </summary>
		/// <param name="Value">Source uint value.</param>
		/// <param name="Length">Length of the bit array.</param>
		/// <param name="BitDelimiter">Sets whether some kind of bit delimiter should be used.</param>
		/// <returns>Returns the binary string representation of specified unsigned integer value with specified length.</returns>
		public static string	ToString(uint Value, int Length, ZBitDelimiter BitDelimiter)
		{	
			string S = "";
			if (Length > 32)	Length = 32;
			for (int i = 0; i < Length; i++)
			{
				if (GetBit(Value, i))	S = "1" + S;	else S = "0" +S;
				if (BitDelimiter != ZBitDelimiter.None)
					if ((i+1) % (int)BitDelimiter == 0)	 S = BitDelimiterChar + S;
			}
			return S.Trim(BitDelimiterChar);
		}
		#endregion
	}



	/// <summary>
	/// This struct defines the set of methods for bit arrays operations.
	/// </summary>
	public struct		ZBitArray
	{
		//	Fields & Properties
		#region
		private BitArray		raw;
		/// <summary>
		/// Gets or sets the value of the bit with specified index.
		/// </summary>
		/// <param name="Index">Index of the bit to get or set.</param>
		/// <returns>Returns the value of the bit with specified index.</returns>
		public bool				this[int Index]
		{	
			get {	if (raw != null  &&  Index < raw.Length)  return raw[Index];	return false;	}
			set {	if (raw != null  &&  Index < raw.Length)  raw[Index] = value;	}
		}
		/// <summary>
		/// Gets the number of elements in this bit array.
		/// </summary>
		public int				Count		{	get {	return raw.Length;	}}
		/// <summary>
		/// Gets or sets the current bit delimiter char.
		/// </summary>
		public static char		BitDelimiterChar	= '\'';
		#endregion

		//	Get/Set range
		#region
		/// <summary>
		/// Gets the integer value for the specified range of bits.
		/// </summary>
		/// <param name="start">Index of the first bit to get.</param>
		/// <param name="count">Count of bits to get.</param>
		/// <returns>Returns the integer value for the specified range of bits.</returns>
		public int				GetRange(int start, int count)
		{
			if (raw == null  ||  start < 0  ||  start+count >= Count)	return 0;
			return ZBits.GetBits(this.ToUInt(), start, count);
		}
		/// <summary>
		/// Gets the short value for the specified range of bits.
		/// </summary>
		/// <param name="start">Index of the first bit to get.</param>
		/// <param name="count">Count of bits to get.</param>
		/// <returns>Returns the short value for the specified range of bits.</returns>
		public ushort			GetRangeS(int start, int count)
		{	return (ushort)GetRange(start, count);	}
		/// <summary>
		/// Gets the byte value for the specified range of bits.
		/// </summary>
		/// <param name="start">Index of the first bit to get.</param>
		/// <param name="count">Count of bits to get.</param>
		/// <returns>Returns the byte value for the specified range of bits.</returns>
		public byte				GetRangeB(int start, int count)
		{	return (byte)GetRange(start, count);	}
		/// <summary>
		/// Gets the integer value for the specified range of bits.
		/// </summary>
		/// <param name="start">Index of the first bit to set.</param>
		/// <param name="count">Count of bits to set.</param>
		/// <param name="value"></param>
		public void				SetRange(int start, int count, int value)
		{
			if (raw == null  ||  start < 0  ||  start+count >= Count)	return;
			for (int i = start; i < start+count; i++)
				raw[i] = ZBits.GetBit(value, i-start);
		}
		#endregion

		//	To uint, ushort, byte
		#region
		/// <summary>
		/// Gets the uint value of this bit array.
		/// </summary>
		/// <returns></returns>
		public uint				ToUInt()	
		{	
			if (raw == null)	return 0;
			var array = new int[1];
			raw.CopyTo(array, 0);
			return (uint)array[0];
		}
		/// <summary>
		/// Gets the ushort value of this bit array.
		/// </summary>
		/// <returns></returns>
		public ushort			ToUShort()	
		{	return (ushort)ToUInt();	}
		/// <summary>
		/// Gets the byte value of this bit array.
		/// </summary>
		/// <returns></returns>
		public byte				ToByte()	
		{	return (byte)ToUInt();	}
		#endregion

		//	ToString
		#region
		/// <summary>
		/// Returns the string representation of this ZBitArray instance.
		/// </summary>
		/// <returns>Returns the string representation of this ZBitArray instance.</returns>
		public new string		ToString()
		{	return ToString(32, ZBitDelimiter.None);		}
		/// <summary>
		/// Returns the string representation of this ZBitArray instance.
		/// </summary>
		/// <param name="Length">Length of the bit array.</param>
		/// <returns>Returns the string representation of this ZBitArray instance with specified length.</returns>
		public string			ToString(int Length)
		{	return ToString(32, ZBitDelimiter.None);	}
		/// <summary>
		/// Returns the string representation of this ZBitArray instance.
		/// </summary>
		/// <param name="BitDelimiter">Sets whether some kind of bit delimiter should be used.</param>
		/// <returns>Returns the string representation of this ZBitArray instance.</returns>
		public string			ToString(ZBitDelimiter BitDelimiter)
		{	return ToString(32, BitDelimiter);	}
		/// <summary>
		/// Returns the string representation of this ZBitArray instance.
		/// </summary>
		/// <param name="Length">Length of the bit array.</param>
		/// <param name="BitDelimiter">Sets whether some kind of bit delimiter should be used.</param>
		/// <returns>Returns the string representation of this ZBitArray instance.</returns>
		public string			ToString(int Length, ZBitDelimiter BitDelimiter)
		{	
			ZBits.BitDelimiterChar = ZBitArray.BitDelimiterChar;
			return ZBits.ToString(this.ToUInt(), Length, BitDelimiter);
		}
		#endregion

		//	Constructors
		#region
		/// <summary>
		/// Constructor of ZBitArray instance from unsigned integer.
		/// </summary>
		/// <param name="src">Source unsigned integer value.</param>
		public ZBitArray(uint src)
		{	raw = new BitArray(new int[] { (int)src });	}
		/// <summary>
		/// Constructor of ZBitArray instance from signed integer.
		/// </summary>
		/// <param name="src">Source signed integer value.</param>
		public ZBitArray(int src)
		{	raw = new BitArray(new int[] { src });		}
		/// <summary>
		/// Constructor of ZBitArray instance from unsigned long integer.
		/// </summary>
		/// <param name="src">Source unsigned long integer value.</param>
		public ZBitArray(ulong src)
		{	raw = new BitArray(ZConvert.LongToDoubleInt(src));	}
		/// <summary>
		/// Constructor of ZBitArray instance from signed long integer.
		/// </summary>
		/// <param name="src">Source signed long value.</param>
		public ZBitArray(long src)
		{	raw = new BitArray(ZConvert.LongToDoubleInt(src));		}
		#endregion
	}



	/// <summary>
	/// This enum defines the types of bit delimiters.
	/// </summary>
	public enum			ZBitDelimiter
	{
		/// <summary>
		/// No delimiters.
		/// </summary>
		None		= 0,
		/// <summary>
		/// Every 4 bits.
		/// </summary>
		Every_4		= 4,
		/// <summary>
		/// Every 8 bits.
		/// </summary>
		Every_8		= 8,
		/// <summary>
		/// Every 16 bits.
		/// </summary>
		Every_16	= 16
	}
}

//	4 kb
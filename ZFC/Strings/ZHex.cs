namespace ZFC.Strings
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;


	/// <summary>
	/// This class defines the set of methods for operations on hexadecimal numbers.
	/// </summary>
	public class ZHex
	{
		#region Gets whether some string is Hex

		private readonly static List<char>	_hexDigits	= new List<char>()
		{ '1','2','3','4','5','6','7','8','9','0','A','B','C','D','E','F','a','b','c','d','e','f' };
		/// <summary>
		/// Gets the list of hexadecimal symbols (digits and letters).
		/// </summary>
		public static List<char>			HexDigits	{	get	{	return _hexDigits;	}}

		/// <summary>
		/// Gets whether the specified string represents a valid hexadecimal number.
		/// </summary>
		/// <param name="textValue">The string value to check.</param>
		/// <returns>Returns TRUE if specified string is a valid hexadecimal number, otherwise returns FALSE.</returns>
		public static bool			IsHexNumber(string textValue)
		{
			for (var i = 0; i < textValue.Length; i++)
			{
				if (!HexDigits.Contains(textValue[i]))
					return false;
			}
			return true;
		}

		#endregion


		#region Get hex string from byte array, like "1A3B5C7D" or "0x1A3B, 0x5C7D"

		/// <summary>
		/// Gets the exact hex string from array of bytes, e.g. "E01F15CD"
		/// </summary>
		/// <param name="sourceArray">Source byte array.</param>
		/// <returns>Returns the string with hexadecimal representation.</returns>
		public static string		Get_ExactHexStringFromBytes(byte[] sourceArray)
		{
			return 	Get_ExactHexStringFromBytes(sourceArray, false);
		}
		/// <summary>
		/// Gets the exact hex string from array of bytes, e.g. "E01F15CD"
		/// </summary>
		/// <param name="sourceArray">Source byte array.</param>
		/// <param name="useSpaceDelimiter">Specified whether bytes must be separated by spaces, like in "1D 3C 5E".</param>
		/// <returns>Returns the string with hexadecimal representation.</returns>
		public static string		Get_ExactHexStringFromBytes(byte[] sourceArray, bool useSpaceDelimiter)
		{
			if (sourceArray == null  ||  sourceArray.Length == 0)
				return null;
			string resultString = BitConverter.ToString(sourceArray).ToLower();
			return resultString.Replace("-", useSpaceDelimiter ? " " : "");
		}
	
		/// <summary>
		/// Gets the list of hexadecimal values from specified byte array.
		/// </summary>
		/// <param name="sourceArray">Source byte array.</param>
		/// <param name="wordSize">Size of a word. 1 is "0x1C, 0x4D", 2 is "0x1C4D", etc.</param>
		/// <returns>Returns the list of hexadecimal values from specified byte array if successful, otherwise returns NULL.</returns>
		public static string		Get_HexList(byte[] sourceArray, int wordSize)
		{
			if (sourceArray == null  ||  sourceArray.Length == 0)
				return null;
			string resultString = string.Empty;

			for (int i = 0; i < sourceArray.Length / wordSize; i++)
			{
				string tempString = Convert.ToString(sourceArray[i], 16);
				if (wordSize == 2)	tempString = Convert.ToString(BitConverter.ToUInt16(sourceArray, i*2), 16);
				if (wordSize == 4)	tempString = Convert.ToString(BitConverter.ToUInt32(sourceArray, i*4), 16);
				tempString = tempString.PadLeft(wordSize*2, '0');
				resultString += ", 0x" + tempString.ToUpper();
			}
			return resultString.Substring(2, resultString.Length-2);
		}

		#endregion


		#region Get values from ints, shorts, bytes

		private static string		Get_HexStringVal(uint value, int length)
		{
			string result = Convert.ToString(value, 16);
			return result.ToUpper().PadLeft(length, '0');
		}
		/// <summary>
		/// Gets string representation of signed short value.
		/// </summary>
		/// <param name="value">Signed short value.</param>
		/// <returns>Result string with hex representation of value.</returns>
		public static string		Get_HexShort(short value)
		{
			return Get_HexStringVal((uint)value, 4);
		}
		/// <summary>
		/// Gets string representation of unsigned short value.
		/// </summary>
		/// <param name="value">Unsigned short value.</param>
		/// <returns>Result string with hex representation of value.</returns>
		public static string		Get_HexShort(ushort value)
		{
			return Get_HexStringVal((uint)value, 4);
		}
		/// <summary>
		/// Gets string representation of signed integer value.
		/// </summary>
		/// <param name="value">Signed integer value.</param>
		/// <returns>Result string with hex representation of value.</returns>
		public static string		Get_HexInt(int value)
		{
			return Get_HexStringVal((uint)value, 8);
		}
		/// <summary>
		/// Gets string representation of unsigned integer value.
		/// </summary>
		/// <param name="value">Unsigned integer value.</param>
		/// <returns>Result string with hex representation of value.</returns>
		public static string		Get_HexInt(uint value)
		{
			return Get_HexStringVal((uint)value, 8);
		}
		/// <summary>
		/// Get string representation of signed byte value.
		/// </summary>
		/// <param name="value">Signed byte value.</param>
		/// <returns>Result string with hex representation of value.</returns>
		public static string		Get_HexByte(sbyte value)
		{
			return Get_HexStringVal((uint)value, 2);
		}
		/// <summary>
		/// Get string representation of unsigned byte value.
		/// </summary>
		/// <param name="value">Unsigned byte value.</param>
		/// <returns>Result string with hex representation of value.</returns>
		public static string		Get_HexByte(byte value)
		{
			return Get_HexStringVal((uint)value, 2);
		}

		#endregion


		#region Colors to hex strings and vica versa

		/// <summary>
		/// Converts the specified color to its hexadecimal representation, with "#" sign afront.
		/// </summary>
		/// <param name="color">The color to convert.</param>
		/// <returns>Returns the result string with hexadecimal representation of specified color.</returns>
		public static string		Get_ColorHexString(Color color)
		{
			return Get_ColorHexString(color.ToArgb() & 0x00FFFFFF, true, false);
		}
		/// <summary>
		/// Converts the integer value of color to hex-string.
		/// </summary>
		/// <param name="color">The integer value of color to convert.</param>
		/// <returns>Returns the result string with hexadecimal representation of specified color.</returns>
		public static string		Get_ColorHexString(int color)
		{
			return Get_ColorHexString(color, true, false);
		}
		/// <summary>
		/// Converts the integer value of color to its hexadecimal representation.
		/// </summary>
		/// <param name="color">The integer value of color to convert.</param>
		/// <param name="includePoundSign">Sets whether to include # or not.</param>
		/// <returns>Returns the result string with hexadecimal representation of specified color.</returns>
		public static string		Get_ColorHexString(int color, bool includePoundSign)
		{
			return Get_ColorHexString(color, includePoundSign, false);
		}
		/// <summary>
		/// Converts the integer value of color to hex-string.
		/// </summary>
		/// <param name="color">The integer value of color to convert.</param>
		/// <param name="includePoundSign">Sets whether to include # or not.</param>
		/// <param name="includeAlpha">Sets whether alpha-value should be included.</param>
		/// <returns>Returns the result string with hexadecimal representation of specified color.</returns>
		public static string		Get_ColorHexString(int color, bool includePoundSign, bool includeAlpha)
		{
			if (!includeAlpha)
				color = color & 0x00FFFFFF;
			string resultText = color.ToString("X").PadLeft(6, '0');
			return includePoundSign ? "#" + resultText : resultText;
		}

		/// <summary>
		/// Gets the integer value of color from hex string, with or without pound sign.
		/// </summary>
		/// <param name="sourceText">The source input string.</param>
		/// <returns>Returns the integer value for specified color if source string is valid, otherwise returns -1.</returns>
		public static int			Get_ColorValFromHexString(string sourceText)
		{
			if (sourceText == "auto")
				return 0;
			if (sourceText.StartsWith("#"))	 
				sourceText = sourceText.Substring(1, sourceText.Length-1);
			try		{	return Convert.ToInt32(sourceText, 16);	}
			catch	{	return -1;	}
		}
		/// <summary>
		/// Gets the color from hex string, with or without pound sign.
		/// </summary>
		/// <param name="sourceText">The source input string.</param>
		/// <returns>Returns the color if source string is valid.</returns>
		public static Color			Get_ColorFromHexString(string sourceText)
		{
			return Color.FromArgb(Get_ColorValFromHexString(sourceText));
		}

		#endregion
	}
}
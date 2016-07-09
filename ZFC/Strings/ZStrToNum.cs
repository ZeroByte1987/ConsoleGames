namespace ZFC.Strings
{
	using System.Text.RegularExpressions;
	using ZLinq;


	/// <summary>
	/// This class defines the set of methods for string-to-value operations.
	/// </summary>
	public static class ZStrToNum
	{
		#region Checks whether specified string is a number

		/// <summary>
		/// Gets or sets the decimal separator.
		/// </summary>
		public static char			DecimalSeparator	{	get	{	return _separator;	}	set	{	_separator = value;	}}
		private static char			_separator = '.';

		/// <summary>
		/// Gets whether specified source string represents an integer number.
		/// </summary>
		/// <param name="sourceString">The input string to analyze.</param>
		/// <returns>Returns TRUE if specified string represents a decimal number, otherwise returns FALSE.</returns>
		public static bool			IsIntNumber(string sourceString)
		{
			return sourceString.All(char.IsDigit);
		}

		/// <summary>
		/// Gets whether specified source string represents a float number.
		/// </summary>
		/// <param name="sourceString">The input string to analyze.</param>
		/// <returns>Returns TRUE if specified string represents a decimal number, otherwise returns FALSE.</returns>
		public static bool			IsFloatNumber(string sourceString)
		{
			return sourceString.All(t => char.IsDigit(t)  ||  t == DecimalSeparator);
		}

		#endregion


		#region String to integer values

		/// <summary>
		/// Gets the signed byte value from string.
		/// </summary>
		/// <param name="sourceString">Source string.</param>
		/// <returns>Returns the obtained signed byte value if successful, otherwise returns 255.</returns>
		public static sbyte			ToSByte(string sourceString)
		{
			return (sbyte)ToInt32(sourceString);
		}
		/// <summary>
		/// Gets the byte value from string.
		/// </summary>
		/// <param name="sourceString">Source string.</param>
		/// <returns>Returns the obtained byte value if successful, otherwise returns 255.</returns>
		public static byte			ToByte(string sourceString)
		{
			return (byte)ToUInt32(sourceString);
		}
		/// <summary>
		/// Gets the short value from string.
		/// </summary>
		/// <param name="sourceString">Source string.</param>
		/// <returns>Returns the obtained short value if successful, otherwise returns -1.</returns>
		public static short			ToInt16(string sourceString)
		{
			return (short)ToInt32(sourceString);
		}
		/// <summary>
		/// Gets the ushort value from string.
		/// </summary>
		/// <param name="sourceString">Source string.</param>
		/// <returns>Returns the obtained ushort value if successful, otherwise returns -1.</returns>
		public static ushort		ToUInt16(string sourceString)
		{
			return (ushort)ToUInt32(sourceString);
		}
		/// <summary>
		/// Gets the integer value from string.
		/// </summary>
		/// <param name="sourceString">Source string.</param>
		/// <returns>Returns the obtained integer value if successful, otherwise returns -1.</returns>
		public static int			ToInt32(string sourceString)
		{
			return (int)ToUInt32(sourceString);
		}
		/// <summary>
		/// Gets the uint value from string.
		/// </summary>
		/// <param name="sourceString">Source string.</param>
		/// <returns>Returns the obtained uint value if successful, otherwise returns -1.</returns>
		public static uint			ToUInt32(string sourceString)
		{
			string resultString = Regex.Match(sourceString, @"[-+]?[0-9]+").Value;
			return resultString != "" ? uint.Parse(resultString) : uint.MaxValue;
		}

		#endregion


		#region String to float values

		/// <summary>
		/// Gets the float value from string.
		/// </summary>
		/// <param name="stringSource">Source string.</param>
		/// <returns>Returns the obtained float value if successful, otherwise returns -1.</returns>
		public static float			ToFloat(string stringSource)
		{
			return ToFloat(stringSource, false);
		}
		/// <summary>
		/// Gets the double value from string.
		/// </summary>
		/// <param name="stringSource">Source string.</param>
		/// <returns>Returns the obtained double value if successful, otherwise returns -1.</returns>
		public static double		ToDouble(string stringSource)
		{
			return ToDouble(stringSource, false);
		}
		/// <summary>
		/// Gets the float value from string.
		/// </summary>
		/// <param name="stringSource">Source string.</param>
		/// <param name="useInvariantCulture">Use invarian culture (i.e. always use dot as the decimal separator).</param>
		/// <returns>Returns the obtained float value if successful, otherwise returns -1.</returns>
		public static float			ToFloat(string stringSource, bool useInvariantCulture)
		{
			return (float)ToDouble(stringSource, useInvariantCulture);
		}
		/// <summary>
		/// Gets the double value from string.
		/// </summary>
		/// <param name="stringSource">Source string.</param>
		/// <param name="useInvariantCulture">Use invarian culture (i.e. always use dot as the decimal separator).</param>
		/// <returns>Returns the obtained double value if successful, otherwise returns -1.</returns>
		public static double		ToDouble(string stringSource, bool useInvariantCulture)
		{
			var oldCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
			if (useInvariantCulture)
				System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
			string resultString = Regex.Match(stringSource, @"[-+]?[0-9]*\" + _separator + "?[0-9]+").Value;
			var result = (resultString != "") ? double.Parse(resultString) : -1f;
			System.Threading.Thread.CurrentThread.CurrentCulture = oldCulture;
			return result;
		}

		/// <summary>
		/// Gets the boolean value from string
		/// </summary>
		/// <param name="stringSource">Source string.</param>
		/// <returns>Returns TRUE if string is one of the set ("true", "on", "1"), otherwise return FALSE.</returns>
		public static bool			ToBool(string stringSource)
		{
			string S = stringSource.ToLower();
			return S == "true"  ||  S == "1"  ||  S == "on"  ||  S == "yes";
		}

		#endregion
	}
}

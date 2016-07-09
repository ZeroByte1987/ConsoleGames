namespace ZFC.Strings
{
	using System;
	using System.Collections.Generic;
	using ZLinq;


	/// <summary>
	/// This class defines the set of methods for string operations.
	/// </summary>
	public static class ZString
	{
		#region Lists of Digits and HexDigits
		/// <summary>
		/// Defines the set of decimal digits.
		/// </summary>
		public readonly static List<char>	Digits		= new List<char>()	{ '1','2','3','4','5','6','7','8','9','0', };
		/// <summary>
		/// Defines the set of non-Unicode symbols.
		/// </summary>
		private readonly static List<int>	NonUnicodeChars	= new List<int>()
		{	0x2013, 0x2014, 0x2018, 0x2019, 0x201A, 0x201C, 0x201D, 0x201E, 0x2020, 0x2021, 0x2022, 0x2026, 0x2030, 0x2039,
			0x203A, 0x20AC, 0x2122, 0x0152, 0x0153, 0x0160, 0x0161, 0x0178, 0x017D, 0x017E, 0x0192, 0x02C6, 0x02DC	};

		#endregion


		#region Miscellaneous

		/// <summary>
		/// Gets whether the specified string contains ASCII characters only.
		/// </summary>
		/// <param name="inputString">Input string to check.</param>
		/// <returns>True if given string contains ASCII characters only, otherwise false.</returns>
		public static bool			IsASCII(string inputString)
		{
			return inputString.All(t => (int)t <= 127);
		}

		/// <summary>
		/// Gets whether the specified string contains ANSI characters only.
		/// </summary>
		/// <param name="inputString">Input string to check.</param>
		/// <returns>True if given string contains ANSI characters only, otherwise false.</returns>
		public static bool			IsANSI(string inputString)
		{
			return inputString.All(t => t <= 255 || NonUnicodeChars.Contains(t));
		}

		/// <summary>
		/// Gets whether the specified string contains Unicode characters
		/// and returns the index of the first occurence of Unicode character.
		/// </summary>
		/// <param name="inputString">Input string</param>
		/// <returns>Returns the index of the first occurence.</returns>
		public static int			IsUnicode(string inputString)
		{
			for (int i = 0; i < inputString.Length; i++)
				if (inputString[i] > 255  &&  NonUnicodeChars.Contains(inputString[i]))
					return i;
			return -1;
		}

		/// <summary>
		/// Trims the string to specified length.
		/// </summary>
		/// <param name="inputString">Text string to trim.</param>
		/// <param name="length">Maximal length.</param>
		/// <returns>Returns the trimmed string.</returns>
		public static string		TrimText(string inputString, int length)
		{
			return inputString.Length > length ? inputString.Substring(0, length - 3) + "..." : inputString;
		}

		/// <summary>
		/// Recreate string to empty string if it is null.
		/// </summary>
		/// <param name="sourceString">String to recreate.</param>
		public static void			Restring(ref string sourceString)
		{
			if (sourceString == null)	
				sourceString = string.Empty;
		}

		/// <summary>
		/// Gets the null-terminated char-array from string.
		/// </summary>
		/// <param name="sourceString">Source string.</param>
		/// <returns>Return result char-array.</returns>
		public static char[]		Get_NullString(string sourceString)
		{
			var C1 = new List<char>(sourceString);
			C1.Add((char)0);
			return C1.ToArray();
		}

		#endregion



		#region Numbers-to-String

		/// <summary>
		/// Gets the string from array of bytes.
		/// </summary>
		/// <param name="sourceArray">Source array of bytes.</param>
		/// <returns>Returns the resulting string.</returns>
		public static string		Get_StringFromBytes(byte[] sourceArray)
		{
			var chars = new char[sourceArray.Length / sizeof(char)];
			Buffer.BlockCopy(sourceArray, 0, chars, 0, sourceArray.Length);
			return new string(chars);
		}

		/// <summary>
		/// Converts the integer value to its literal form.
		/// </summary>
		/// <param name="value">The value to convert.</param>
		/// <returns>Returns the resulting string with converter integer value.</returns>
		public static string		Convert_NumberToWords(int value)
		{
			int V = value;
			if (V == 0)		return "zero";
			if (V < 0)		return "minus " + Convert_NumberToWords(System.Math.Abs(V));
			string words = string.Empty;
			if ((V/1000000000) > 0)	{	words += Convert_NumberToWords(V / 1000000000) + " billion ";	V %= 1000000;	}
			if ((V / 1000000) > 0)	{	words += Convert_NumberToWords(V / 1000000) + " million ";	V %= 1000000;	}
			if ((V / 1000) > 0)		{	words += Convert_NumberToWords(V / 1000) + " thousand ";	V %= 1000;		}
			if ((V / 100) > 0)		{	words += Convert_NumberToWords(V / 100) + " hundred ";		V %= 100;		}
			if (V > 0)
			{
				if (words != string.Empty)	
					words += "and ";
				var unitsMap = new[] { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
				var tensMap  = new[] { "zero", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };
				if (V < 20)	words += unitsMap[V];
				else
				{
					words += tensMap[V / 10];
					if ((V % 10) > 0)	words += "-" + unitsMap[V % 10];
				}
			}
			return words.Trim();;
		}

		/// <summary>
		/// Gets the two-digit representation of given integer.
		/// </summary>
		/// <param name="value">Integer value.</param>
		/// <returns>Returns the two-digit representation of given integer.</returns>
		public static string		Get_2Digit(int value)
		{
			return Get_NDigit(value, 2);
		}
		/// <summary>
		/// Gets the N-digit representation of given integer.
		/// </summary>
		/// <param name="value">Integer value.</param>
		/// <param name="digitCount">Count of digits.</param>
		/// <returns>Returns the N-digit representation of given integer.</returns>
		public static string		Get_NDigit(int value, int digitCount)
		{
			return Convert.ToString(value).PadLeft(digitCount, '0');
		}

		#endregion


		#region Get string with size in KB, MB or GB

		/// <summary>
		/// Gets the string with kilobytes.
		/// </summary>
		/// <param name="value">Number of bytes.</param>
		/// <returns>Returns string with size in kilobytes.</returns>
		public static string		GetKB(long value)
		{
			return GetKB(value, true);
		}
		/// <summary>
		/// Gets the string with kilobytes.
		/// </summary>
		/// <param name="value">Number of bytes.</param>
		/// <param name="addDescription">Add a "kb" at the end of result string.</param>
		/// <returns>Returns string with size in kilobytes.</returns>
		public static string		GetKB(long value, bool addDescription)
		{
			int Kb		= (int)(value / 1024);
			int Mb		= (int)(Kb / 1000);
			int Gb		= 0;
			if (Mb > 999)	{	Gb = Mb / 1000;	  Mb = Mb % 1000;	}	

			string stringValue = string.Empty;
			if (Gb > 0)		stringValue += Gb + "'";
			if (Mb > 0)		stringValue += Convert.ToString(Mb).PadLeft(3, '0') + "'";
			stringValue += Convert.ToString(Kb % 1000).PadLeft(3, '0');
			stringValue = stringValue.TrimStart('0');
			return addDescription ? stringValue + " kb" : stringValue;
		}

		/// <summary>
		/// Gets the string with megabytes.
		/// </summary>
		/// <param name="value">Number of bytes.</param>
		/// <returns>Returns string with size in megabytes.</returns>
		public static string		GetMB(long value)
		{
			return GetMB(value, true);
		}
		/// <summary>
		/// Gets the string with megabytes.
		/// </summary>
		/// <param name="value">Number of bytes.</param>
		/// <param name="addDescription">Add a "mb" at the end of result string.</param>
		/// <returns>Returns string with size in megabytes.</returns>
		public static string		GetMB(long value, bool addDescription)
		{
			int Mb		= (int)(value / 1048576);
			int Gb		= 0;
			if (Mb > 999)	{	Gb = Mb / 1000;	  Mb = Mb % 1000;	}

			string stringValue = string.Empty;
			if (Gb > 0)		stringValue += Gb + "'";
			if (Mb > 0)		stringValue += Convert.ToString(Mb).PadLeft(3, '0');
			stringValue = stringValue.TrimStart('0');
			return addDescription ? stringValue + " mb" : stringValue;
		}

		/// <summary>
		/// Gets the string with gigabytes.
		/// </summary>
		/// <param name="value">Number of bytes.</param>
		/// <returns>Returns string with size in gigabytes.</returns>
		public static string		GetGB(long value)
		{
			return GetGB(value, true);
		}
		/// <summary>
		/// Gets the string with gigabytes.
		/// </summary>
		/// <param name="value">Number of bytes.</param>
		/// <param name="addDescription">Add a "gb" at the end of result string.</param>
		/// <returns>Returns string with size in gigabytes.</returns>
		public static string		GetGB(long value, bool addDescription)
		{
			float gbValue = value / 1073741824f;
			var stringValue = Convert.ToString(gbValue);
			return addDescription ? stringValue + " gb" : stringValue;
		}

		#endregion
	}
}

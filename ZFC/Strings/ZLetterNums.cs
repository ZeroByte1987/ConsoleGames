namespace ZFC.Strings
{
	/// <summary>
	/// This class defines the set of methods for numbers in literal forms, like Roman or Letter numbers.
	/// </summary>
	public static class ZLetterNums
	{
		#region Get a string with roman number

		/// <summary>
		/// Gets the string with Roman number.
		/// </summary>
		/// <param name="value">Integer value.</param>
		/// <returns>Returns the  string with Roman number that represents specified value.</returns>
		public static string		Get_RomanNumber(int value)
		{
			return Get_RomanNumber((uint)value, false);
		}
		/// <summary>
		/// Gets the string with Roman number.
		/// </summary>
		/// <param name="value">Integer value.</param>
		/// <returns>Returns the string with Roman number that represents specified value.</returns>
		public static string		Get_RomanNumber(uint value)
		{
			return Get_RomanNumber(value, false);
		}
		/// <summary>
		/// Gets the string with Roman number.
		/// </summary>
		/// <param name="value">Integer value.</param>
		/// <param name="useLowerCase">Sets whether the result string will be transformed to lower-case.</param>
		/// <returns>Returns the string with Roman number that represents specified value.</returns>
		public static string		Get_RomanNumber(int value, bool useLowerCase)
		{
			return Get_RomanNumber((uint)value, useLowerCase);
		}
		/// <summary>
		/// Gets the string with Roman number.
		/// </summary>
		/// <param name="value">Integer value.</param>
		/// <param name="useLowerCase">Sets whether the result string will be transformed to lower-case.</param>
		/// <returns>Returns the string with Roman number that represents specified value.</returns>
		public static string		Get_RomanNumber(uint value, bool useLowerCase)
		{
			string		resultString = string.Empty;
			if (value <= 0)	
				return resultString;
			string[]	romanDigits =      { "I","IV","V","IX","X","XL","L","XC","C", "CD", "D", "CM", "M"};
			int[]		romanDigitValues = {  1,  4,   5,  9,  10,  40, 50,  90, 100, 400,  500, 900,  1000};
			int			i = 12;
			while (value > 0)
			{
				while (romanDigitValues[i] > value)
					i--;
				resultString += romanDigits[i];
				value -= (uint)romanDigitValues[i];
			}
			return useLowerCase ? resultString.ToLower() : resultString;
		}

		/// <summary>
		/// Gets the integer value from string with roman number, like XXIII = 23.
		/// </summary>
		/// <param name="sourceText">Source string with roman number.</param>
		/// <returns>Returns the result integer value if string is valid, otherwise returns -1.</returns>
		public static int			Get_NumberFromRoman(string sourceText)
		{
			sourceText = sourceText.ToUpper().Trim();
			int N = 0;
			char C = ' ';
			for (int i = 0; i < sourceText.Length; i++)
			{
				C = i < sourceText.Length-1 ? sourceText[i+1] : ' ';
				switch (sourceText[i])
				{
					case 'I'	:	if (C == 'V')	 {	i++;  N += 4;	}	else if (C == 'X')	 {	i++;  N += 9;	}	else N++;		break;
					case 'V'	:	N += 5;		break;
					case 'X'	:	if (C == 'L')	 {	i++;  N += 40;	}	else if (C == 'C')	 {	i++;  N += 90;	}	else N += 10;	break;
					case 'L'	:	N += 50;	break;
					case 'C'	:	if (C == 'D')	 {	i++;  N += 400;	}	else if (C == 'M')	 {	i++;  N += 900;	}	else N += 100;	break;
					case 'D'	:	N += 500;	break;
					case 'M'	:	N += 1000;	break;
				}
			}
			return N;
		}

		#endregion
		
		#region Get a string with latin-letter number

		/// <summary>
		/// Gets the string with letter number.
		/// </summary>
		/// <param name="value">Integer value.</param>
		/// <returns>Returns the string with letter number that represents specified value.</returns>
		public static string		Get_LetterNumber(int value)
		{
			return Get_LetterNumber((uint)value, false);
		}
		/// <summary>
		/// Gets the string with letter number.
		/// </summary>
		/// <param name="value">Integer value.</param>
		/// <returns>Returns the string with letter number that represents specified value.</returns>
		public static string		Get_LetterNumber(uint value)
		{
			return Get_LetterNumber(value, false);
		}
		/// <summary>
		/// Gets the string with letter number.
		/// </summary>
		/// <param name="value">Integer value.</param>
		/// <param name="useLowerCase">Set whether the result string will be transformed to lower-case.</param>
		/// <returns>Returns the string with letter number that represents specified value.</returns>
		public static string		Get_LetterNumber(int value, bool useLowerCase)
		{
			return Get_LetterNumber((uint)value, useLowerCase);
		}
		/// <summary>
		/// Gets the string with letter number.
		/// </summary>
		/// <param name="value">Integer value.</param>
		/// <param name="useLowerCase">Set whether the result string will be transformed to lower-case.</param>
		/// <returns>Returns the string with letter number that represents specified value.</returns>
		public static string		Get_LetterNumber(uint value, bool useLowerCase)
		{
			const string digits = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
			string resultString = string.Empty;
			if (value <= 0)	
				return resultString;
			int currentValue = (int)value;
			while (currentValue != 0)
			{
				resultString = digits[--currentValue % 26] + resultString;
				currentValue /= 26;
			}
			return useLowerCase ? resultString.ToLower() : resultString;
		}

		/// <summary>
		/// Gets the integer value from letter number, like AC = 29
		/// </summary>
		/// <param name="value">Source string with letter number.</param>
		/// <returns>Returns the result integer value if string is valid, otherwise returns -1.</returns>
		public static int			Get_NumberFromLetters(string value)
		{
			if (string.IsNullOrEmpty(value))
				return -1;
			string preparedValue = value.ToUpper().Trim();
			int resultValue = 0,  currentDegree = 1;
			for (int i = preparedValue.Length-1; i >= 0; i--)
			{
				int letterValue = preparedValue[i] - 64;
				if (letterValue < 1  ||  letterValue > 26)
					return -1;
				resultValue += letterValue * currentDegree;
				currentDegree *= 26;
			}
			return resultValue;
		}

		#endregion
	}
}

namespace ZFC
{
	using System.Drawing;


	/// <summary>
	/// This class defines static methods for drawing purposes.
	/// </summary>
	public class ZDrawing
	{
		/// <summary>
		/// Checks whether specified string represents a color.
		/// </summary>
		/// <param name="text">Input text string.</param>
		/// <returns>Returns true if specified string represents a color, otherwise returns false.</returns>
		public static bool		IsHexColor(string text)
		{
			if ((text.Length == 6  ||  text.Length == 3  ||  ((text.Length == 7  ||  text.Length == 4)  &&  text.StartsWith("#"))))
			{
				for (int i = 0; i < text.Length; i++)
					if (!char.IsDigit(text[i])  &&  !Strings.ZHex.HexDigits.Contains(text[i])  &&  text[i] != '#')	
						return false;
				return true;
			}
			return false;
		}

		/// <summary>
		/// Gets a width of the string with specified font and font-size.
		/// </summary>
		/// <param name="text">Input string.</param>
		/// <param name="fontName">Font name.</param>
		/// <param name="fontSize">Font size.</param>
		/// <returns>Returns a width of specified string in pixels.</returns>
		public static float		Get_StringWidth(string text, string fontName, float fontSize)
		{
			return Get_StringWidth(text, fontName, fontSize, 0);
		}
		/// <summary>
		/// Gets a width of the string with specified font and font-size.
		/// </summary>
		/// <param name="text">Input string.</param>
		/// <param name="fontName">Font name.</param>
		/// <param name="fontSize">Font size.</param>
		/// <param name="styleFlags">Integer value representing System.Drawing.FontStyle instance.</param>
		/// <returns>Returns a width of specified string in pixels.</returns>
		public static float		Get_StringWidth(string text, string fontName, float fontSize, int styleFlags)
		{
			var bitmap	= Graphics.FromImage(new Bitmap(1, 1));
			var font	= new Font(fontName, fontSize / 2, (FontStyle)styleFlags);
			var stringSize	= bitmap.MeasureString(text, font);
			return stringSize.Width;
		}
	}
}

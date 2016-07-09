namespace ZConsole
{
	using System;
	using System.Collections.Generic;
	using System.Runtime.InteropServices;
	using System.Text;


	#region Public Enums

	public enum			Color
	{
		Black,
		DarkBlue,
		DarkGreen,
		DarkCyan,
		DarkRed,
		DarkMagenta,
		DarkYellow,
		Gray,
		DarkGray,
		Blue,
		Green,
		Cyan,
		Red,
		Magenta,
		Yellow,
		White
	}

	public enum			FrameType
	{
		None	= 0,
		Single	= 1,
		Double	= 2,
		Custom  = 3
	}

	#endregion

	#region Structures

	public class		Range
	{
		public int	Min;
		public int	Max;

		public int	RangeValue { get { return Max - Min + 1; }}

		public Range(int min, int max)
		{
			Min = min;
			Max = max;
		}

		public static Range GetFromString(string text)
		{
			try
			{
				var tokens = text.Split(new [] { '-', ',', ';' });
				return new Range(int.Parse(tokens[0].Trim()), int.Parse(tokens[1].Trim()));
			}
			catch
			{
				return new Range(0, 0);
			}			
		}
	}

	public class		Size
	{
		public int	Width;
		public int	Height;
		public bool IsEmpty		{ get { return Width == 0  ||  Height == 0; }}

		public Size(int width, int height)
		{
			Width  = width;
			Height = height;
		}

		public static Size GetFromString(string text)
		{
			try
			{
				var tokens = text.Split(new [] { '-', ',', ';' });
				return new Size(int.Parse(tokens[0].Trim()), int.Parse(tokens[1].Trim()));
			}
			catch
			{
				return new Size(0, 0);
			}			
		}
	}

	public class		Coord
	{
		public int	X	{ get; set; }
		public int	Y	{ get; set; }

		public Coord(int xPosition, int yPosition)
		{
			X = xPosition;
			Y = yPosition;
		}

		public void		Move(int dx, int dy)
		{
			X += dx;
			Y += dy;
		}
		public bool		Equals(Coord target)
		{
			return target.X == X  &&  target.Y == Y;
		}
	}

	public class		Rect
	{
		public int	Left	{ get; set; }
		public int	Top		{ get; set; }
		public int	Right	{ get; set; }
		public int	Bottom	{ get; set; }

		public Rect(int left, int top, int right, int bottom)
		{
			Left	= left;
			Top		= top;
			Right	= right;
			Bottom	= bottom;
		}

		/// <summary>
        /// Gets or sets the width of a rectangle.  When setting the width, the column position of the bottom right corner is adjusted.
        /// </summary>
		public int		Width
		{
			get { return Right-Left+1; }
			set { Right = Left + value - 1; }
		}

        /// <summary>
        /// Gets or sets the height of a rectangle.  When setting the height, the row position of the bottom right corner is adjusted.
        /// </summary>
		public int		Height
		{
			get { return Bottom-Top+1; }
			set { Bottom = Top + value - 1; }
		}
    }

	

	/// <summary>
	/// This class defines the console char attribute properties.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct		ZCharAttribute
	{
		#region Private Fields and Constructors

		private ushort raw;


		/// <summary>
        /// Creates a new instance of the ZCharAttribute structure.
        /// </summary>
		/// <param name="foreColor">The foreground color.</param>
		/// <param name="backColor">The background color.</param>
		public ZCharAttribute(Color foreColor, Color backColor)
		{
			raw = (ushort)(((ushort)backColor << 4) | (ushort)foreColor);
		}

		/// <summary>
        /// Creates a new instance of the ZCharAttribute structure.
        /// </summary>
        /// <param name="attributeValue">The combined foreground/background attribute.</param>
		public ZCharAttribute(int attributeValue)
		{
			raw = (ushort)attributeValue;
		}

		#endregion

		#region Public Properties

		/// <summary>
        /// Gets or sets the foreground color attribute.
        /// </summary>
        public Color	ForeColor
        {
            get { return (Color)(raw & 0x0F); }
            set { raw = (ushort)((raw & 0xFFF0) | (ushort)value); }
        }

        /// <summary>
        /// Gets or sets the background color attribute.
        /// </summary>
        public Color	BackColor
        {
            get { return (Color)((raw >> 4) & 0x0F); }
            set { raw = (ushort)((raw & 0xff0f) | ((ushort)value << 4)); }
        }

		/// <summary>
        /// Gets or sets the attribute (combined foreground/background color).
        /// </summary>
        public int		Attribute
        {
            get { return raw; }
            set { raw = (ushort)value; }
        }

		#endregion
	}

	/// <summary>
    /// Specifies a character and its attributes.
    /// </summary>
	[StructLayout(LayoutKind.Explicit)]
	public struct		ZCharInfo
	{
		#region Private Fields and Constructors

		[FieldOffset(0)]	private char			unicodeCharacter;
		[FieldOffset(0)]	private byte			asciiCharacter;
		[FieldOffset(2)]	private ZCharAttribute	attr;


		/// <summary>
        /// Creates a new instance of the ConsoleCharInfo structure.
        /// </summary>
        /// <param name="unicodeChar">The Unicode character.</param>
        /// <param name="charAttribute">Character attributes.</param>
		public ZCharInfo(char unicodeChar, ZCharAttribute charAttribute)
		{
			asciiCharacter = 0;
			unicodeCharacter = unicodeChar;
			attr = charAttribute;
		}

        /// <summary>
        /// Creates a new instance of the ConsoleCharInfo structure.
        /// </summary>
        /// <param name="asciiChar">The ASCII character.</param>
        /// <param name="charAttribute">Character attributes.</param>
		public ZCharInfo(byte asciiChar, ZCharAttribute charAttribute)
		{
			unicodeCharacter = '\x0';
			asciiCharacter = asciiChar;
			attr = charAttribute;
		}

		#endregion

		#region Public Properties

		/// <summary>
        /// Gets or sets the Unicode character represented by this ConsoleCharInfo structure.
        /// </summary>
		public char				UnicodeChar
		{
			get { return unicodeCharacter; }
			set { unicodeCharacter = value; }
		}

        /// <summary>
        /// Gets or sets the ASCII character represented by this ConsoleCharInfo structure.
        /// </summary>
		public byte				AsciiChar
		{
			get { return asciiCharacter; }
			set { asciiCharacter = value; }
		}

        /// <summary>
        /// Gets or sets the attributes for this character.
        /// </summary>
		public ZCharAttribute	Attribute
		{
			get { return attr; }
			set { attr = value; }
		}

        /// <summary>
        /// Gets or sets the foreground color attribute.
        /// </summary>
        public Color			ForeColor
		{
			get { return attr.ForeColor; }
			set { attr.ForeColor = value; }
		}

        /// <summary>
        /// Gets or sets the background color attribute.
        /// </summary>
		public Color			BackColor
		{
			get { return attr.BackColor; }
			set { attr.BackColor = value; }
		}

		#endregion
	}	

	

	public class		StatsArea
	{
		#region Private Fields and Constructors

		public StatsArea(int descrLeft, int top, int valueLeft, int valueRight)
		{
			Left		= descrLeft;
			Top			= top;
			ValueLeft	= valueLeft;
			ValueRight	= valueRight;
		}

		#endregion

		#region Public Properties

		public int		Left		{ get; set; }
		public int		Top			{ get; set; }
		public int		ValueLeft	{ get; set; }
		public int		ValueRight	{ get; set; }
		public int		ValueWidth	{ get { return ValueRight - ValueLeft; } }

		#endregion
    }

	#endregion

	
	public static class Tools
	{
		public static List<string>	GetWrappedTextStrings(string[] lines, int maxTextLineLength)
		{
			var textLines = new List<string>();
			var textLine = string.Empty;

			foreach (var line in lines)
			{
				var words = line.Split(' ');
				foreach (var word in words)
				{
					if (textLine.Length + word.Length < maxTextLineLength)
					{
						textLine += word + " ";
					}
					else
					{
						textLines.Add(textLine.TrimEnd());
						var openCount  = textLine.Split('<').Length-1;
						var closeCount = textLine.Split('>').Length-1;
						textLine = word + " ";

						if (openCount > closeCount)
						{
							textLine = "<" + textLine;
						}
					}
				}

				textLines.Add(textLine.TrimEnd());
				textLine = string.Empty;
			}

			return textLines;
		}

		public static string[]		Split(this string str, int chunkSize)
		{
			if (str.Length <= chunkSize)	return new [] { str };
			var list = new List<string>();
			var fullChunkCount = str.Length/chunkSize;
			for (var i = 0; i < fullChunkCount; i++)
			{
				list.Add(str.Substring(i*chunkSize, chunkSize));
			}
				
			list.Add(str.Substring(fullChunkCount*chunkSize, str.Length-fullChunkCount*chunkSize));
			return list.ToArray();
		}

		public static int		Count(this string str, params char[] charsToCount)
		{
			var specCharCount = 0;
			for (var i = 0; i < str.Length; i++)
				for (var j = 0; j < charsToCount.Length; j++)
					if (str[i] == charsToCount[j])
						specCharCount++;
			return specCharCount;
		}


		public static char			Get_UnicodeChar_From_TwoChars(char[] charsToReplace)
		{
			return Get_UnicodeChars_From_TwoCharArray(charsToReplace)[0];
		}

		public static char[]		Get_UnicodeChars_From_TwoCharArray(char[] charsToReplace)
		{
			var byteArray = Encoding.GetEncoding(866).GetBytes(charsToReplace);
			var result = Encoding.Unicode.GetChars(byteArray);
			return result;
		}

		public static byte			Get_Ascii_Byte(char unicodeChar)
		{
			return Encoding.GetEncoding(437).GetBytes(new [] { unicodeChar })[0];
		}


		public static int			SetIntoRange(int value, int min, int max)
		{
			return value > max
				       ? max
				       : value < min
					         ? min
					         : value;
		}


		public static void			Shuffle<T>(this IList<T> list)  
		{  
			var rng = new Random();  
			var n = list.Count;  
			while (n > 1) 
			{  
				n--;  
				var k = rng.Next(n + 1);  
				var value = list[k];  
				list[k] = list[n];  
				list[n] = value;  
			}  
		}
	}
}


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
    sealed class ExtensionAttribute : Attribute
    {
    }
}
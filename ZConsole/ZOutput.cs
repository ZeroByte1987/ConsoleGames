namespace ZConsole
{
	using System;
	using System.Collections.Generic;
	using LowLevel;


	public static class ZOutput
	{
		public static IntPtr	hConsoleOutput;

		public static char		BB_BoldOpenChar		= '<';
		public static char		BB_BoldCloseChar	= '>';
		public static char		BB_ShadedOpenChar	= '^';
		public static char		BB_ShadedCloseChar	= '^';


		public static void		FillCharAttribute(int x, int y, ZCharAttribute attribute, int count = 1)
		{
			var result = 0;
			var coord = new CoordInternal(x, y);
			WinCon.FillConsoleOutputAttribute(hConsoleOutput, attribute, count, coord, ref result);
		}

		public static void		FillCharAttribute(int x, int y, Color foreColor, Color backColor, int count = 1)
		{
			FillCharAttribute(x, y, new ZCharAttribute(foreColor, backColor), count);
		}

		public static void		FillRectCharAttribute(int left, int top, int right, int bottom, ZCharAttribute attribute)
		{
			var result = 0;
			var width  = right  - left + 1;
			var height = bottom - top  + 1;

			for (var i = 0; i < height; i++)
			{
				var coord = new CoordInternal(left, top+i);
				WinCon.FillConsoleOutputAttribute(hConsoleOutput, attribute, width, coord, ref result);	
			}			
		}

		public static void		FillRectCharAttribute(Rect rect, ZCharAttribute attribute)
		{
			FillRectCharAttribute(rect.Left, rect.Top, rect.Right, rect.Bottom, attribute);
		}

		public static void		FillRectCharAttribute(int left, int top, int right, int bottom, Color foreColor, Color backColor)
		{
			FillRectCharAttribute(left, top, right, bottom, new ZCharAttribute(foreColor, backColor));
		}

		public static void		FillRectCharAttribute(Rect rect, Color foreColor, Color backColor)
		{
			FillRectCharAttribute(rect.Left, rect.Top, rect.Right, rect.Bottom, new ZCharAttribute(foreColor, backColor));
		}


		public static void		Print(string text)
		{
			Console.Write(text);
		}
		public static void		Print(string text, Color color)
		{
			ZColors.SetColor(color);
			Console.Write(text);
		}
		public static void		Print(int x, int y, string text)
		{
			Console.SetCursorPosition(x, y);
			Console.Write(text);
		}
		public static void		Print(int x, int y, string text, Color color)
		{
			ZColors.SetColor(color);
			Print(x, y, text);
		}
		public static void		Print(int x, int y, string text, Color color, Color backColor)
		{
			ZColors.SetBackColor(backColor);
			Print(x, y, text, color);
		}
		public static void		Print(int x, int y, string text, ZCharAttribute colors)
		{
			ZColors.SetBackColor(colors.BackColor);
			Print(x, y, text, colors.ForeColor);
		}
		
		public static void		Print(char text)
		{
			Console.Write(text);
		}
		public static void		Print(int x, int y, char text)
		{
			Console.SetCursorPosition(x, y);
			Console.Write(text);
		}
		public static void		Print(int x, int y, char text, Color color)
		{
			ZColors.SetColor(color);
			Print(x, y, text);
		}
		public static void		Print(int x, int y, char text, Color color, Color backColor)
		{
			ZColors.SetBackColor(backColor);
			Print(x, y, text, color);
		}
		public static void		Print(int x, int y, char text, ZCharAttribute colors)
		{
			ZColors.SetBackColor(colors.BackColor);
			Print(x, y, text, colors.ForeColor);
		}

		public static void		Print(int value)
		{
			Console.Write(value);
		}
		public static void		Print(int x, int y, int value)
		{
			Console.SetCursorPosition(x, y);
			Console.Write(value);
		}
		public static void		Print(int x, int y, int value, Color color)
		{
			ZColors.SetColor(color);
			Print(x, y, value);
		}
		public static void		Print(int x, int y, int value, Color color, Color backColor)
		{
			ZColors.SetBackColor(backColor);
			Print(x, y, value, color);
		}
		public static void		Print(int x, int y, int value, ZCharAttribute colors)
		{
			ZColors.SetBackColor(colors.BackColor);
			Print(x, y, value, colors.ForeColor);
		}

		public static void		Print(float value)
		{
			Console.Write(value);
		}
		public static void		Print(int x, int y, float value)
		{
			Console.SetCursorPosition(x, y);
			Console.Write(value);
		}
		public static void		Print(int x, int y, float value, Color color)
		{
			ZColors.SetColor(color);
			Print(x, y, value);
		}
		public static void		Print(int x, int y, float value, Color color, Color backColor)
		{
			ZColors.SetBackColor(backColor);
			Print(x, y, value, color);
		}
		public static void		Print(int x, int y, float value, ZCharAttribute colors)
		{
			ZColors.SetBackColor(colors.BackColor);
			Print(x, y, value, colors.ForeColor);
		}

		public static void		PrintBB(int x, int y, string text, Color regularColor = Color.White, Color boldColor = Color.Yellow, Color shadedColor = Color.DarkGray, Color backColor = Color.Black)
		{
			#region Split source text into tokens

			var tokens = new List<string>();
			var index = 0;
			while (index < text.Length)
			{
				var BB_startIndex = text.IndexOfAny(new [] { BB_BoldOpenChar, BB_ShadedOpenChar }, index);
				if (BB_startIndex == -1)
				{
					tokens.Add(text.Substring(index));
					break;
				}

				var closingChar = (text[BB_startIndex] == BB_BoldOpenChar) ? BB_BoldCloseChar : BB_ShadedCloseChar;

				tokens.Add(text.Substring(index, BB_startIndex-index));
				var BB_endIndex = text.IndexOf(closingChar, BB_startIndex+1);
				if (BB_endIndex == -1)
				{
					tokens.Add(text.Substring(BB_startIndex) + closingChar);
					break;
				}
				tokens.Add(text.Substring(BB_startIndex, BB_endIndex-BB_startIndex+1));
				index = BB_endIndex+1;
			}

			#endregion

			#region Print result text piece by piece
			
			var xPos = x;
			foreach (var token in tokens)
			{
				if (string.IsNullOrEmpty(token))
					continue;

				if (token[0] == BB_BoldOpenChar  ||  token[0] == BB_ShadedOpenChar)
				{
					Print(xPos, y, token.Substring(1, token.Length-2), token[0] == BB_BoldOpenChar ? boldColor : shadedColor, backColor);
					xPos += token.Length - 2;
				}
				else
				{
					Print(xPos, y, token, regularColor, backColor);
					xPos += token.Length;
				}
			}
			
			#endregion
		}
		


		#region Miscellaneous - error messages, etc.

		/// <summary>
		/// Fills a rectangle with specified coords with specified char.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="fillChar"></param>
		/// <param name="foreColor"></param>
		/// <param name="backColor"></param>
		public static void		FillRect(int x, int y, int width, int height, char fillChar, Color foreColor = Color.Gray, Color backColor = Color.Black)
		{
			var newBuffer = new ZCharInfo[height,width];
			var charInfo = new ZCharInfo(Tools.Get_Ascii_Byte(fillChar), new ZCharAttribute(foreColor, backColor));

			for (var i = 0; i < height; i++)
				for (var j = 0; j < width; j++)
					newBuffer[i,j] = charInfo;

			ZBuffer.SaveBuffer("FillRectBuffer", newBuffer);
			ZBuffer.WriteBuffer("FillRectBuffer", x, y);
		}

		public static void		FillRect(Rect rect, char fillChar, Color foreColor = Color.Gray, Color backColor = Color.Black)
		{
			FillRect(rect.Left, rect.Top, rect.Width, rect.Height, fillChar, foreColor, backColor);
		}


		/// <summary>
		/// Prints the text string to console window and waits for a key press.
		/// </summary>
		/// <param name="text">Text to print.</param>
		public static void		PrintWait(string text)
		{
			Print(text);
			ZInput.ReadKey();
		}

		/// <summary>
		/// Prints the error message and closes the application.
		/// </summary>
		/// <param name="text">Text to print.</param>
		public static void		ErrorMsg(string text)
		{
			Print(text);
			Environment.Exit(0);
		}

		/// <summary>
		/// Prints the error message and closes the application.
		/// </summary>
		/// <param name="text">Text to print.</param>
		public static void		ErrorMsgWait(string text)
		{
			Print(text);
			ZInput.ReadKey();
			Environment.Exit(0);
		}

		#endregion
	}
}
namespace ZConsole
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using ZLinq;


	public static class ZMessageLog
	{
		#region Private Fields

		private static List<string> Log;
		private static int Left, Top, Right, Bottom;
		private static int Width		{	get {	return Right - Left;	}}
		private static int Height		{	get {	return Bottom - Top;	}}
		private static int yCurrentPosition;
		public static ColorScheme Colors { get; set; }

	    public static string YesText = "Yes";
        public static string NoText = "No";

		private static int topPosition;
		
		#endregion


		#region Public Methods

		public static void		Initialize(int left, int top, int right, int bottom, Color regularColor = Color.White, Color boldColor = Color.Yellow, Color shadedColor = Color.DarkGray, Color backColor = Color.Black)
		{
			Log		= new List<string>();
			Left	= left;
			Top		= top;
			Right	= right;
			Bottom	= bottom;
			topPosition = top;
			yCurrentPosition = topPosition;
			Colors = new ColorScheme { RegularColor = regularColor, BoldColor = boldColor, ShadedColor = shadedColor, BackColor = backColor };
		}


		public static void		Clear()
		{
			yCurrentPosition = topPosition;
		}


		public static void		Draw_Message(string text, bool useSpacing = true, bool writeToLog = true)
		{
			if (writeToLog)
			{
				Log.Add(text);
			}
				
			CheckLogScrolling((text.Length/Width) + 1 + text.Split('\r').Length-1);
			var lineCount = Draw_WrappedText(Left, yCurrentPosition, text, Width, Colors);
			yCurrentPosition += lineCount + (useSpacing ? 1 : 0);
			CheckLogScrolling(0);
		}


		public static bool		Draw_Message_YesNo(string text, bool buttonsOnSameLine = false, bool isNoDefault = false)
		{			
			CheckLogScrolling(((text.Length+8)/Width) + 2);
			var lineCount = Draw_WrappedText(Left, yCurrentPosition, text, Width, Colors);
			var textLines = text.Split(new [] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries);
			var lastLine = textLines[textLines.Length - 1];

			var result = ZUI.Get_BooleanAnswer(
				buttonsOnSameLine ? Left + lastLine.Length + 2 - 
				lastLine.Count(ZOutput.BB_BoldOpenChar, ZOutput.BB_ShadedOpenChar, ZOutput.BB_BoldCloseChar, ZOutput.BB_ShadedCloseChar) : Width/2-6, 
				yCurrentPosition + lineCount + (buttonsOnSameLine ? -1 : 0), isNoDefault, true);

			yCurrentPosition += lineCount + 1;
			Log.Add(text);
			Log.Add("- " + (result ? YesText : NoText));
			CheckLogScrolling(0);
			return result;
		}


		public static void		ShadowOldMessages()
		{
			ZOutput.FillRectCharAttribute(Left, Top, Right, yCurrentPosition-2, Colors.ShadedColor, Colors.BackColor);
		}


		public static bool		FlushLogToFile(string fileName)
		{
			try
			{
				if (File.Exists(fileName))
				{
					File.AppendAllText(fileName, Log.Aggregate((i, j) => i + "\r\n" + j) + "\r\n");
				}
				else
				{
					File.WriteAllLines(fileName, Log.ToArray());
				}
				Log.Clear();
				return true;
			}
			catch
			{
				return false;
			}
			
		}

		#endregion

		
		#region Private Methods

		private static void		CheckLogScrolling(int lineCount)
		{
			if (yCurrentPosition + lineCount > Bottom)
			{
				ScrollDown(lineCount+1);
				yCurrentPosition -= (lineCount+1);
			}
		}

		private static void		ScrollDown(int rowCount)
		{
			ZBuffer.ReadBuffer( "LogScreenBuffer", Left, Top, Width, Height+1);
			ZBuffer.WriteBuffer("LogScreenBuffer", 0, rowCount, Left, Top, Right, Bottom);
			ZOutput.FillRect(Left, Bottom-rowCount, Width, rowCount, ' ');
		}


		private static int		Draw_WrappedText(int x, int y, string text, int maxWidth, Color regularColor, Color boldColor, Color shadedColor, Color backColor)
		{
			var lines = text.Split(new [] {"\r\n"}, StringSplitOptions.None);
			var textLines = Tools.GetWrappedTextStrings(lines, maxWidth+1);
			
			foreach (var line in textLines)
			{
				ZOutput.PrintBB(x, y++, line, regularColor, boldColor, shadedColor, backColor);
			}
				
			return textLines.Count;
		}

		private static int		Draw_WrappedText(int x, int y, string text, int maxWidth, ColorScheme colors)
		{
			return Draw_WrappedText(x, y, text, maxWidth, colors.RegularColor, colors.BoldColor, colors.ShadedColor, colors.BackColor);
		}

		#endregion


		public class ColorScheme
		{
			public Color	RegularColor { get; set; }
			public Color	BackColor	 { get; set; }
			public Color	BoldColor	 { get; set; }
			public Color	ShadedColor	 { get; set; }

			public ColorScheme()
			{
				RegularColor	= Color.White;
				BackColor		= Color.Black;
				BoldColor		= Color.Yellow;
				ShadedColor		= Color.DarkGray;
			}
		}
	}
}
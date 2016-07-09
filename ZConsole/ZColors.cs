namespace ZConsole
{
	using System;
	using System.Collections.Generic;


	public static class ZColors
	{
		private static readonly Stack<Color> backupForeColors = new Stack<Color>();
		private static readonly Stack<Color> backupBackColors = new Stack<Color>();


		public static Color		GetCurrentColor()
		{
			return (Color)Console.ForegroundColor;
		}

		public static Color		GetCurrentBackColor()
		{
			return (Color)Console.BackgroundColor;
		}

		public static void		StoreColors()
		{
			backupForeColors.Push(GetCurrentColor());
			backupBackColors.Push(GetCurrentBackColor());
		}

		public static void		SetAndStoreColors(Color color, Color backColor)
		{
			StoreColors();
			SetColor(color, backColor);
		}

		public static void		RestoreColors()
		{
			if (backupForeColors.Count > 0  &&  backupBackColors.Count > 0)
			{
				SetColor(backupForeColors.Pop());
				SetBackColor(backupBackColors.Pop());
			}
		}

		public static void		SetColor(Color color)
		{
			Console.ForegroundColor = (ConsoleColor)color;
		}

		public static void		SetColor(Color color, Color backColor)
		{
			Console.ForegroundColor = (ConsoleColor)color;
			Console.BackgroundColor = (ConsoleColor)backColor;
		}

		public static void		SetBackColor(Color backColor)
		{
			Console.BackgroundColor = (ConsoleColor)backColor;
		}
	}
}
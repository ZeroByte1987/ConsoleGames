namespace ZFrontier.Logic
{
	using System;
	using ZConsole;


	public static class ZIOX
	{
		public static string	PressAnyKeyMessage = string.Empty;


		public static void		PressAnyKey(bool expression = true)
		{
			if (!expression) return;

			var xPos = ZFrontier.xEventAreaSize - PressAnyKeyMessage.Length + 1;
			ZOutput.Print(xPos, ZFrontier.yEventAreaSize-1, PressAnyKeyMessage, Color.Red);
			var exitFlag = false;
			while (!exitFlag)
			{
				var key = ZInput.ReadKey();
				if (ZFrontier.HotKeys.ContainsKey(key))
					ZFrontier.HotKeys[key]();
				else exitFlag = true;
			}
			ZOutput.Print(xPos, ZFrontier.yEventAreaSize-1, "".PadRight(PressAnyKeyMessage.Length, ' '), Color.Red);
		}

		public static void		Draw_Coords(int x, int y, int xCoord, int yCoord)
		{
			draw_DoubleValue(x, y, xCoord, yCoord, ",");
		}
		public static void		Draw_State(int x, int y, int currentHP, int maxHP)
		{
			draw_DoubleValue(x, y, currentHP, maxHP, "/");
		}
		public static void		Draw_Currency(int x, int y, int amount)
		{
			draw_SingleValue(x, y, amount, "$");
		}
		public static void		Draw_Currency(int x, int y, int amount, int width, bool clearAfter)
		{
			draw_SingleValue(x, y, amount, "$", width, clearAfter);
		}
		public static void		Draw_Mass(int x, int y, int amount)
		{
			draw_SingleValue(x, y, amount, "t");
		}
		public static void		Draw_CargoLoad(int x, int y, int currentLoad, int maxLoad)
		{
			draw_DoubleValue(x, y, currentLoad, maxLoad, "/", "t");
		}
		public static void		Draw_Date(int x, int y, DateTime date)
		{
			ZOutput.Print(x,	y, date.Year,	Color.White);
			ZOutput.Print(x+4,	y, '-', Color.DarkGray);
			ZOutput.Print(x+5,	y, date.Month.ToString().PadLeft(2, '0'),	Color.White);
			ZOutput.Print(x+7,	y, '-', Color.DarkGray);
			ZOutput.Print(x+8,	y, date.Day.ToString().PadLeft(2, '0'),	Color.White);
		}


		private static void		draw_SingleValue(int x, int y, int amount, string additionalChars, int padLeft = 0, bool clearAfter = true)
		{
			var amountText = amount.ToString();
			if (padLeft > 0)
			{
				amountText = amountText.PadLeft(padLeft, ' ');
			}

			ZOutput.Print(x, y, amountText, Color.White);
			ZOutput.Print(x + amountText.Length, y, additionalChars, Color.DarkGray);
			if (clearAfter)
			{
				ZOutput.Print("  ");
			}
		}
		private static void		draw_DoubleValue(int x, int y, int value1, int value2, string separatorChars)
		{
			var value1Length = value1.ToString().Length;
			ZOutput.Print(x, y, value1, Color.White);
			ZOutput.Print(x+value1Length, y, separatorChars, Color.Cyan);
			ZOutput.Print(x+value1Length + separatorChars.Length, y, value2, Color.White);
			ZOutput.Print("  ");
		}
		private static void		draw_DoubleValue(int x, int y, int value1, int value2, string separatorChars, string additionalChars)
		{
			var value1Length = value1.ToString().Length;
			var separatorLength = separatorChars.Length;
			ZOutput.Print(x, y, value1, Color.White);
			ZOutput.Print(x+value1Length, y, separatorChars, Color.Cyan);
			ZOutput.Print(x+value1Length + separatorLength, y, value2, Color.White);
			ZOutput.Print(x+value1Length + separatorLength + value2.ToString().Length, y, additionalChars, Color.DarkGray);
			ZOutput.Print("  ");
		}		
	}
}



namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Assembly)]
    sealed class ExtensionAttribute : Attribute
    {
    }
}
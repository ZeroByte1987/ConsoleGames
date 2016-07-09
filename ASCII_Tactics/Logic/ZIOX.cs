namespace ASCII_Tactics.Logic
{
	using ZConsole;


	public static class ZIOX
	{
		public enum OutputTypeEnum
		{
			Direct,
			Buffer
		}

		public static OutputTypeEnum OutputType { get; set; }
		public static string		 BufferName { get; set; }


		public delegate void	DrawComplexValue(int x, int y, int value1, int value2, bool isTwoDigitPad = false);
		public delegate void	DrawSingleValue(int x, int y, int value);

		public static void		Draw_Item(StatsArea area, int statIndex, string itemName, bool isActive)
		{
			Print(area.Left, area.Top+statIndex, itemName, Color.DarkGreen, isActive ? Color.DarkMagenta : Color.Black);
		}
		public static void		Draw_StatDescr(StatsArea area, int statIndex, string statName)
		{
			Print(area.Left, area.Top+statIndex, statName, statIndex % 2 == 0 ? Color.Green : Color.Magenta);
			Print(area.Left+statName.Length, area.Top+statIndex, ":", Color.DarkGray);
		}
		public static void		Draw_Stat(StatsArea area, int statIndex, string statName, int statValue)
		{
			Draw_Stat(area, statIndex, statName, statValue.ToString());
		}
		public static void		Draw_Stat(StatsArea area, int statIndex, string statName, string statValue)
		{
			Draw_StatDescr(area, statIndex, statName);
			Print(area.ValueLeft, area.Top+statIndex, statValue.PadRight(area.ValueWidth, ' '), Color.White);
		}
		public static void		Draw_Stat(StatsArea area, int statIndex, string statName, DrawComplexValue drawMethod, int value1, int value2, bool isTwoDigitPad = false)
		{
			Draw_StatDescr(area, statIndex, statName);
			drawMethod(area.ValueLeft, area.Top+statIndex, value1, value2, isTwoDigitPad);
		}
		public static void		Draw_Stat(StatsArea area, int statIndex, string statName, DrawSingleValue drawMethod, int value)
		{
			Draw_StatDescr(area, statIndex, statName);
			drawMethod(area.ValueLeft, area.Top+statIndex, value);
		}

		public static void		Print(int x, int y, string text, Color foreColor, Color backColor = Color.Black)
		{
			if (OutputType == OutputTypeEnum.Direct)
				ZOutput.Print(x, y, text, foreColor, backColor);
			else
				ZBuffer.PrintToBuffer(BufferName, x, y, text, foreColor, backColor);
		}
		public static void		Print(int x, int y, char text, Color foreColor, Color backColor = Color.Black)
		{
			if (OutputType == OutputTypeEnum.Direct)
				ZOutput.Print(x, y, text, foreColor, backColor);
			else
				ZBuffer.PrintToBuffer(BufferName, x, y, text, foreColor, backColor);
		}

		public static void		Draw_Coords(int x, int y, int xCoord, int yCoord)
		{
			draw_DoubleValue(x, y, xCoord, yCoord, ",");
		}
		public static void		Draw_State(int x, int y, int currentHP, int maxHP, bool isTwoDigitPad = false)
		{
			draw_DoubleValue(x, y, currentHP, maxHP, "/", isTwoDigitPad);
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
			draw_SingleValue(x, y, amount, "kg");
		}
		public static void		Draw_CargoLoad(int x, int y, int currentLoad, int maxLoad)
		{
			draw_DoubleValue(x, y, currentLoad, maxLoad, "/", "t");
		}


		private static void		draw_SingleValue(int x, int y, int amount, string additionalChars, int padLeft = 0, bool clearAfter = true)
		{
			var amountText = amount.ToString();
			if (padLeft > 0)
			{
				amountText = amountText.PadLeft(padLeft, ' ');
			}

			Print(x, y, amountText, Color.White);
			Print(x + amountText.Length, y, additionalChars + (clearAfter ? "  " : ""), Color.DarkGray);
		}
		private static void		draw_DoubleValue(int x, int y, int value1, int value2, string separatorChars, bool isTwoDigitPad = false)
		{
			var value1text = value1.ToString();
			if (isTwoDigitPad && value1 < 10)
				value1text = " " + value1text;

			var value1Length = value1text.Length;
			Print(x, y, value1text, Color.White);
			Print(x+value1Length, y, separatorChars, Color.Cyan);
			Print(x+value1Length + separatorChars.Length, y, value2.ToString() + "  ", Color.White);
		}
		private static void		draw_DoubleValue(int x, int y, int value1, int value2, string separatorChars, string additionalChars, bool isTwoDigitPad = false)
		{
			var value1text = value1.ToString();
			if (isTwoDigitPad && value1 < 10)
				value1text = " " + value1text;

			var value1Length = value1text.Length;
			var separatorLength = separatorChars.Length;
			Print(x, y, value1text, Color.White);
			Print(x+value1Length, y, separatorChars, Color.Cyan);
			Print(x+value1Length + separatorLength, y, value2.ToString(), Color.White);
			Print(x+value1Length + separatorLength + value2.ToString().Length, y, additionalChars + "  ", Color.DarkGray);
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
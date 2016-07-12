namespace ZConsole
{
	using System;


	public static class ZUI
	{
        public static string YesText = "Yes";
        public static string NoText  = "No";


		public static void		Draw_Button(int x, int y, string text, bool isActive = false, 
			Color textColor = Color.Cyan, Color activeColor = Color.Yellow, Color bracketsColor = Color.White, Color activeBackColor = Color.DarkGray, Color backColor = Color.Black)
		{
			ZOutput.Print(x, y, "[", bracketsColor, isActive ? activeBackColor : backColor);
			ZOutput.Print(x+text.Length+1, y, "]", bracketsColor, isActive ? activeBackColor : backColor);
			ZOutput.Print(x+1, y, text,	isActive ? activeColor : textColor, isActive ? activeBackColor : backColor);
		}


		public static bool		Get_BooleanAnswer(int x, int y, bool isNoDefault, bool hideButtons, bool canEscape = false, int distance = 1, Color textColor = Color.Cyan, Color backColor = Color.Black)
		{
			var result = !isNoDefault;
			var oldResult = !result;
			var exitFlag = false;

			while (!exitFlag)
			{
				if (oldResult != result)
				{
					Draw_Button(x, y, YesText, result, textColor, backColor:backColor);
					Draw_Button(x+YesText.Length+2 + distance, y, NoText, !result, textColor, backColor:backColor);
					oldResult = result;
				}

				var key = ZInput.ReadKey();
				switch (key)
				{
					case ConsoleKey.LeftArrow:	result = true;		break;
					case ConsoleKey.RightArrow:	result = false;		break;
					case ConsoleKey.Y	:		result = true;	exitFlag = true;	break;
					case ConsoleKey.N	:		result = false;	exitFlag = true;	break;
					
					case ConsoleKey.Enter:
					case ConsoleKey.Spacebar:	exitFlag = true;	break;
					case ConsoleKey.Escape:		if (canEscape)	{ result = false;	exitFlag = true;	}	break;
				}
			}

			if (hideButtons)
			{
				ZOutput.Print(x, y, " ".PadRight(YesText.Length + NoText.Length + 5, ' '), Color.White, Color.Black);
			}
			return result;
		}
	}
}
namespace ZFrontier.Logic.UI.Windows
{
	using System;
	using Objects.GameData;
	using ZConsole;
	using ZLinq;


	public static class HelpInfo
	{
		#region Private Fields

		private static readonly Rect	TableRect	= new Rect(13,  10, 108, 39);

		private const Color	BackColor	= Color.DarkBlue;
		private const Color	HeaderColor	= Color.Yellow;

		#endregion
		
		
		public static void		Show()
		{
			ZBuffer.ReadBuffer("Window", TableRect);
			DrawTable();
			DrawHelpInfo();
			ZBuffer.WriteBuffer("Window", TableRect.Left, TableRect.Top);
			ZColors.SetBackColor(Color.Black);
		}
		

		private static void		DrawTable()
		{
			ZTable.DrawTable(TableRect.Left, TableRect.Top, new ZTable.Table(TableRect.Width, TableRect.Height)
				{
					Caption = "Table",
					Borders = new ZTable.FrameBorders(FrameType.Double),
					BorderColors = new ZCharAttribute(Color.Cyan, BackColor),
					FillColors = new ZCharAttribute(Color.Cyan, BackColor), 
				});
		}


		private static void		DrawHelpInfo()
		{
			var help = GameConfig.HelpSets.ContainsKey(GameConfig.CurrentLanguageName)
				? GameConfig.HelpSets[GameConfig.CurrentLanguageName]
				: GameConfig.HelpSets["English"];
			var allHeaders = help.Select(a => a.Key).ToArray();

			var top			= TableRect.Top  + 2;
			var leftHeaders	= TableRect.Left + 3;
			var leftContent	= TableRect.Left + 22;
			ZColors.SetBackColor(BackColor);

			var index = 0;
			ZColors.SetColor(HeaderColor);
			foreach (var header in allHeaders)
			{
				PrintItem(leftHeaders, top + index*2, header);
				index++;
			}
			
			var exitFlag = false;
			var currentIndex = 0;
			var oldIndex = 1;
			while (!exitFlag)
			{
				if (currentIndex != oldIndex)
				{
					PrintItem(leftHeaders, top + oldIndex*2, allHeaders[oldIndex]);
					PrintItem(leftHeaders, top + currentIndex*2, allHeaders[currentIndex], true);

					ZOutput.FillRect(leftContent, top, TableRect.Width-24, TableRect.Height-3, ' ', Color.Gray, BackColor);
					ZColors.SetColor(Color.White);
					ZColors.SetBackColor(BackColor);
					var topIndex = top;
					var contentLines = Tools.GetWrappedTextStrings(help[allHeaders[currentIndex]].ToArray(), TableRect.Width-26);
					foreach (var t in contentLines)
					{
						ZOutput.PrintBB(leftContent, topIndex++, t, Color.White, Color.Green, Color.Gray, BackColor);
					}
						
					oldIndex = currentIndex;
				}

				var key = ZInput.ReadKey();
				switch (key)
				{
					case ConsoleKey.UpArrow	:	if (currentIndex > 0)	currentIndex--;		else currentIndex = allHeaders.Length-1;	break;
					case ConsoleKey.DownArrow:	if (currentIndex < allHeaders.Length-1)		currentIndex++;	  else currentIndex = 0;	break;
					case ConsoleKey.Escape	:	exitFlag = true;	break;
				}				
			}
		}


		private static void		PrintItem(int x, int y, string text, bool isSelected = false)
		{
			ZOutput.Print(x, y, " " + text.PadRight(15, ' '), HeaderColor, isSelected ? Color.DarkMagenta : BackColor);
		}
	}
}

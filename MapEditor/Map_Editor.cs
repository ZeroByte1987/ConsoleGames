namespace MapEditor
{
	using System;
	using ZConsole;


	public class Map_Editor
	{
		private const int xEditAreaSize		= 80;
		private const int yEditAreaSize		= 50;
		private const int xControlAreaSize	= 40;
		private const int xTotalScreenSize	= xEditAreaSize + xControlAreaSize + 4;
		private const int yTotalScreenSize	= yEditAreaSize + 2;
		

		static void				PrepareGraphicMode()
		{
			ZConsoleMain.Initialize(xTotalScreenSize, yTotalScreenSize);			
			ZConsoleMain.ClearScreen();
			
			ZCursor.SetCursorVisibility(false);
			ZCursor.SetPosition(xEditAreaSize/2, yEditAreaSize/2);
			ZCursor.SetCursorValidArea(1, 1, xEditAreaSize, yEditAreaSize);
			ZCursor.SetCursorChar('↑', Color.Yellow, Color.Black);
		}


		private static void		DrawUI()
		{
			ZTable.DrawTable(0, 0, new ZTable.Table(xTotalScreenSize-1, yTotalScreenSize) 
				{
					Caption = "Table",
					Borders = new ZTable.FrameBorders(FrameType.Double)  { LeftBorder = FrameType.Single },
					BorderColors = new ZCharAttribute(Color.Cyan, Color.Black),
					Cells = new []
						{
							new ZTable.Cell(0,  0,  xEditAreaSize+1, yTotalScreenSize) { Borders = new ZTable.FrameBorders(FrameType.Double) },
							new ZTable.Cell(xEditAreaSize+1, 25,  xTotalScreenSize,  yTotalScreenSize)
						}
				});
		}

		static void				Main()
		{
			PrepareGraphicMode();
			DrawUI();

			ZCursor.GetChar(0, 0);

			var exitFlag = false;
			while (!exitFlag)
			{
				ZCursor.ShowCursor();
				var key = ZInput.ReadKeyInfo();
				ZCursor.HideCursor();

				switch (key.Key)
				{
					case ConsoleKey.LeftArrow	:	ZCursor.MoveCursor(-1, 0);	break;
					case ConsoleKey.RightArrow	:	ZCursor.MoveCursor(+1, 0);	break;
					case ConsoleKey.UpArrow		:	ZCursor.MoveCursor(0, -1);	break;
					case ConsoleKey.DownArrow	:	ZCursor.MoveCursor(0, +1);	break;
					case ConsoleKey.Escape		:	exitFlag = true;		break;
				}
			}
		}
	}
}

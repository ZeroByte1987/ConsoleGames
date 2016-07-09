namespace TileSet_Editor
{
	using ZConsole;


	public class TileSet_Editor
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




		static void Main(string[] args)
		{
			PrepareGraphicMode();

			for (var i = 0; i < 4; i++)
			{
				for (var j = 0; j < 32; j++)
				{
					ZOutput.Print(i*8, j, (i*32+j).ToString() + " " + (char)(i*32+j));
				}
			}


			ZOutput.Print(50, 20, '\\');	ZOutput.Print(51, 21, (char)1);
			ZOutput.Print(54, 20, '|');		ZOutput.Print(54, 21, (char)1);
			ZOutput.Print(58, 20, '/');		ZOutput.Print(57, 21, (char)1);

			ZOutput.Print(50, 23, '-');		ZOutput.Print(51, 23, (char)1);
			//ZOutput.Print(54, 20, '|');	ZOutput.Print(54, 21, 'O');
			ZOutput.Print(58, 23, '-');		ZOutput.Print(57, 23, (char)1);

			ZOutput.Print(50, 26, '/');		ZOutput.Print(51, 25, (char)1);
			ZOutput.Print(54, 26, '|');		ZOutput.Print(54, 25, (char)1);
			ZOutput.Print(58, 26, '\\');	ZOutput.Print(57, 25, (char)1);

		
			ZInput.ReadKey();
		}
	}
}

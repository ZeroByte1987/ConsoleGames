namespace ASCII_Tactics
{
	using Logic;
	using ZConsole;


	class Program
	{
		private static void		PrepareGraphicMode()
		{
			ZConsoleMain.Initialize(Config.WindowSizeX+1, Config.WindowSizeY);
			ZCursor.SetCursorVisibility(false);
			ZBuffer.CreateBuffer("defaultBuffer",		Config.GameAreaSizeX.Max,					Config.GameAreaSizeY.Max);
			ZBuffer.CreateBuffer("InfoBuffer",			Config.InfoAreaSizeX.RangeValue-1,			Config.InfoAreaSizeY.RangeValue);
			ZBuffer.CreateBuffer("TargetInfoBuffer",	Config.TargetInfoAreaSizeX.RangeValue-1,	Config.TargetInfoAreaSizeY.RangeValue);
		}

		
		private static void		Main()
		{
			PrepareGraphicMode();
			
			var mainGame = new MainGame();
			mainGame.InitializeGame();
			mainGame.MainGameLoop();
		}
	}
}
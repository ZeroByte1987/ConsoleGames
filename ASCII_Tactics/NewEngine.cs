namespace ASCII_Tactics
{
	using Config;
	using Logic;
	using Logic.Map;
	using ZConsole;


	class Program
	{
		private static void		PrepareGraphicMode()
		{
			ZConsoleMain.Initialize(UIConfig.WindowSize.Width+1, UIConfig.WindowSize.Height);
			ZCursor.SetCursorVisibility(false);
			ZBuffer.CreateBuffer("defaultBuffer",		UIConfig.GameAreaRect.Size);
			ZBuffer.CreateBuffer("InfoBuffer",			UIConfig.UnitInfoRect.Size);
			ZBuffer.CreateBuffer("TargetInfoBuffer",	UIConfig.TargetInfoRect.Size);
		}

		
		private static void		Main()
		{
			PrepareGraphicMode();

//			LevelsShowRoom.ShowStation();

			var mainGame = new MainGame();			
			mainGame.InitializeGame();
			mainGame.MainGameLoop();
		}
	}
}
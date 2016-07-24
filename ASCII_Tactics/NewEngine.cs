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
			ZConsoleMain.SetConsoleFont("Bm437 IBM BIOS", 8, 8);
			ZConsoleMain.Initialize(UIConfig.WindowSize.Width+1, UIConfig.WindowSize.Height);

			ZCursor.SetCursorVisibility(false);
			ZBuffer.CreateBuffer(UIConfig.Buffer_Default,		UIConfig.GameAreaRect.Size);
			ZBuffer.CreateBuffer(UIConfig.Buffer_Info,			UIConfig.UnitInfoRect.Size);
			ZBuffer.CreateBuffer(UIConfig.Buffer_TargetInfo,	UIConfig.TargetInfoRect.Size);
			ZBuffer.CreateBuffer(UIConfig.Buffer_Inventory,		UIConfig.InventoryRect.Size);
		}

		
		private static void		Main()
		{
			PrepareGraphicMode();

//			LevelsShowRoom.ShowStation();

			SoldierConfig.Initialize();
			var mainGame = new MainGame();			
			mainGame.InitializeGame();
			mainGame.MainGameLoop();
		}
	}
}
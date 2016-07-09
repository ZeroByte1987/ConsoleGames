namespace ASCII_Tactics
{
	using System;
	using Logic;
	using ZConsole;


	class ASCII_Tactics_Game
	{
		static void Main1()
		{
			LoadData();
			PrepareGraphicMode(121, 50);
			DrawMainMenu();
		}

		static void LoadData()
		{
			
		}

		static void PrepareGraphicMode(int xSize, int ySize)
		{
			ZConsoleMain.Initialize();			
			ZConsoleMain.ClearScreen();
			ZConsoleMain.SetWindowSize(xSize+1, ySize);
			ZConsoleMain.WindowSize = new Size(xSize, ySize);
			ZCursor.SetCursorVisibility(false);
		}


		static void DrawMainMenu()
		{
			string result = null;
			do 
			{
				result = ZMenu.GetMenuResult(50, 6,
					new ZMenu.MenuItem
					{ 
						Caption = "Main menu",

						Options = new ZMenu.Options
						{
							Mode = ZMenu.MenuMode.ShortkeysAndArrows,
							ItemSpacing = 2,
							FrameSpacingHorizontal = 3,
							FrameSpacingVertical = 2,
							UseSelectedBackColor = true,
							FrameOptions = new ZFrame.Options
								{
									FrameType = FrameType.Double,
									Width = 10, 
									Height = 4, 
									ColorScheme = new ZFrame.ColorScheme
										{
											CaptionForeColor = Color.Cyan,
											CaptionBackColor = Color.DarkGray
										}
								},						
						},

						ChildMenuItems = new ZMenu.MenuItemList
						{
							new ZMenu.MenuItem { Caption = "Start new game" },
							new ZMenu.MenuItem 
							{ 
								Caption = "Load game",
								Options = new ZMenu.Options { 
									UseSelectedBackColor = true, 
									ForceDimensions = true, 
									FrameSpacingHorizontal = 2,
									FrameSpacingVertical = 2,
									ItemSpacing = 2,
									FrameOptions = new ZFrame.Options { Width = 18, Height = 4 }
								},
								
								ChildMenuItems = new ZMenu.MenuItemList
								{
									new ZMenu.MenuItem { Caption = "Load company save" },
									new ZMenu.MenuItem
										{
											Caption = "Load custom game save",
											ChildMenuItems = new ZMenu.MenuItemList
											{
												new ZMenu.MenuItem { Caption = "aaaaaaaaa" },
												new ZMenu.MenuItem { Caption = "bbbbbbbbb" }
											}
										},
									new ZMenu.MenuItem { Caption = "Load multyplayer game", IsActive = false } 
								}},
							new ZMenu.MenuItem { Caption = "Credits", IsActive = false },
							new ZMenu.MenuItem { Caption = "Quit" }
						}
					}).Text;

				ZOutput.Print(40, 0, result);
				
				if (result == "Start new game")
				{
					var newGame = new MainGame();
					newGame.InitializeGame();
					newGame.MainGameLoop();
				}

				Console.ReadKey();
				ZConsoleMain.ClearScreen();
			} while (!string.IsNullOrEmpty(result));
		}
	}
}

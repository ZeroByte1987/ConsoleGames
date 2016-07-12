namespace ZFrontier
{
	using System;
	using System.Collections.Generic;
	using Logic;
	using Logic.Events;
	using Logic.UI;
	using Logic.UI.Windows;
	using Objects.Galaxy;
	using Objects.GameData;
	using Objects.Units;
	using Objects.Units.PlayerData;
	using ZConsole;
	using ZConsole.Table;
	using ZLinq;


	public class ZFrontier
	{
		#region Release Info

		public const string ReleaseInfo_Version	= "0.5.5";
		public const string ReleaseInfo_Date	= "2015-03-20";
		public const string ReleaseInfo_Email	= "zerobyte666@gmail.com";

		#endregion		

		#region Screen/InterfaceFrames Properties and Consts

		private const int xTotalScreenSize	= 124;
		private const int yTotalScreenSize	= 50;

		public const int  xEventAreaSize	= 70;
		public const int  yEventAreaSize	= 36;
		private const int yGalaxyAreaSize	= 25;
		private const int xPlayerStatsSize	= 26;
		private const int yControlDivider	= 36;
		public const int  yReleaseInfo		= 47;

		public static Rect EventAreaCell		= new Rect(0,					0,					xEventAreaSize+1,		yEventAreaSize);
		public static Rect GalaxyAreaCell		= new Rect(xEventAreaSize+1,	0,					xTotalScreenSize-2,		yGalaxyAreaSize);
		public static Rect ActionAreaCell		= new Rect(0,					yEventAreaSize,		xEventAreaSize+1,		yTotalScreenSize-1);
		public static Rect PlayerStatsAreaCell	= new Rect(xEventAreaSize+1,	yGalaxyAreaSize,	xEventAreaSize+xPlayerStatsSize, yTotalScreenSize-1);
		public static Rect BattleStatsAreaCell	= new Rect(0,					yEventAreaSize,	    14, 23);

		public static Rect EventArea			= new Rect(0,					0,					xEventAreaSize+1,		yEventAreaSize);
		public static Rect GalaxyArea			= new Rect(xEventAreaSize+1,	0,					xTotalScreenSize-2,		yGalaxyAreaSize);
		public static Rect ActionArea			= new Rect(0,					yEventAreaSize,		xEventAreaSize+1,		yTotalScreenSize-1);
		public static Rect PlayerStatsArea		= new Rect(xEventAreaSize+1,	yGalaxyAreaSize,	xEventAreaSize+xPlayerStatsSize, yTotalScreenSize-1);
		public static Rect BattleStatsArea		= new Rect(0,					yEventAreaSize,	14, 7);

		public static int xControls		= PlayerStatsArea.Right + 2;
		public static int yControls		= yGalaxyAreaSize + 1;
		
		#endregion

		#region Global game variables

		public static GalaxyMap			GalaxyMap;
		public static EventLog			EventLog;
		public static ActionPanel		ActionPanel;
		public static PlayerStats		PlayerStats;
		public static BattleStats		BattleStats;

		public static GalaxyModel		Galaxy;
		public static PlayerModel		Player;

		public static StarSystemModel	CurrentStarSystem	{	get {	return Galaxy.StarSystems[Player.PosY, Player.PosX];	}}
		public static TranslationSet	Lang				{	get {	return GameConfig.Lang;		}}

		public static readonly Dictionary<ConsoleKey, ZMenu.MenuAction>	HotKeys = new Dictionary<ConsoleKey, ZMenu.MenuAction>
			{
				{	ConsoleKey.F1,	HelpInfo.Show	},
				{	ConsoleKey.F5,	PlayerInfo.Show	},
				{	ConsoleKey.F6,	GalaxyInfo.Show	},
				{	ConsoleKey.F10,	Quit_Game		}
			};
		
		#endregion


		#region Initial Routines  and  Main Menu

		private static void		PrepareConsoleMode()
		{
			ZColors.SetColor(Color.Gray, Color.Black);
			ZConsoleMain.Initialize(xTotalScreenSize, yTotalScreenSize);
			ZConsoleMain.ClearScreen();
			ZCursor.SetCursorVisibility(false);
			ZConsoleMain.ChangeConsoleCaption("ZFrontier  v" + ReleaseInfo_Version + "  by ZeroByte");
		}


		private static void		Create_NewGame()
		{
			#region Prepare UI and get menu result

			ZConsoleMain.ClearScreen();
			Draw_Interface();
			Draw_Credits();
			var loadGameFileName = MainMenu();

			#endregion

			#region Create Galaxy, Player and all area handlers

			GameConfig.Reset();
			Galaxy = GalaxyModel.Create(GameConfig.CurrentGalaxySizeX, GameConfig.CurrentGalaxySizeY);
			Player = PlayerModel.Create();
			CurrentStarSystem.IsExplored = true;

			GalaxyMap	= new GalaxyMap(GalaxyArea, GameConfig.CurrentGalaxySizeX, GameConfig.CurrentGalaxySizeY);
			EventLog	= new EventLog(EventArea);
			ActionPanel	= new ActionPanel(ActionArea);
			PlayerStats = new PlayerStats(PlayerStatsArea);
			BattleStats	= new BattleStats(BattleStatsArea);
			EventLog.ClearArea();

			#endregion

			#region Load game or show Intro Text and get Player name

			if (!string.IsNullOrEmpty(loadGameFileName))
			{
				SaveLoad.LoadGame(loadGameFileName);
			}
			else
			{
				PrintIntroText();
				EventLog.Print("Common_EnterYourName");
				ZCursor.SetCursorVisibility(true);
				Player.Name = ZInput.ReadLine(2 + Lang["Common_EnterYourName"].Length, 7, 9, Color.Yellow, Color.Black, false, "Jameson");
				ZCursor.SetCursorVisibility(false);
			}

			#endregion

			#region Prepare UI, draw galaxy, etc.

			Draw_Controls(true);
			GlobalEvent.Create(Galaxy, CurrentStarSystem);
			ActionPanel.ClearArea();
			PlayerStats.Draw_PlayerStats();
			GalaxyMap.Draw_GalaxyMap();
			GalaxyMap.HighlightArea();
			EventLog.Print("Galaxy_ChooseSystem");

			#endregion
		}


		private static string	MainMenu()
		{
			#region Menu initialization

			var nestedMenuOptions = new ZMenu.Options
				{
					Mode = ZMenu.MenuMode.ArrowsOnly,
					UseSelectedBackColor = true,
					FrameSpacingHorizontal = 1,
					FrameSpacingVertical = 0,
				};

			var loadGameMenuItems = SaveLoad.GetSaveGames();
		    var exitFlag = false;
			var currentPosition = 0;
			
			#endregion

		    while (!exitFlag)
		    {
				#region Change current displayed options

				var languageMenuItems = new ZMenu.MenuItemList();
				languageMenuItems.AddRange(GameConfig.Languages.Keys.Select(a => new ZMenu.MenuItem(a)));
				var difficultyItems = new ZMenu.MenuItemList((int)GameConfig.CurrentDifficulty);
				difficultyItems.AddRange(Enums.All_Difficulties.Select(a => new ZMenu.MenuItem(Lang["Difficulty_" + a.ToString()])));
				var galaxySizeItems = new ZMenu.MenuItemList(GameConfig.CurrentGalaxySizeX-3);
				galaxySizeItems.AddRange(Enums.All_GalaxySizes.Select(a => new ZMenu.MenuItem(Lang["GalaxySize_" + a.ToString()])));

			    const int xPos = 30;
				ZColors.SetBackColor(Color.Black);
				ZOutput.Print(19, xPos,   (Lang["StartMenu_Difficulty"]	+ ":").PadRight(17, ' '),	Color.Magenta);		ZOutput.Print(38, xPos,   Lang["Difficulty_Short_" + GameConfig.CurrentDifficulty].PadRight(10, ' '), Color.White);
				ZOutput.Print(19, xPos+1, (Lang["StartMenu_GalaxySize"]	+ ":").PadRight(17, ' '),	Color.Green);		ZOutput.Print(38, xPos+1, GameConfig.CurrentGalaxySizeX + " x " + GameConfig.CurrentGalaxySizeY, Color.White);
				ZOutput.Print(19, xPos+2, (Lang["StartMenu_Language"]	+ ":").PadRight(17, ' '),	Color.Magenta);		ZOutput.Print(38, xPos+2, Lang["Common_CurrentLanguage"].PadRight(20, ' '), Color.White);

				#endregion

				#region Get Menu Result

				var menuResult =  ZMenu.GetMenuResult(18, 6,
				    new ZMenu.MenuItem(Lang["Common_YourAction"])
					    {
						    Options = new ZMenu.Options
						    {
							    Mode = ZMenu.MenuMode.ShortkeysAndArrows,
							    ItemSpacing = 2,
							    FrameSpacingHorizontal = 3,
							    FrameSpacingVertical = 2,
							    UseSelectedBackColor = true,
                                FrameOptions = new ZFrame.Options { FrameType = FrameType.Double },						
						    },

						    ChildMenuItems = new ZMenu.MenuItemList(currentPosition)
							    {
									new ZMenu.MenuItem { Caption = Lang["StartMenu_NewGame"]	},
									new ZMenu.MenuItem { Caption = Lang["StartMenu_LoadGame"],	  Options = nestedMenuOptions,	ChildMenuItems = loadGameMenuItems,	 IsActive = loadGameMenuItems.Count > 0 },
									new ZMenu.MenuItem { Caption = Lang["StartMenu_Difficulty"],  Options = nestedMenuOptions,	ChildMenuItems = difficultyItems	},
									new ZMenu.MenuItem { Caption = Lang["StartMenu_GalaxySize"],  Options = nestedMenuOptions,	ChildMenuItems = galaxySizeItems	},
									new ZMenu.MenuItem { Caption = Lang["StartMenu_Language"],	  Options = nestedMenuOptions,	ChildMenuItems = languageMenuItems,	 IsActive = GameConfig.Languages.Count > 0 },
									new ZMenu.MenuItem { Caption = Lang["Common_Quit"]	}
							    }
					    });

				#endregion

				#region Choose action

				if (menuResult.Index == -1)
				{
					menuResult.Text = Lang["Common_Quit"];
				}

				var actions = new Dictionary<string, Action>
				{
					{	Lang["StartMenu_NewGame"],	() => { exitFlag = true;    }},
					{	Lang["Difficulty_Easy"],	() => { GameConfig.CurrentDifficulty = Difficulty.Easy;		currentPosition = 2;	}},
					{	Lang["Difficulty_Normal"],	() => { GameConfig.CurrentDifficulty = Difficulty.Normal;	currentPosition = 2;	}},
					{	Lang["Difficulty_Hard"],	() => { GameConfig.CurrentDifficulty = Difficulty.Hard;		currentPosition = 2;	}},
					{	Lang["GalaxySize_Small"],	() => { GameConfig.CurrentGalaxySizeX = GameConfig.CurrentGalaxySizeY = 3;	currentPosition = 3;	}},
					{	Lang["GalaxySize_Normal"],	() => { GameConfig.CurrentGalaxySizeX = GameConfig.CurrentGalaxySizeY = 4;	currentPosition = 3;	}},
					{	Lang["GalaxySize_Big"],		() => { GameConfig.CurrentGalaxySizeX = GameConfig.CurrentGalaxySizeY = 5;	currentPosition = 3;	}},
					{	Lang["Common_Quit"],		() => {	ZConsoleMain.RestoreMode();	Environment.Exit(0);	}}
				};

                if (actions.ContainsKey(menuResult.Text))
				    actions[menuResult.Text]();
				else if (GameConfig.Languages.ContainsKey(menuResult.Text))
				{
					GameConfig.Lang = GameConfig.Languages[menuResult.Text];
					GameConfig.CurrentLanguageName = menuResult.Text;
					GameConfig.Apply_Languages();
					currentPosition = 4;
				}
				else return menuResult.Text;

				#endregion
		    }
		
			return null;
		}

		
		public static void		Quit_Game()
		{
			if (EventLog.Get_YesNo("Common_QuitConfirmation", true, true))
			{
				EventLog.WriteLogToFile();
				ZConsoleMain.RestoreMode();
				Environment.Exit(0);
			}
		}

		#endregion


		#region Interface routines  and  PrintIntroText

		private static void		PrintIntroText()
		{
			foreach (var line in GameConfig.IntroSets[GameConfig.CurrentLanguageName])
			{
				if (line != "<clr>")
				{
					EventLog.PrintPlainWithoutLog(line);
				}
				else
				{
					ZIOX.PressAnyKey();
					EventLog.Clear();
				}
			}
		}

		private static void		Draw_Interface()
		{
			ZTable.DrawTable(0, 0, new Table(xTotalScreenSize-1, yTotalScreenSize) 
				{
					Caption = "Table",
					Borders = new FrameBorders(FrameType.Double),
					BorderColors = new ZCharAttribute(Color.Cyan, Color.Black),
					Cells = new []
						{
							new Cell(EventAreaCell),
							new Cell(GalaxyAreaCell),
							new Cell(ActionAreaCell),
							new Cell(PlayerStatsAreaCell),
							new Cell(xEventAreaSize+xPlayerStatsSize, yControlDivider, xTotalScreenSize, yTotalScreenSize),
							new Cell(xEventAreaSize+xPlayerStatsSize, yReleaseInfo,    xTotalScreenSize, yTotalScreenSize)
						}
				});
		}

		public static void		Draw_Credits()
		{
			const int infoX = 23, infoY = 39;

			ZOutput.Print(infoX+2, infoY, "ZFrontier", Color.Yellow);
			ZOutput.Print(" v", Color.White);
			ZOutput.Print(ReleaseInfo_Version, Color.Green);
			ZOutput.Print(infoX+3, infoY+2, " by  ", Color.White);
			ZOutput.Print("ZeroByte", Color.Cyan);
			ZOutput.Print(infoX-2, infoY+4, "Release date:  ", Color.Gray);
			ZOutput.Print(ReleaseInfo_Date, Color.DarkGreen);
			ZOutput.Print(infoX, infoY+6, ReleaseInfo_Email, Color.Magenta);
			ZOutput.Print(infoX+11, infoY+6, '@', Color.White);
		}

		public static void		Draw_Controls(bool isActive)
		{
			Draw_ControlKeyInfo(0, "F5 ", Lang["ControlKey_PlayerInfo"]);
			Draw_ControlKeyInfo(1, "F6 ", Lang["ControlKey_GalaxyInfo"]);

			Draw_ControlKeyInfo(3, "F1 ", Lang["ControlKey_Help"]);
			Draw_ControlKeyInfo(5, "F2 ", Lang["ControlKey_StartNewGame"],	isActive);
			Draw_ControlKeyInfo(6, "F3 ", Lang["ControlKey_SaveGame"],		isActive);
			Draw_ControlKeyInfo(7, "F4 ", Lang["ControlKey_LoadGame"],		isActive);
			
			Draw_ControlKeyInfo(9, "F10", Lang["ControlKey_Quit"]);
		}

		private static void		Draw_ControlKeyInfo(int index, string key, string description, bool isActive = true)
		{
			var yPos = yControlDivider + index + 1;
			ZOutput.Print(xControls,  yPos, key, isActive ? Color.Green : Color.DarkGreen);
			ZOutput.Print(xControls+key.Length+1, yPos, '-', isActive ? Color.Yellow : Color.DarkYellow);
			ZOutput.Print(xControls+key.Length+3, yPos, description, isActive ? Color.White : Color.DarkGray);
		}

		#endregion


		#region Global player's actions like Hyperjump or Interplanetary Flight
			
		private static void		JumpToAnotherSystem()
		{
			#region Check whether player is able to jump to the specified star system

			if (Player.Credits < 0)
			{	EventLog.Print("Galaxy_CannotJumpNoMoney");		return;		}

			var distanceX = Math.Abs(GalaxyMap.TargetX - Player.PosX);
			var distanceY = Math.Abs(GalaxyMap.TargetY - Player.PosY);
			if (distanceX > 1  ||  distanceY > 1)
			{	EventLog.Print("Galaxy_CannotJumpRange");		return;		}

			var distance = distanceX + distanceY + 1;
			var fuelPrice = GameConfig.Get_FuelConsumption(GalaxyMap.TargetX, GalaxyMap.TargetY, Player);
			if (Player.FuelLeft - fuelPrice < 0)
			{	EventLog.Print("Galaxy_CannotJumpFuel");		return;		}

			#endregion

			#region Change Player and Galaxy data
				
			EventLog.ShadowOldMessages();
			GalaxyMap.Hide_PlayerShip();
			Player.IsLanded = false;
			Player.FuelLeft -= fuelPrice;
			Player.PosX = GalaxyMap.TargetX;
			Player.PosY = GalaxyMap.TargetY;
			Galaxy.GameDate = Galaxy.GameDate.AddDays(distance);
            Galaxy.Update_AllSystems();
			
			GalaxyMap.Draw_PlayerShip();
			PlayerStats.Draw_PlayerStats();
			EventLog.Print("Galaxy_JumpDone", CurrentStarSystem.Name, fuelPrice.ToString());
			Player.Missions.UpdateAndRemove();

			#endregion
		}


		private static void		InterPlanetaryFlight()
		{
			Draw_Controls(false);
			EventLog.ShadowOldMessages();
			EventLog.Print("Flight_Start", CurrentStarSystem.Name);
			Player.Missions.Process(false);

			if (!Player.IsDead)
			for (var i = 0; i < GameConfig.EventsPerFlight; i++)
			{
				EventLog.Print("Flight_Continue");
				FlightEncounters.CreateEncounter();
				
				EventLog.PrintDivider();
				EventLog.ShadowOldMessages();

				Galaxy.GameDate = Galaxy.GameDate.AddDays(1);
				PlayerStats.Draw_PlayerStats();
				if (Player.IsDead)	break;
			}

			#region If Player Is Dead

			if (Player.IsDead)
			{
				if (Player.EscapeBoat == EquipmentState.Yes)
				{
					Player.Ship = GameConfig.ShipModels.Single(a => a.ModelName == "Eagle");
					Player.CurrentHP	= Player.MaxHP;
					Player.CurrentMissiles = 0;
					Player.CurrentCargo = new CargoStorage();
					Player.Attack		= GameConfig.DefaultPlayers[Difficulty.Hard].Attack;
					Player.Defense		= GameConfig.DefaultPlayers[Difficulty.Hard].Defense;
					Player.ECM			= EquipmentState.No;
					Player.Scanner		= EquipmentState.No;
					Player.MiningLaser	= EquipmentState.No;
					Player.EscapeBoat	= EquipmentState.No;
					foreach (var mission in Player.Missions.Where(a => a.Type == MissionType.Passenger))
					{
						Player.ReputationRating -= mission.MissionTypeData.ReputationChange*2;
						mission.Date = new DateTime(1,1,1);
					}

					EventLog.Print("Common_PlayerIsDeadHasEscapeBoat", Player.Ship.ModelName);
					EventLog.Print("Common_GoodLuck");
				}
				else
				{
					EventLog.Print("Common_PlayerIsDead");
				}

				PlayerStats.Draw_PlayerStats();
				ZIOX.PressAnyKey();
				EventLog.PrintDivider();
				if (Player.IsDead)
					return;
			}

			#endregion

			LandOnPlanet();
		}


		public static void		LandOnPlanet()
		{
			Galaxy.RandomSeed = RNG.GetSeed();
			
			var refuelingPrice = (GameConfig.FuelMax - Player.FuelLeft) * GameConfig.FuelPrice / (GameConfig.FuelMax+1) + 1;
			EventLog.Print("Planet_Landed", CurrentStarSystem.Name, refuelingPrice.ToString());
			Galaxy.Update_AllSystems();
			Player.Missions.UpdateAndRemove();
			Player.Missions.Process(true);

		    Player.IsLanded = true;
			Player.FuelLeft = GameConfig.FuelMax;
			Player.Credits -= GameConfig.FuelPrice;
			CurrentStarSystem.IsExplored = true;
			for (var i = 0; i < CurrentStarSystem.PriceChanges.Length; i++)
				CurrentStarSystem.PriceChanges[i] = RNG.GetNumber(-GameConfig.MaxPriceChange, GameConfig.MaxPriceChange);

			if ((Galaxy.GameDate - Galaxy.GlobalEventLog.Last().EventDate).Days > GameConfig.GlobalEventsCooldown)
		        GlobalEvent.Create(Galaxy, CurrentStarSystem);
			
			GalaxyMap.Draw_GalaxyMap();
			GalaxyMap.Draw_PlayerShip();
			GalaxyMap.Draw_CurrentSystemInfo();

			ZIOX.PressAnyKey();
			GoToStores();
		}


		private static void		GoToStores()
		{
			EventLog.ShadowOldMessages();
			Draw_Controls(false);
			EventLog.Print("Planet_ChooseAction");
			ActionPanel.HighlightArea();
			ActionPanel.PlanetMainMenu();
			GalaxyMap.HighlightArea();
			Draw_Controls(true);
		}


		private static void		ApproachDestination()
		{
			if (GalaxyMap.TargetX == Player.PosX  &&  GalaxyMap.TargetY == Player.PosY)
			{
				if (Player.IsLanded)
				{
					GoToStores();
				}
				else
				{
					EventLog.HighlightArea();
					
					if (CurrentStarSystem.CurrentEvent == GlobalEventType.AlienInvasion)
					{
						if (EventLog.Get_YesNo("GlobalEvent_FlightAlienInvasion"))
						{
							FlightEncounters.DoAlienInvasion();
						}
					}
					else if (EventLog.Get_YesNo("Flight_Confirmation", true))
					{
						InterPlanetaryFlight();
					}
					GalaxyMap.HighlightArea();
				}
			}
			else
			{
				JumpToAnotherSystem();
			}
		}
		
		#endregion


	
		static void Main()
		{			
			PrepareConsoleMode();
			GameConfig.Initialize();
			RNG.Initialize();
			Create_NewGame();

			while (true)
			{
				GalaxyMap.Draw_TargetShip();
				var key = ZInput.ReadKey();
				GalaxyMap.Hide_TargetShip();

				switch (key)
				{
					case ConsoleKey.LeftArrow:	GalaxyMap.Move_Target(-1, 0);	break;
					case ConsoleKey.RightArrow:	GalaxyMap.Move_Target(+1, 0);	break;
					case ConsoleKey.UpArrow	:	GalaxyMap.Move_Target( 0,-1);	break;
					case ConsoleKey.DownArrow:	GalaxyMap.Move_Target( 0,+1);	break;
					case ConsoleKey.Enter	:	ApproachDestination();			break;
					case ConsoleKey.C		:	GalaxyMap.Move_Target(Player.PosX, Player.PosY, true);	break;
					case ConsoleKey.F2		:	if (MessageBox_YesNo.GetResult("NewGame_Confirmation"))	{	EventLog.WriteLogToFile();	Create_NewGame();	}	break;
					case ConsoleKey.F3		:	SaveLoad.SaveGameFull();	break;
					case ConsoleKey.F4		:	SaveLoad.LoadGameFull();	break;
					case ConsoleKey.F10		:	Quit_Game();				break;

					case ConsoleKey.F1		:	HelpInfo.Show();	break;
					case ConsoleKey.F5		:	PlayerInfo.Show();	break;
					case ConsoleKey.F6		:	GalaxyInfo.Show();	break;
				}

				if (Player.IsDead)
				{
					Create_NewGame();
				}
			}
		}
	}
}
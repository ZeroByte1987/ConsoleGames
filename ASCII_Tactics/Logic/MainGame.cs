namespace ASCII_Tactics.Logic
{
	using System;
	using System.Collections.Generic;
	using Config;
	using Extensions;
	using Map;
	using Models.CommonEnums;
	using Render;
	using Models;
	using Models.Map;
	using Soldiers;
	using ZConsole;
	using ZConsole.LowLevel;
	using ZLinq;


	public class MainGame
	{
		public static List<Team>	Teams;
		public static SpaceStation	SpaceStation;
		public static GameState		CurrentGameState;

		public bool		ExitFlag = false;

		public int		CurrentTeamIndex = 0;
		public int		CurrentUnitIndex = 0;

		public Team		CurrentTeam		{ get { return Teams[CurrentTeamIndex]; }}
		public Unit		CurrentUnit		{ get { return CurrentTeam.Units[CurrentUnitIndex]; }}
		public Level	CurrentLevel	{ get { return CurrentUnit.CurrentLevel; }}


		public string	PlayerTeamName = "X-COM Squad";


		public void		DrawUI()
		{
			ZTable.DrawTable(UIConfig.UIAreaRect.Left, UIConfig.UIAreaRect.Top, UIConfig.UITable);
		}


		public void		InitializeGame()
		{
			RNG.Initialize();
			ZConsoleMain.ClearScreen();
			DrawUI();

			SpaceStation = MapGenerator.CreateSpaceStation();

			Teams = new List<Team>();
			var playerTeam = TeamGenerator.CreateTeam(PlayerTeamName, true);
			Teams.Add(playerTeam);

			TeamGenerator.LocateTeamInStation(playerTeam, SpaceStation, 0);
			StartTurn(PlayerTeamName);
			CurrentUnit.Draw();
		}


		public void		Test()
		{
			var time = DateTime.Now;

			for (var i = 0; i < 100; i++)
			{
				CurrentUnit.Draw();
			}
			var ms = (DateTime.Now - time).TotalMilliseconds;
			var n = ms;

			// 500-550
		}


		public void		MainGameLoop()
		{
			Test();

			while (!ExitFlag)
			{
				var input = ZInput.WaitForInput();
				var key = input.KeyEvent;

				if (input.EventType == ConsoleInputEventType.KeyEvent  &&  key.IsKeyDown)
				{
					ProcessKeyEvent(input.KeyEvent);
				}

				if (input.EventType == ConsoleInputEventType.MouseEvent)
				{
					ProcessMouseEvent(input.MouseEvent);
				}

				switch (CurrentGameState)
				{
					case GameState.GameIsWon:
						ZConsoleMain.ClearScreen();
						ZOutput.Print(50, 25, "!!!  YOU  WIN  !!!", Color.Yellow, Color.Black);
						ZInput.ReadKey();
						InitializeGame();
						break;

					case GameState.GameIsLost:
						ZConsoleMain.ClearScreen();
						ZOutput.Print(50, 25, "!!!  YOU  LOST  !!!", Color.Red, Color.Black);
						ZInput.ReadKey();
						InitializeGame();
						break;
				}
			}
		}


		private void	ProcessKeyEvent(ConsoleKeyEventInfo key)
		{
			switch (key.VirtualKeyCode)
			{
				case ConsoleKey.Escape		:	ExitFlag = true;			break;
				case ConsoleKey.F8			:	InitializeGame();			break;
				case ConsoleKey.F5			:	StartTurn(PlayerTeamName);	break;

				case ConsoleKey.F11			:	CurrentGameState = GameState.GameIsWon;		break;
				case ConsoleKey.F12			:	CurrentGameState = GameState.GameIsLost;	break;
				
				case ConsoleKey.LeftArrow	:
				case ConsoleKey.NumPad4		:	if (key.IsAltPressed) CurrentUnit.Move(-1, 0); else CurrentUnit.TurnLeft();		break;
				case ConsoleKey.RightArrow	:
				case ConsoleKey.NumPad6		:	if (key.IsAltPressed) CurrentUnit.Move(+1, 0); else CurrentUnit.TurnRight();	break;
				
				case ConsoleKey.UpArrow	:	
				case ConsoleKey.NumPad8		:	CurrentUnit.Move(0, -1);		break;
				case ConsoleKey.DownArrow:
				case ConsoleKey.NumPad2		:	CurrentUnit.Move(0, +1);		break;

				case ConsoleKey.NumPad7		:	CurrentUnit.Move(-1, -1);	break;
				case ConsoleKey.NumPad9		:	CurrentUnit.Move(+1, -1);	break;
				case ConsoleKey.NumPad1		:	CurrentUnit.Move(-1, +1);	break;
				case ConsoleKey.NumPad3		:	CurrentUnit.Move(+1, +1);	break;

				case ConsoleKey.PageDown	:	CurrentUnit.ChangeStandingState(true);	break;
				case ConsoleKey.PageUp		:	CurrentUnit.ChangeStandingState(false);	break;
				case ConsoleKey.E			:	CurrentUnit.DoAction();					break;

				case ConsoleKey.Tab			:	
					CurrentUnit.Draw();
					CurrentUnitIndex = CurrentUnitIndex < CurrentTeam.Units.Count-1 ? CurrentUnitIndex + 1 : 0;	break;
			}

			CurrentUnit.Draw();
		}

		private void	ProcessMouseEvent(ConsoleMouseEventInfo mouse)
		{
			HideTargetInfo();
			for (var i = 0; i < CurrentTeam.Units.Count; i++)
			{
				var unit = CurrentTeam.Units[i];
				if (unit.Position.Equals(mouse.MousePosition)  &&  IsVisibleToCurrentUnit(unit))
				{
					InfoRender.DrawUnitInfoAsTarget(unit);
				}
			}

			//	Choose a unit
			if (mouse.ButtonState == ConsoleMouseButtonState.LeftButtonPressed)
			{
				for (var i = 0; i < CurrentTeam.Units.Count; i++)
				{
					var unit = CurrentTeam.Units[i];
					if (unit.Position.Equals(mouse.MousePosition)  &&  IsVisibleToCurrentUnit(unit))
					{
						CurrentUnitIndex = i;
						CurrentUnit.Draw();
					}
				}
			}

			//	Select a target
			if (mouse.ButtonState == ConsoleMouseButtonState.RightButtonPressed)
			{
				var allUnits = Teams.SelectMany(w => w.Units).ToArray();
				for (var i = 0; i < allUnits.Length; i++)
				{
					var unit = allUnits[i];
					if (unit.Name != CurrentUnit.Name  &&  unit.Position.Equals(mouse.MousePosition)  &&  IsVisibleToCurrentUnit(unit))
					{
						ShootUnit(CurrentUnit, unit);
					}
				}
			}
		}

		private bool	IsVisibleToCurrentUnit(Unit targetUnit)
		{
			return CurrentUnit.View.IsUnitVisible(CurrentLevel, CurrentUnit.Position, targetUnit.Position) == Visibility.Full;
		}

		


		private void	ShootUnit(Unit attacker, Unit target)
		{
			var currentItem = attacker.ActiveItem;
			var weapon = currentItem.Type.AsWeapon();

//				if (ammo.Amount > 0  &&  unit.Stats.CurrentTU >= weapon.Time_SnapShot)
//				{
//				}
		}


		private void	StartTurn(string teamName)
		{
			var team = Teams.Single(w => w.Name == teamName);
			CurrentGameState = team.IsPlayable ? GameState.PlayerTurn : GameState.MonstersTurn;

			foreach (var unit in team.Units)
			{
				var stats = unit.Stats;

				stats.CurrentTU = stats.MaxTU;
				stats.CurrentHP = stats.CurrentHP == stats.MaxHP ? stats.CurrentHP : stats.CurrentHP += GameConfig.HitPointsRestoredPerTurn;
			}
		}

		private static void	HideTargetInfo()
		{
			ZOutput.FillRect(UIConfig.TargetInfoRect, ' ');
		}
	}
}
namespace ASCII_Tactics.Logic
{
	using System;
	using System.Collections.Generic;
	using Config;
	using Extensions;
	using Map;
	using Models;
	using Models.Items;
	using Models.Map;
	using Models.Tiles;
	using Models.UnitData;
	using View;
	using ZConsole;
	using ZConsole.LowLevel;
	using ZLinq;


	public class MainGame
	{
		public static List<Team>	Teams;
		public static ItemTypes		ItemTypes;
		public static SpaceStation	SpaceStation;

		public bool ExitFlag = false;

		public int CurrentTeamIndex = 0;
		public int CurrentUnitIndex = 0;

		public Team CurrentTeam { get { return Teams[CurrentTeamIndex]; }}
		public Unit CurrentUnit { get { return CurrentTeam.Units[CurrentUnitIndex]; }}


		public void		DrawUI()
		{
			ZTable.DrawTable(UIConfig.UIAreaRect.Left, UIConfig.UIAreaRect.Top, UIConfig.UITable);
		}

		public void		InitializeItems()
		{
			ItemTypes = new ItemTypes();
			ItemTypes.Add(new Weapon("Pistol",		15, 14, 22, 15, 12, 25)		{ Time_SnapShot = 30, Time_AimShot = 50, Time_AutoShot =  0 });
			ItemTypes.Add(new Weapon("Rifle",		45, 20, 30, 30, 20, 40)		{ Time_SnapShot = 40, Time_AimShot = 60, Time_AutoShot = 70 });
			ItemTypes.Add(new Weapon("Shotgun",		45, 25, 40, 18, 10, 50)		{ Time_SnapShot = 40, Time_AimShot = 60, Time_AutoShot =  0 });
			ItemTypes.Add(new Weapon("Sniper Rifle",	65, 35, 50, 40,  5, 50) { Time_SnapShot =  0, Time_AimShot = 50, Time_AutoShot =  0 });
			ItemTypes.Add(new Ammo(  "Pistol Ammo",			3,  "Pistol",		12));
			ItemTypes.Add(new Ammo(  "Rifle Ammo",			10, "Rifle",		20));
			ItemTypes.Add(new Ammo(  "Shotgun Ammo",		8,  "Shotgun",		10));
			ItemTypes.Add(new Ammo(  "Sniper Rifle Ammo",	5,  "Sniper Rifle",  3));
		}

		public void		InitializeGame()
		{
			RNG.Initialize();
			SpaceStation = MapGenerator.CreateSpaceStation();

			ZConsoleMain.ClearScreen();
			InitializeItems();
			DrawUI();

			Teams = new List<Team>();
			var playerTeam = new Team("X-COM Squad", true);
			playerTeam.Add(new Unit("John", 0, 40, 20)
				{
					Stats = new UnitStats
						{
							Accuracy = 50,
							Strength = 50,
							TU = 500,
							MaxHP = 50
						},
					
					View = ViewLogic.Initialize(GameConfig.DefaultViewWidth, 4, GameConfig.DefaultViewDistance),
					Inventory = new Inventory
						{
							ItemTypes["Rifle"].Copy(),
							ItemTypes["Rifle Ammo"].Copy(),
							ItemTypes["Rifle Ammo"].Copy(),
							ItemTypes["Shotgun"].Copy(),
							ItemTypes["Shotgun Ammo"].Copy(),
							ItemTypes["Shotgun Ammo"].Copy(),
						}
				});

			playerTeam.Add(new Unit("Dave", 0, 40, 25)
				{
					Stats = new UnitStats
						{
							Accuracy = 70,
							Strength = 30,
							TU = 450,
							MaxHP = 40
						},

					View = ViewLogic.Initialize(GameConfig.DefaultViewWidth, 7, GameConfig.DefaultViewDistance),
					Inventory = new Inventory
						{
							ItemTypes["Pistol"].Copy(),
							ItemTypes["Pistol Ammo"].Copy(),
							ItemTypes["Sniper Rifle"].Copy(),
							ItemTypes["Sniper Rifle Ammo"].Copy(),
						}
				});

			Teams.Add(playerTeam);
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

			// 1900
		}


		public void		MainGameLoop()
		{
			CurrentUnit.Draw();

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
			}
		}


		private void	ProcessKeyEvent(ConsoleKeyEventInfo key)
		{
			switch (key.VirtualKeyCode)
			{
				case ConsoleKey.Escape		:	ExitFlag = true;				break;
				case ConsoleKey.LeftArrow	:	CurrentUnit.TurnLeft();			break;
				case ConsoleKey.RightArrow	:	CurrentUnit.TurnRight();		break;
				case ConsoleKey.UpArrow		:	CurrentUnit.Move(1);			break;
				case ConsoleKey.DownArrow	:	CurrentUnit.Move(-1);			break;
				case ConsoleKey.PageDown	:	CurrentUnit.ChangeState(true);	break;
				case ConsoleKey.PageUp		:	CurrentUnit.ChangeState(false);	break;
				case ConsoleKey.Enter		:	DoTargetAction();				break;
				case ConsoleKey.E			:	CurrentUnit.DoAction();			break;

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
				if (unit.Position.X == mouse.MousePosition.X  &&  unit.Position.Y == mouse.MousePosition.Y)
				{
					unit.DrawInfoAsTarget();
				}
			}

			//	Choose a unit
			if (mouse.ButtonState == ConsoleMouseButtonState.LeftButtonPressed)
			{
				for (var i = 0; i < CurrentTeam.Units.Count; i++)
				{
					var unit = CurrentTeam.Units[i];
					if (unit.Position.X == mouse.MousePosition.X  &&  unit.Position.Y == mouse.MousePosition.Y)
					{
						CurrentUnitIndex = i;
						CurrentUnit.Draw();
					}
				}
			}

			//	Select a target
			if (mouse.ButtonState == ConsoleMouseButtonState.RightButtonPressed)
			{
				ViewLogic.DrawRay = true;
				CurrentUnit.View.IsRayPossible(
					CurrentUnit.CurrentLevel, CurrentUnit.Position.X, CurrentUnit.Position.Y, 
					mouse.MousePosition.X, mouse.MousePosition.Y, CurrentUnit.Position.IsSitting);
				ViewLogic.DrawRay = false;


				for (var i = 0; i < CurrentTeam.Units.Count; i++)
				{
					var unit = CurrentTeam.Units[i];
					if (unit.Position.X == mouse.MousePosition.X  &&  unit.Position.Y == mouse.MousePosition.Y)
					{
						Shoot(unit.Position);
					}
				}
			}
		}

		public void		DoTargetAction()
		{
			var exitFlag = false;

			var target = CurrentUnit.Position;
			DrawTargetMark(target, true);

			while (!exitFlag)
			{
				var key = ZInput.ReadKey();
				var oldTarget = new Coord(target.X, target.Y);

				switch (key)
				{
					case ConsoleKey.LeftArrow	:	MoveTarget(target, -1,  0);		break;
					case ConsoleKey.RightArrow	:	MoveTarget(target, +1,  0);		break;
					case ConsoleKey.UpArrow		:	MoveTarget(target,  0, -1);		break;
					case ConsoleKey.DownArrow	:	MoveTarget(target,  0, +1);		break;

					case ConsoleKey.Enter		:	Shoot(target);					break;
					case ConsoleKey.Escape		:	exitFlag = true;	break;
				}

				if (!oldTarget.Equals(target))
				{
					DrawTargetMark(oldTarget, false);
					DrawTargetMark(target, true);
				}
			}
		}

		private void	DrawTargetMark(Coord target, bool toShow)
		{
			if (toShow)
			{
				ZBuffer.ReadBuffer("targetBuffer", target.X, target.Y, 3, 3);

				var buffer = ZBuffer.BackupBuffer("defaultBuffer");
				ZIOX.OutputType = ZIOX.OutputTypeEnum.Buffer;
				ZIOX.BufferName = "defaultBuffer";

				var targetColor = IsTargetOnSoldier(target) ? Color.Yellow : Color.Cyan;
				
				ZIOX.Print(target.X-1, target.Y-1, (char)Tools.Get_Ascii_Byte('┌'), targetColor);
				ZIOX.Print(target.X+1, target.Y-1, (char)Tools.Get_Ascii_Byte('┐'), targetColor);
				ZIOX.Print(target.X-1, target.Y+1, (char)Tools.Get_Ascii_Byte('└'), targetColor);
				ZIOX.Print(target.X+1, target.Y+1, (char)Tools.Get_Ascii_Byte('┘'), targetColor);

				CurrentUnit.DrawVisibleUnits();
				ZIOX.OutputType = ZIOX.OutputTypeEnum.Direct;
				
				ZBuffer.WriteBuffer("defaultBuffer", UIConfig.GameAreaRect.Left, UIConfig.GameAreaRect.Top);
				ZBuffer.SaveBuffer("defaultBuffer", buffer);
			}
			else
			{
				ZBuffer.WriteBuffer("targetBuffer", target.X, target.Y);
			}
		}

		private void	MoveTarget(Coord target, int dx, int dy)
		{
			if (target.X + dx >= UIConfig.GameAreaRect.Left  &&  target.X + dx <= UIConfig.GameAreaRect.Right
			&&  target.Y + dy >= UIConfig.GameAreaRect.Top   &&  target.Y + dy <= UIConfig.GameAreaRect.Bottom)
			{
				target.X += dx;
				target.Y += dy;
			}
		}

		private bool	IsTargetOnSoldier(Coord target)
		{
			foreach (var team in Teams)
				foreach (var unit in team.Units.Where(a => a.Name != CurrentUnit.Name))
					if (target.Equals(unit.Position))
						return true;
			return false;
		}

		private void	HideTargetInfo()
		{
			ZOutput.FillRect(UIConfig.TargetInfoRect, ' ');
		}


		private void	Shoot(Coord target)
		{
			var unit = CurrentUnit;

			if (IsTargetOnSoldier(target))
			{
				var currentItem = unit.CurrentItem;
				var ammo = unit.Inventory.First(item => item.Name == currentItem.Name + " Ammo").AsAmmo();
				var weapon = currentItem.AsWeapon();

				if (ammo.Amount > 0  &&  unit.Stats.CurrentTU >= weapon.Time_SnapShot)
				{
				}
			}
		}
	}
}


//	Make rendering more convinient
//	Refactor ZIO to smaller classes
//	Refactor game's mechanics to smaller classes

//	Shoot
//	Choose mode
//	Choose weapon
//	Weapon+Ammo - better work
//	Coords everywhere
//	Terrains - LoS
//	Accuracy
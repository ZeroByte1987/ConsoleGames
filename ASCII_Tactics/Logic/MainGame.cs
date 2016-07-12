namespace ASCII_Tactics.Logic
{
	using System;
	using System.Collections.Generic;
	using Extensions;
	using Models;
	using Models.Items;
	using ZConsole;
	using ZConsole.LowLevel;
	using ZConsole.Table;
	using ZLinq;


	public class MainGame
	{
		public static List<Team>	Teams;
		public static ItemTypes	ItemTypes;

		public int CurrentTeamIndex = 0;
		public int CurrentUnitIndex = 0;

		public Team CurrentTeam { get { return Teams[CurrentTeamIndex]; }}
		public Unit CurrentUnit { get { return CurrentTeam.Units[CurrentUnitIndex]; }}


		public void		DrawUI()
		{
			ZTable.DrawTable(0, 0, new Table(Config.WindowSizeX, Config.WindowSizeY)
				{
					Borders = new FrameBorders(FrameType.Double),
					BorderColors = new ZCharAttribute(Color.Cyan, Color.Black),
					Cells = new []
						{
							new Cell(Config.GameAreaSizeX.Max+1,  0, Config.WindowSizeX, 30),
							new Cell(Config.GameAreaSizeX.Max+1, 30, Config.WindowSizeX, Config.WindowSizeY)
						}
				});
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
			ZConsoleMain.ClearScreen();
			InitializeItems();
			DrawUI();

			Teams = new List<Team>();
			var playerTeam = new Team("X-COM Squad", true);
			playerTeam.Add(new Unit("John", 40, 20)
				{
					InitialAccuracy = 50,
					Strength = 50,
					InitialMaxTU = 50,
					MaxHP = 50,
					View = ViewLogic.Initialize(Config.DefaultViewWidth, 4, Config.DefaultViewDistance),
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

			playerTeam.Add(new Unit("Dave", 40, 25)
				{
					InitialAccuracy = 70,
					Strength = 30,
					InitialMaxTU = 45,
					MaxHP = 40,
					View = ViewLogic.Initialize(Config.DefaultViewWidth, 7, Config.DefaultViewDistance),
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


		public void		MainGameLoop()
		{
			var exitFlag = false;

			while (!exitFlag)
			{
				CurrentUnit.Draw();

				var input = ZInput.ReadInput();
				var key = input.KeyEvent;

				if (input.EventType == ConsoleInputEventType.KeyEvent  &&  key.IsKeyDown)
				{
					switch (key.VirtualKeyCode)
					{
						case ConsoleKey.Escape		:	exitFlag = true;				break;
						case ConsoleKey.LeftArrow	:	CurrentUnit.TurnLeft();			break;
						case ConsoleKey.RightArrow	:	CurrentUnit.TurnRight();		break;
						case ConsoleKey.UpArrow		:	CurrentUnit.Move(1);			break;
						case ConsoleKey.DownArrow	:	CurrentUnit.Move(-1);			break;
						case ConsoleKey.PageDown	:	CurrentUnit.ChangeState(true);	break;
						case ConsoleKey.PageUp		:	CurrentUnit.ChangeState(false);	break;
						case ConsoleKey.Enter		:	DoAction();						break;

						case ConsoleKey.Tab			:	
							CurrentUnit.Draw();
							CurrentUnitIndex = CurrentUnitIndex < CurrentTeam.Units.Count-1 ? CurrentUnitIndex + 1 : 0;	break;
					}
				}

				if (input.EventType == ConsoleInputEventType.MouseEvent)
				{
					ProcessMouseEvent(input.MouseEvent);
				}
			}
		}


		private void	ProcessMouseEvent(ConsoleMouseEventInfo mouse)
		{
			HideTargetInfo();
			for (var i = 0; i < CurrentTeam.Units.Count; i++)
			{
				var unit = CurrentTeam.Units[i];
				if (unit.XPos == mouse.MousePosition.X-1  &&  unit.YPos == mouse.MousePosition.Y-1)
				{
					unit.DrawTargetInfo();
				}
			}

			//	Choose a unit
			if (mouse.ButtonState == ConsoleMouseButtonState.LeftButtonPressed)
			{
				for (var i = 0; i < CurrentTeam.Units.Count; i++)
				{
					var unit = CurrentTeam.Units[i];
					if (unit.XPos == mouse.MousePosition.X-1  &&  unit.YPos == mouse.MousePosition.Y-1)
					{
						CurrentUnit.Draw();
						CurrentUnitIndex = i;
					}
				}
			}

			//	Select a target
			if (mouse.ButtonState == ConsoleMouseButtonState.RightButtonPressed)
			{
				for (var i = 0; i < CurrentTeam.Units.Count; i++)
				{
					var unit = CurrentTeam.Units[i];
					if (unit.XPos == mouse.MousePosition.X-1  &&  unit.YPos == mouse.MousePosition.Y-1)
					{
						Shoot(new Coord(unit.XPos, unit.YPos));
					}
				}
			}
		}

		public void		DoAction()
		{
			var exitFlag = false;

			var target = new Coord(CurrentUnit.XPos, CurrentUnit.YPos);
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
				
				ZBuffer.WriteBuffer("defaultBuffer", Config.GameAreaSizeX.Min, Config.GameAreaSizeY.Min);
				ZBuffer.SaveBuffer("defaultBuffer", buffer);
			}
			else
			{
				ZBuffer.WriteBuffer("targetBuffer", target.X, target.Y);
			}
		}

		private void	MoveTarget(Coord target, int dx, int dy)
		{
			if (target.X + dx >= Config.GameAreaSizeX.Min  &&  target.X + dx < Config.GameAreaSizeX.Max-1
			&&  target.Y + dy >= Config.GameAreaSizeY.Min  &&  target.Y + dy < Config.GameAreaSizeY.Max-1)
			{
				target.X += dx;
				target.Y += dy;
			}
		}

		private bool	IsTargetOnSoldier(Coord target)
		{
			foreach (var team in Teams)
				foreach (var unit in team.Units.Where(a => a.Name != CurrentUnit.Name))
					if (target.X == unit.XPos  &&  target.Y == unit.YPos)
						return true;
			return false;
		}

		private void	HideTargetInfo()
		{
			ZOutput.FillRect(Config.TargetInfoAreaSizeX.Min, Config.TargetInfoAreaSizeY.Min, Config.TargetInfoAreaSizeX.RangeValue-1, Config.TargetInfoAreaSizeY.RangeValue-1, ' ');
		}


		private void	Shoot(Coord target)
		{
			var unit = CurrentUnit;

			if (IsTargetOnSoldier(target))
			{
				var currentItem = unit.CurrentItem;
				var ammo = unit.Inventory.First(item => item.Name == currentItem.Name + " Ammo").AsAmmo();
				var weapon = currentItem.AsWeapon();

				if (ammo.Amount > 0  &&  unit.CurrentTU >= weapon.Time_SnapShot)
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
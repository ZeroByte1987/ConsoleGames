namespace ASCII_Tactics.Models
{
	using System;
	using CommonEnums;
	using Config;
	using Items;
	using Logic;
	using Logic.Extensions;
	using Map;
	using Tiles;
	using UnitData;
	using ZConsole;
	using ZLinq;


	public class Unit
	{
		public int			Id				{ get; set; }
		public string		Name			{ get; set; }
		
		public Position		Position		{ get; set; }
		public UnitStats	Stats			{ get; set; }

		public Inventory	Inventory		{ get; set; }
		public Item			CurrentItem		{ get { return Inventory.CurrentItem; }}

		public ViewLogic	View			{ get; set; }
		public Level		CurrentLevel	{ get { return MainGame.SpaceStation.Levels.SingleOrDefault(w => w.Id == Position.LevelId); }}

		public Team			Team			{ get; set; }


		public Unit(string name, int levelId, int xPos, int yPos)
		{
			Name		= name;
			Position	= new Position(levelId, xPos, yPos);
		}


		#region Public Methods

		public bool		Move(int dx, int dy)
		{
			var moveTimeCost = Math.Abs(dx+dy) == 1 ? 2 : 3;
			if (Stats.CurrentTU < moveTimeCost)
				return false;
			
			var newPosX = Position.X + dx;
			var newPosY = Position.Y + dy;

			if (!CurrentLevel.Map[newPosY, newPosX].Type.IsPassable)
				return false;

			foreach (var team in MainGame.Teams)
				foreach (var unit in team.Units)
					if (newPosX == unit.Position.X  &&  newPosY == unit.Position.Y)
						return false;

			Position.X = newPosX;
			Position.Y = newPosY;
			Stats.CurrentTU -= moveTimeCost;
			return true;
		}

		public bool		Move(int dx)
		{
			return Move(View.DX*dx, View.DY*dx);
		}

		public void		TurnLeft()
		{
			if (Stats.CurrentTU > 0)
			{
				View.TurnLeft();
				Stats.CurrentTU--;
			}
		}
		
		public void		TurnRight()
		{
			if (Stats.CurrentTU > 0)
			{
				View.TurnRight();
				Stats.CurrentTU--;
			}
		}
		
		public void		ChangeState(bool toSit)
		{
			if (Stats.CurrentTU >= 4  &&  Position.IsSitting != toSit)
			{
				Position.IsSitting = toSit;
				Stats.CurrentTU -= 4;
			}
		}


		public void		DoAction()
		{
			var tile = CurrentLevel.Map[Position.Y + View.DY, Position.X + View.DX];

			if (tile.Type.Role == TileRole.Door)
			{
				CurrentLevel.Map[Position.Y + View.DY, Position.X + View.DX] = tile.Type.Name == "OpenedDoor"
					? new Tile("ClosedDoor")
					: new Tile("OpenedDoor");
			}
		}


		public void		DrawVisibleTerrain()
		{
			var sizeX = MapConfig.LevelSize.Width;
			var sizeY = MapConfig.LevelSize.Height;
			var level = CurrentLevel;
			var map = level.Map;
			var viewMap1 = new int[sizeY, sizeX];
			var viewMap2 = new int[sizeY, sizeX];
			
			for (var y = 0; y < sizeY; y++)
				for (var x = 0; x < sizeX; x++)
					if (View.IsPointVisible(level, Position, x, y, Position.IsSitting))
						viewMap1[y, x] = 1;

			for (var y = 0; y < sizeY; y++)
				for (var x = 0; x < sizeX; x++)
				{
					var tile = map[y, x].Type;

					if (tile.Size != ObjectSize.FullTile)
						continue;

					for (var i = -1; i <= 1; i++)
						for (var j = -1; j <= 1; j++)
						{
							var x1 = x + j;
							var y1 = y + i;
							
							if (y1 >= 0  &&  y1 < sizeY  &&  x1 >= 0  &&  x1 < sizeX  &&
								viewMap1[y1, x1] == 1  &&  map[y1, x1].Type.Height == ObjectHeight.None)
							{
								viewMap2[y, x] = 1;
								break;
							}
						}
				}

			for (var y = 0; y < sizeY; y++)
				for (var x = 0; x < sizeX; x++)
					if (viewMap1[y, x] == 1  ||  viewMap2[y, x] == 1)
					{
						var tile = map[y, x].Type;
						ZIOX.Print(x, y, tile.Character, tile.ForeColor, tile.BackColor);
					}
					else
					{
						ZIOX.Print(x, y, ' ', Color.Black);
					}
		}


		public void		DrawVisibleUnits()
		{
			foreach (var team in MainGame.Teams)
				foreach (var unit in team.Units.Where(a => a.Name != Name))
					if (View.IsPointVisible(CurrentLevel, Position, unit.Position))
						ZIOX.Print(unit.Position.X, unit.Position.Y, "@", (team.Name == Team.Name) ? Color.Magenta : Color.Red);
		}


		public void		Draw()
		{
			ZIOX.OutputType = ZIOX.OutputTypeEnum.Buffer;
			ZIOX.BufferName = "defaultBuffer";

			DrawVisibleTerrain();
			DrawVisibleUnits();

			ZBuffer.PrintToBuffer(Position.X, Position.Y, '@', Color.Yellow);
			ZBuffer.WriteBuffer("defaultBuffer", UIConfig.GameAreaRect.Left, UIConfig.GameAreaRect.Top);
			
			DrawInfo();

			ZIOX.OutputType = ZIOX.OutputTypeEnum.Direct;
		}


		public void		DrawInfo()
		{
			ZIOX.BufferName = "InfoBuffer";
			var area = UIConfig.UnitStatsArea;
			
			ZIOX.Draw_Stat(area, 0, "Name",		Name);
			ZIOX.Draw_Stat(area, 1, "Health", 	ZIOX.Draw_State, Stats.CurrentHP, Stats.MaxHP, true);
			ZIOX.Draw_Stat(area, 2, "TU", 		ZIOX.Draw_State, Stats.CurrentTU, Stats.MaxTU, true);
			ZIOX.Draw_Stat(area, 3, "Crouch",	Position.IsSitting.ToString());
			ZIOX.Draw_Stat(area, 5, "Accuracy",	Stats.CurrentAccuracy);
			ZIOX.Draw_Stat(area, 6, "Strength",	Stats.CurrentStrength);
			
			ZIOX.Draw_Stat(area, 8, "In hands",	CurrentItem.Name);
			var ammo = Inventory.First(item => item.Name == CurrentItem.Name + " Ammo");
			ZIOX.Draw_Stat(area, 9, "Ammo",	ammo.AsAmmo().Amount);

			Inventory.Draw(area, 11);			
			
			ZBuffer.WriteBuffer("InfoBuffer", UIConfig.UnitInfoRect.Left, UIConfig.UnitInfoRect.Top);
		}

		public void		DrawInfoAsTarget()
		{
			ZIOX.OutputType = ZIOX.OutputTypeEnum.Buffer;
			ZIOX.BufferName = "TargetInfoBuffer";
			var area = UIConfig.UnitStatsArea;
			
			ZIOX.Draw_Stat(area, 0, "Name",		Name);
			ZIOX.Draw_Stat(area, 1, "Health", 	ZIOX.Draw_State, Stats.CurrentHP, Stats.MaxHP, true);
			ZIOX.Draw_Stat(area, 2, "TU", 		ZIOX.Draw_State, Stats.CurrentTU, Stats.MaxTU, true);
			ZIOX.Draw_Stat(area, 3, "Crouch",	Position.IsSitting.ToString());
			
			ZIOX.Draw_Stat(area, 5, "In hands",	CurrentItem.Name);
			
			ZBuffer.WriteBuffer("TargetInfoBuffer", UIConfig.TargetInfoRect.Left, UIConfig.TargetInfoRect.Top);
			ZIOX.OutputType = ZIOX.OutputTypeEnum.Direct;
		}		

		#endregion		
	}
}
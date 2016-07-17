namespace ASCII_Tactics.Models
{
	using System;
	using Config;
	using Items;
	using Logic;
	using Logic.Extensions;
	using Logic.Map;
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
			var moveTimeCost = Math.Abs(dx+dy) == 1 ? ActionCostConfig.Move : ActionCostConfig.MoveDiagonal;
			if (Position.IsSitting)
			{
				moveTimeCost *= ActionCostConfig.CrouchFactor;
			}

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
			if (Stats.CurrentTU >= ActionCostConfig.Turn)
			{
				View.TurnLeft();
				Stats.CurrentTU -= ActionCostConfig.Turn;
			}
		}

		public void		Strafe(int dx)
		{
			if (Stats.CurrentTU > ActionCostConfig.StrafePenalty + ActionCostConfig.MoveDiagonal)
			{
				if  (dx == -1)
				{
					View.TurnLeft(2);
					Move(1);
					View.TurnRight(2);
				}
				if  (dx == 1)
				{
					View.TurnRight(2);
					Move(1);
					View.TurnLeft(2);
				}
				
				Stats.CurrentTU -= ActionCostConfig.StrafePenalty;
			}
		}
		
		public void		TurnRight()
		{
			if (Stats.CurrentTU > ActionCostConfig.Turn)
			{
				View.TurnRight();
				Stats.CurrentTU -= ActionCostConfig.Turn;
			}
		}
		
		public void		ChangeStandingState(bool toSit)
		{
			var costTU = toSit ? ActionCostConfig.SitDown : ActionCostConfig.StandUp;
			if (Stats.CurrentTU >= costTU  &&  Position.IsSitting != toSit)
			{
				Position.IsSitting = toSit;
				Stats.CurrentTU -= costTU;
			}
		}


		public void		DoAction()
		{
			var tile = CurrentLevel.Map[Position.Y + View.DY, Position.X + View.DX];

			if (tile.Type.Role == TileRole.Door)
			{
				if (Stats.CurrentTU >= ActionCostConfig.OpenCloseDoor)
				{
					CurrentLevel.Map[Position.Y + View.DY, Position.X + View.DX] = tile.Type.Name == "OpenedDoor"
						? new Tile("ClosedDoor")
						: new Tile("OpenedDoor");

					Stats.CurrentTU -= ActionCostConfig.OpenCloseDoor;
				}
			}
		}


		public void		Draw()
		{
			ZIOX.OutputType = ZIOX.OutputTypeEnum.Buffer;
			ZIOX.BufferName = "defaultBuffer";

			MapRender.DrawVisibleTerrain(this);
			MapRender.DrawVisibleUnits(this);

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
			ZIOX.Draw_Stat(area, 5, "Looks",	View.DirectionName);
			
			ZIOX.Draw_Stat(area, 7, "In hands",	CurrentItem.Name);
			
			ZBuffer.WriteBuffer("TargetInfoBuffer", UIConfig.TargetInfoRect.Left, UIConfig.TargetInfoRect.Top);
			ZIOX.OutputType = ZIOX.OutputTypeEnum.Direct;
		}		

		#endregion		
	}
}
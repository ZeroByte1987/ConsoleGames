namespace ASCII_Tactics.Models
{
	using CommonEnums;
	using Config;
	using Items;
	using Logic;
	using Logic.Render;
	using Map;
	using Tiles;
	using UnitData;
	using ZConsole;
	using ZLinq;


	public class Unit
	{
		public string		Name			{ get; set; }
		
		public Position		Position		{ get; set; }
		public UnitStats	Stats			{ get; set; }

		public Inventory	Inventory		{ get; set; }
		public Item			ActiveItem		{ get { return Inventory.ActiveItem; }}

		public ViewLogic	View			{ get; set; }
		public Level		CurrentLevel	{ get { return MainGame.SpaceStation.Levels.SingleOrDefault(w => w.Id == Position.LevelId); }}

		public Team			Team			{ get; set; }


		public Unit(string name, int levelId, int xPos, int yPos)
		{
			Name		= name;
			Position	= new Position(levelId, xPos, yPos);
		}
		public Unit(string name) : this(name, 0, 0, 0)
		{
		}


		public bool		Move(int dx, int dy)
		{
			var moveTimeCost = TimeCost.GetTimeCostForMove(dx, dy, Position.IsSitting, View);
			if (Stats.CurrentTU < moveTimeCost)
				return false;

			var moveDirection = View.GetAbsoluteMoveDirection(dx, dy);
			var newPosX = Position.X + moveDirection.X;
			var newPosY = Position.Y + moveDirection.Y;

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


		public void		TurnLeft()
		{
			if (Stats.CurrentTU >= ActionCostConfig.Turn)
			{
				View.TurnLeft();
				Stats.CurrentTU -= ActionCostConfig.Turn;
			}
		}
		
		public void		TurnRight()
		{
			if (Stats.CurrentTU >= ActionCostConfig.Turn)
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
			var xPos = Position.X + View.DX;
			var yPos = Position.Y + View.DY;
			var tile = CurrentLevel.Map[yPos, xPos];

			if (tile.Type.Role == TileRole.Door)
			{
				if (Stats.CurrentTU >= ActionCostConfig.OpenCloseDoor)
				{
					CurrentLevel.Map[yPos, xPos] = tile.Type.Name == "OpenedDoor"
						? new Tile("ClosedDoor")
						: new Tile("OpenedDoor");

					Stats.CurrentTU -= ActionCostConfig.OpenCloseDoor;
				}
			}

			if (tile.Type.Role == TileRole.Target)
			{
				MainGame.CurrentGameState = GameState.GameIsWon;
				return;
			}

			tile = CurrentLevel.Map[Position.Y, Position.X];
			if (tile.Type.Role == TileRole.Stairs)
			{
				var delta = tile.Type.Name == "StairsUp" ? -1 : 1;
				if (Stats.CurrentTU >= ActionCostConfig.UseStairs)
				{
					Position.LevelId += delta;
					Stats.CurrentTU -= ActionCostConfig.UseStairs;
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
			
			InfoRender.DrawUnitInfo(this);

			ZIOX.OutputType = ZIOX.OutputTypeEnum.Direct;
		}
	}
}
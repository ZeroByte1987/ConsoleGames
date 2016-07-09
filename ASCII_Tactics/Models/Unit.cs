namespace ASCII_Tactics.Models
{
	using System;
	using Items;
	using Logic;
	using Logic.Extensions;
	using ZConsole;
	using ZFC.Maths;
	using ZLinq;


	public class Unit
	{
		#region Public Properties

		public int			Id				{ get; set; }
		public string		Name			{ get; set; }
		public int			XPos			{ get; set; }
		public int			YPos			{ get; set; }
		public bool			IsSitting		{ get; set; }

		public int			MaxHP			{ get; set; }
		public int			CurrentHP		{ get; set; }
	
		public int			InitialMaxTU	{ get; set; }
		public int			MaxTU			{ get { return InitialMaxTU/2 + (InitialMaxTU/2)*(CurrentHP/MaxHP); }}
		public int			CurrentTU		{ get; set; }

		public int			InitialAccuracy	{ get; set; }
		public int			Accuracy		{ get { return InitialAccuracy/2 + (InitialAccuracy/2)*(CurrentHP/MaxHP); }}
		
		public int			Strength		{ get; set; }
		public int			MaxWeight		{ get { return Strength + Strength*(CurrentHP/MaxHP); }}

		public Inventory	Inventory		{ get; set; }
		public Item			CurrentItem		{ get { return Inventory.CurrentItem; }}

		public ViewLogic	View			{ get; set; }

		public Team			Team			{ get; set; }

		#endregion

		#region Public Methods

		public bool		Move(int dx, int dy)
		{
			var moveTimeCost = Math.Abs(dx+dy) == 1 ? 2 : 3;
			if (CurrentTU < moveTimeCost)
				return false;

			var newPosX = XPos + dx;
			var newPosY = YPos + dy;

			foreach (var team in MainGame.Teams)
				foreach (var unit in team.Units)
					if (newPosX == unit.XPos  &&  newPosY == unit.YPos)
						return false;

			if (ZMath.IsInRange(newPosX, 0, ZConsoleMain.WindowSize.Width)  
		     && ZMath.IsInRange(newPosY, 0, ZConsoleMain.WindowSize.Height))
			{
				XPos = newPosX;
				YPos = newPosY;
				CurrentTU -= moveTimeCost;
				return true;
			}

			return false;
		}

		public bool		Move(int dx)
		{
			return Move(View.DX*dx, View.DY*dx);
		}

		public void		TurnLeft()
		{
			if (CurrentTU > 0)
			{
				View.TurnLeft();
				CurrentTU--;
			}
		}
		
		public void		TurnRight(int times = 1)
		{
			if (CurrentTU > 0)
			{
				View.TurnRight();
				CurrentTU--;
			}
		}

		public void		ChangeState(bool toSit)
		{
			if (CurrentTU >= 4  &&  IsSitting != toSit)
			{
				IsSitting = toSit;
				CurrentTU -= 4;
			}
		}


		public void		DrawVisibleTerrain()
		{
			for (var i = 0; i < Config.GameAreaSizeY.Max-1; i++)
				for (var j = 0; j < Config.GameAreaSizeX.Max-1; j++)
					ZIOX.Print(j, i, ".", View.IsPointVisible(XPos, YPos, j, i) ? Color.DarkGray : Color.Black);
		}

		public void		DrawVisibleUnits()
		{
			foreach (var team in MainGame.Teams)
				foreach (var unit in team.Units.Where(a => a.Name != Name))
					if (View.IsPointVisible(XPos, YPos, unit.XPos, unit.YPos))
						ZIOX.Print(unit.XPos, unit.YPos, "@", (team.Name == Team.Name) ? Color.Magenta : Color.Red);
		}

		public void		Draw()
		{
			ZIOX.OutputType = ZIOX.OutputTypeEnum.Buffer;

			ZIOX.BufferName = "defaultBuffer";
			DrawVisibleTerrain();
			DrawVisibleUnits();

			ZBuffer.PrintToBuffer(XPos, YPos, '@', Color.Yellow);
			ZBuffer.WriteBuffer("defaultBuffer", Config.GameAreaSizeX.Min, Config.GameAreaSizeY.Min);
			
			
			DrawInfo();

			ZIOX.OutputType = ZIOX.OutputTypeEnum.Direct;
		}

		public void		DrawInfo()
		{
			ZIOX.BufferName = "InfoBuffer";
			var area = Config.UnitStatsArea;
			
			ZIOX.Draw_Stat(area, 0, "Name",		Name);
			ZIOX.Draw_Stat(area, 1, "Health", 	ZIOX.Draw_State, CurrentHP, MaxHP, true);
			ZIOX.Draw_Stat(area, 2, "TU", 		ZIOX.Draw_State, CurrentTU, MaxTU, true);
			ZIOX.Draw_Stat(area, 3, "Crouch",	IsSitting.ToString());
			ZIOX.Draw_Stat(area, 5, "Accuracy",	Accuracy);
			ZIOX.Draw_Stat(area, 6, "Strength",	Strength);
			
			ZIOX.Draw_Stat(area, 8, "In hands",	CurrentItem.Name);
			var ammo = Inventory.First(item => item.Name == CurrentItem.Name + " Ammo");
			ZIOX.Draw_Stat(area, 9, "Ammo",	ammo.AsAmmo().Amount);

			Inventory.Draw(area, 11);			
			
			ZBuffer.WriteBuffer("InfoBuffer", Config.InfoAreaSizeX.Min, Config.InfoAreaSizeY.Min);
		}

		#endregion

		#region Constructors
		
		public Unit(string name, int xPos, int yPos)
		{
			Name = name;
			XPos = xPos;
			YPos = yPos;
		}

		#endregion		
	}
}

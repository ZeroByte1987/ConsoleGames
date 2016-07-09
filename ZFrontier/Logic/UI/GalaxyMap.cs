namespace ZFrontier.Logic.UI
{
	using System;
	using Common;
	using Objects.Galaxy;
	using Objects.GameData;
	using Objects.Units;
	using ZConsole;
	using ZLinq;


	public class GalaxyMap : AreaUI_Basic
	{
		#region Fields and Properties

		private EventLog		EventLog		{ get { return ZFrontier.EventLog;	}}
		private GalaxyModel		Galaxy			{ get { return ZFrontier.Galaxy;	}}
		private PlayerModel		Player			{ get { return ZFrontier.Player;	}}
		private int				xGalaxyMap		{ get { return AreaRect.Left+2;		}}
		private int				yGalaxyMap		{ get { return AreaRect.Top+2;		}}
		private readonly int	galaxySizeX;
		private readonly int	galaxySizeY;
		private readonly int	starDistanceX;
		private readonly int	starDistanceY;
		
		private const string	StarChars = "*";
		private const string	PlayerShipChars = "<#|";
		private const Color		Color_PlayerShipIsLanded			= Color.Cyan;
		private const Color		Color_PlayerShipIsNotLanded			= Color.Green;
		private const Color		Color_PlayerShipTargetInRange		= Color.DarkGreen;
		private const Color		Color_PlayerShipTargetOutOfRange	= Color.DarkGray;

		public int				TargetX			{ get; set; }
		public int				TargetY			{ get; set; }

		#endregion

		#region Constructors
		
		public GalaxyMap(Rect galaxyArea, int xStarCount, int yStarCount) : base(galaxyArea)
		{
			galaxySizeX		= xStarCount;
			galaxySizeY		= yStarCount;

			starDistanceX	= (galaxyArea.Width -2)/galaxySizeX;
			starDistanceY	= (galaxyArea.Height-2)/galaxySizeY;
			if (galaxySizeY == 5)
				starDistanceY = 5;

			TargetX			= Player.PosX;
			TargetY			= Player.PosY;
			AreaType		= AreaUI.GalaxyMap;
		}

		#endregion


		#region Public Methods

		public void			Draw_CurrentSystemInfo()
		{
			Draw_CurrentSystemInfo(Player.PosX, Player.PosY);
		}
		public void			Draw_CurrentSystemInfo(int coordX, int coordY)
		{
			ZColors.SetBackColor(Color.Black);
			var areaRect = new StatsArea(ZFrontier.xControls, ZFrontier.yControls, ZFrontier.xControls + 13, ZFrontier.xControls + 24);
			var system = Galaxy.StarSystems[coordY, coordX];
			var illegalGoods = system.IllegalGoods.Aggregate(string.Empty, (current, illegalGood) => current + (Enums.Get_Name(illegalGood)[0] + ",")).TrimEnd(',');

			CommonMethods.Draw_Stat(areaRect, 0, Lang["Galaxy_StarSystem"],		system.Name);
			CommonMethods.Draw_Stat(areaRect, 1, Lang["Galaxy_Position"],		ZIOX.Draw_Coords, coordX+1, coordY+1);
			CommonMethods.Draw_Stat(areaRect, 2, Lang["Galaxy_TechLevel"],		system.IsExplored ? system.TechLevel.ToString()			: "?");
			CommonMethods.Draw_Stat(areaRect, 3, Lang["Galaxy_Allegiance"],		system.IsExplored ? Enums.Get_Name(system.Allegiance)	: "?");
			CommonMethods.Draw_Stat(areaRect, 4, Lang["Galaxy_IllegalGoods"],	system.IsExplored ? illegalGoods						: "?");
			CommonMethods.Draw_Stat(areaRect, 5, Lang["Galaxy_YourStatus"],		system.IsExplored ? Enums.Get_Name(Player.LegalRecords[system.Allegiance].Status) : "?");
			if (system.IsExplored)
				CommonMethods.Draw_Stat(areaRect, 6, Lang["Galaxy_FineAmount"],	ZIOX.Draw_Currency, Player.LegalRecords[system.Allegiance].FineAmount);
			else CommonMethods.Draw_Stat(areaRect, 6, Lang["Galaxy_FineAmount"], "?");

			CommonMethods.Draw_Stat(areaRect, 8, Lang["Galaxy_Situation"],  system.IsExplored ? Lang["GlobalEventName_" + system.CurrentEvent] : "?");
            CommonMethods.Draw_Stat(areaRect, 9, Lang["Galaxy_TimeLeft"], system.IsExplored ? (system.EventDuration > 0 ? system.EventDuration + " " + Lang["Statistics_Days"] : "--") : "?");
		}

		public void			Draw_GalaxyMap()
		{
			ClearArea();
			ZOutput.FillRect(xGalaxyMap-1, yGalaxyMap-1, AreaRect.Width-2, AreaRect.Height-2, ' ', Color.White);
			for (var i = 0; i < galaxySizeY; i++)
				for (var j = 0; j < galaxySizeX; j++)
					Draw_StarSystem(j, i, Galaxy.StarSystems[i, j]);
			Draw_PlayerShip();			
		}

		public void			Draw_StarSystem(int xCoord, int yCoord, StarSystemModel starSystem)
		{
			var nameOffset = starSystem.Name.Length/2;
			var xPos = xGalaxyMap + xCoord*starDistanceX + starDistanceX/2 - 1;
			var yPos = yGalaxyMap + yCoord*starDistanceY + starDistanceY/2 - 2;
			var systemColor = starSystem.Allegiance == Allegiance.Empire ? Color.DarkRed
				            : starSystem.Allegiance == Allegiance.Alliance ? Color.DarkCyan : Color.DarkYellow;

			ZColors.SetBackColor(Color.Black);
			if (starSystem.IsExplored)
			{
				ZOutput.Print(xPos-1,	yPos,   Enums.Get_Name(starSystem.Allegiance)[0], systemColor);
				ZOutput.Print(xPos-2,	yPos+2, "Lvl ", Color.DarkMagenta);
				ZOutput.Print(xPos+2,	yPos+2, starSystem.TechLevel, Color.Magenta);
			}
			ZOutput.Print(xPos,				yPos,   StarChars, Color.Yellow);
			ZOutput.Print(xPos-nameOffset,	yPos+1, starSystem.Name, starSystem.CurrentEvent == GlobalEventType.AlienInvasion ? Color.DarkGray : Color.White);
		}

		public void			Draw_PlayerShip()
		{
			Draw_Ship(Player.PosX, Player.PosY, false, Player.IsLanded ? Color_PlayerShipIsLanded : Color_PlayerShipIsNotLanded);
		}
		public void			Hide_PlayerShip()
		{
			Draw_Ship(Player.PosX, Player.PosY, true, Color_PlayerShipIsNotLanded);
		}
		public void			Draw_TargetShip()
		{
			if (TargetX != Player.PosX  ||  TargetY != Player.PosY)
			{
				var canJump = (Math.Abs(TargetX - Player.PosX) <= 1 && Math.Abs(TargetY - Player.PosY) <= 1);
				Draw_Ship(TargetX, TargetY, false, canJump
					? Color_PlayerShipTargetInRange : Color_PlayerShipTargetOutOfRange);
			}
		}
		public void			Hide_TargetShip()
		{
			if (TargetX != Player.PosX  ||  TargetY != Player.PosY)
				Draw_Ship(TargetX, TargetY, true, Color_PlayerShipTargetInRange);
		}

		public void			Move_Target(int dx, int dy, bool useAbsoluteCoords = false)
		{
			if (useAbsoluteCoords)
			{
				TargetX = dx;
				TargetY = dy;
			}
			else
			{
				if (dx == -1  &&  TargetX > 0)					TargetX--;
				if (dx == +1  &&  TargetX < galaxySizeX-1)		TargetX++;
				if (dy == -1  &&  TargetY > 0)					TargetY--;
				if (dy == +1  &&  TargetY < galaxySizeY-1)		TargetY++;
			}
			
			Draw_CurrentSystemInfo(TargetX, TargetY);
		}

		#endregion


		#region Private Properties

		public override void	HighlightArea()
		{
			CommonMethods.HighlightArea(AreaType);
			EventLog.WriteLogToFile();
		}

		private void			Draw_Ship(int xCoord, int yCoord, bool hide, Color color)
		{
			ZColors.SetBackColor(Color.Black);
			var xPos = xGalaxyMap + xCoord*starDistanceX + starDistanceX/2;
			var yPos = yGalaxyMap + yCoord*starDistanceY + starDistanceY/2 - 2;
			ZOutput.Print(xPos, yPos, hide ? "   " : PlayerShipChars, color);

			if (Player.PosX == xCoord  &&  Player.PosY == yCoord)
			{
				var starSystemName = Galaxy.StarSystems[yCoord, xCoord].Name;
				ZOutput.Print(xPos-starSystemName.Length/2-1, yPos+1, starSystemName, hide ? Color.White : Color.Red);
			}
			
			if (color == Color_PlayerShipTargetInRange)
			{
				var fuelConsumption = GameConfig.Get_FuelConsumption(xCoord, yCoord, Player);
				ZOutput.Print(xPos+1, yPos-1, hide  ||  Player.FuelLeft < fuelConsumption ? "   " :  fuelConsumption.ToString(), Color.DarkGray);
			}			
		}

		#endregion			
	}
}
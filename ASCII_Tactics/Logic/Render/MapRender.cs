namespace ASCII_Tactics.Logic.Render
{
	using Config;
	using Models;
	using Models.CommonEnums;
	using ZConsole;
	using ZLinq;


	public static class MapRender
	{
		public static void		DrawVisibleTerrain(Unit unit)
		{
			var sizeX = MapConfig.LevelSize.Width;
			var sizeY = MapConfig.LevelSize.Height;
			var level = unit.CurrentLevel;
			var map = level.Map;
			var viewMap1 = new Visibility[sizeY, sizeX];
			var viewMap2 = new Visibility[sizeY, sizeX];
			
			for (var y = 0; y < sizeY; y++)
				for (var x = 0; x < sizeX; x++)
					viewMap1[y, x] = unit.View.IsPointVisible(level, unit.Position, new Coord(x, y));

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
								viewMap1[y1, x1] != Visibility.None  &&  map[y1, x1].Type.Height == ObjectHeight.None)
							{
								viewMap2[y, x] = Visibility.Full;
								break;
							}
						}
				}

			for (var y = 0; y < sizeY; y++)
				for (var x = 0; x < sizeX; x++)
					if (viewMap1[y, x] == Visibility.Full  ||  viewMap2[y, x] == Visibility.Full)
					{
						var tile = map[y, x].Type;
						ZIOX.Print(x, y, tile.Character, tile.ForeColor, tile.BackColor);
					}
					else if (viewMap1[y, x] == Visibility.Shadow  ||  viewMap2[y, x] == Visibility.Shadow)
					{
						ZIOX.Print(x, y, (char)176, Color.DarkGray);
					}
					else
					{
						ZIOX.Print(x, y, ' ', Color.Black);
					}
		}


		public static void		DrawVisibleUnits(Unit unit)
		{
			foreach (var team in MainGame.Teams)
				foreach (var target in team.Units.Where(a => a.Name != unit.Name))
					if ((unit.Team.Name == target.Team.Name  &&  unit.Position.LevelId == target.Position.LevelId)  ||
						unit.View.IsUnitVisible(unit.CurrentLevel, unit.Position, target.Position) == Visibility.Full)
					{
						ZIOX.Print(target.Position.X, target.Position.Y, "@", (team.Name == unit.Team.Name) ? Color.Magenta : Color.Red);
					}
		}
	}
}
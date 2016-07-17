namespace ASCII_Tactics.Logic.Map
{
	using System.Collections.Generic;
	using Config;
	using Models.CommonEnums;
	using Models.Map;
	using Models.Tiles;
	using ZConsole;


	public static class MapGenerator
	{
		public static SpaceStation	CreateSpaceStation()
		{
			var station = new SpaceStation();
			for (var i = 0; i < RNG.GetNumber(MapConfig.LevelCount); i++)
			{
				var level = CreateLevel(i);
				if (LevelValidator.IsLevelPassable(level))
				{
					station.Levels.Add(level);
					continue;
				}
				i--;
			}

			CreateItemsOnFloor(station);
			return station;
		}


		private static Level		CreateLevel(int levelId)
		{
			var level = new Level(levelId, MapConfig.LevelSize);

			var xLines = GetMapDivisionPoints(MapConfig.LevelSize.Width);
			var yLines = GetMapDivisionPoints(MapConfig.LevelSize.Height);
			var areas  = GetMapAreas(xLines, yLines);
			
			FillInitialLevelMap(level);

			for (var i = 0; i < RNG.GetNumber(MapConfig.RoomCount); i++)
			{
				CreateRoom(level, areas);				
			}

			return level;
		}

		private static List<int>	GetMapDivisionPoints(int dimensionSize)
		{
			var currentPosition = 0;
			var lines = new List<int>();
			while (currentPosition < dimensionSize - MapConfig.RoomSize.Min)
			{
				lines.Add(currentPosition);
				currentPosition += RNG.GetNumber(MapConfig.RoomSize);
			}
			lines.Add(dimensionSize-1);
			return lines;
		}

		private static List<Rect>	GetMapAreas(IList<int> xLines, IList<int> yLines)
		{
			var areas = new List<Rect>();
			for (var i = 0; i < xLines.Count-1; i++)
			{
				for (var j = 0; j < yLines.Count-1; j++)
				{
					areas.Add(new Rect(xLines[i], yLines[j], xLines[i+1], yLines[j+1]));
				}
			}
			return areas;
		}

		private static void			FillInitialLevelMap(Level level)
		{
			for (var y = 0; y < MapConfig.LevelSize.Height; y++)
			{
				for (var x = 0; x < MapConfig.LevelSize.Width; x++)
				{
					var tile = (x == 0 || x == MapConfig.LevelSize.Width - 1 ||
					            y == 0 || y == MapConfig.LevelSize.Height - 1)
						           ? new Tile("Wall")
						           : new Tile("Empty");

					level.Map[y, x] = tile;
				}
			}
		}

		

		private static void			CreateRoom(Level level, IList<Rect> areas)
		{
			var areaIndex = RNG.GetNumber(areas.Count);
			var room = new Room(areas[areaIndex]);
			areas.RemoveAt(areaIndex);

			FillRoomWalls(level, room);
			PlaceDoors(level, room);
			
			for (var i = 0; i < RNG.GetNumber(MapConfig.FurnitureCount); i++)
			{
				var positionX = RNG.GetNumber(room.Area.Left + 1, room.Area.Right - 1);
				var positionY = RNG.GetNumber(room.Area.Top + 1, room.Area.Bottom - 1);

				level.Map[positionY, positionX] = new Tile("Table");
			}
		}

		private static void			FillRoomWalls(Level level, Room room)
		{
			var area = room.Area;
			for (var y = area.Top; y <= area.Bottom; y++)
			{
				for (var x = area.Left; x <= area.Right; x++)
				{
					if (x == area.Left  ||  x == area.Right  ||  y == area.Top  ||  y == area.Bottom)
					{
						level.Map[y, x] = new Tile("Wall");
					}
				}
			}
		}

		private static void			PlaceDoors(Level level, Room room)
		{
			var area = room.Area;
			var possibleSides = new List<Side> { Side.Left, Side.Right, Side.Top, Side.Bottom };

			for (var j = 0; j < RNG.GetNumber(MapConfig.DoorCountPerRoom); j++)
			{
				var side = possibleSides[RNG.GetNumber(0, possibleSides.Count)];
				possibleSides.Remove(side);

				if (side == Side.Left	&&  area.Left == 0  ||
					side == Side.Top	&&  area.Top == 0  ||
					side == Side.Right  &&  area.Right == MapConfig.LevelSize.Width-1  ||
					side == Side.Bottom &&  area.Bottom == MapConfig.LevelSize.Height-1)
				{
					j--;
					continue;
				}

				var doorCoord = side == Side.Left
									? new Coord(area.Left, RNG.GetNumber(area.Top+2, area.Bottom-1))
									: side == Side.Right
											? new Coord(area.Right, RNG.GetNumber(area.Top+2, area.Bottom-1))
											: side == Side.Top
												? new Coord(RNG.GetNumber(area.Left+2, area.Right-1), area.Top)
												: new Coord(RNG.GetNumber(area.Left+2, area.Right-1), area.Bottom);

				level.Map[doorCoord.Y, doorCoord.X]  = new Tile("ClosedDoor");
			}
		}


		private static void			CreateFurniture(Room room)
		{
		}


		private static void			CreateItemsOnFloor(SpaceStation station)
		{
			for (var i = 0; i < station.Levels.Count; i++)
			{
			}
		}
	}
}
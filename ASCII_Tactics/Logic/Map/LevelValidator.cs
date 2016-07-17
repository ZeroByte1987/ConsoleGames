namespace ASCII_Tactics.Logic.Map
{
	using Config;
	using Models.Map;


	public static class LevelValidator
	{
		private const int LiveColor		= 1000;
		private const int DeadColor		= 1001;
		private const int EmptyColor	= 0;
		private const int WallColor		= 1;


		public static bool		IsLevelPassable(Level level)
		{
			var testMap = GetTestMap(level);
			testMap[1,1] = LiveColor;

			var isFinished = false;
			while (!isFinished)
			{
				isFinished = DoNextStep(testMap);
			}

			return IsLevelPassable(testMap);
		}


		private static int[,]	GetTestMap(Level level)
		{
			var testMap = new int[MapConfig.LevelSize.Height, MapConfig.LevelSize.Width];
			for (var i = 0; i < MapConfig.LevelSize.Height; i++)
			{
				for (var j = 0; j < MapConfig.LevelSize.Width; j++)
				{
					testMap[i,j] = level.Map[i,j].Type.Id;
				}
			}
			return testMap;
		}

		private static bool		DoNextStep(int[,] map)
		{
			var isFinished = true;
			for (var i = 0; i < MapConfig.LevelSize.Height; i++)
			{
				for (var j = 0; j < MapConfig.LevelSize.Width; j++)
				{
					if (map[i,j] == LiveColor)
					{						
						for (var y = -1; y <= 1; y++)
							for (var x = -1; x <= 1; x++)
								if (map[i+y, j+x] != WallColor  &&  map[i+y, j+x] != LiveColor  &&  map[i+y, j+x] != DeadColor)
								{
									map[i+y, j+x] = LiveColor;
									isFinished = false;
								}
						map[i,j] = DeadColor;
					}
				}
			}
			return isFinished;
		}

		private static bool		IsLevelPassable(int[,] map)
		{
			for (var i = 0; i < MapConfig.LevelSize.Height; i++)
			{
				for (var j = 0; j < MapConfig.LevelSize.Width; j++)
				{
					if (map[i,j] == EmptyColor)
					{
						return false;
					}
				}
			}
			return true;
		}
	}
}
namespace ASCII_Tactics.Logic.Map
{
	using System;
	using Config;
	using Models.Map;
	using ZConsole;


	public static class LevelsShowRoom
	{
		public static void		ShowStation()
		{
			RNG.Initialize();
			var station = MapGenerator.CreateSpaceStation();
			var currentLevelIndex = 0;

			while (true)
			{
				ShowLevel(station.Levels[currentLevelIndex]);

				var key = ZInput.ReadKey();

				switch (key)
				{
					case ConsoleKey.UpArrow:
						currentLevelIndex = currentLevelIndex == 0 ? 0 : currentLevelIndex - 1;
						break;

					case ConsoleKey.DownArrow:
						currentLevelIndex = currentLevelIndex == station.Levels.Count-1 ? station.Levels.Count-1 : currentLevelIndex + 1;
						break;

					case ConsoleKey.Escape:
						Environment.Exit(0);
						break;
				}
			}
		}


		private static void		ShowLevel(Level level)
		{
			ZIOX.OutputType = ZIOX.OutputTypeEnum.Buffer;
			ZIOX.BufferName = "defaultBuffer";

			for (var y = 0; y < level.Size.Height; y++)
			{
				for (var x = 0; x < level.Size.Width; x++)
				{
					var tile = level.Map[y, x].Type;
					ZIOX.Print(x, y, tile.Character, tile.ForeColor, tile.BackColor);
				}
			}

			ZBuffer.WriteBuffer("defaultBuffer", UIConfig.GameAreaRect.Left, UIConfig.GameAreaRect.Top);
			ZIOX.OutputType = ZIOX.OutputTypeEnum.Direct;
		}
	}
}
namespace ASCII_Tactics
{
	using ZConsole;


	public static class Config
	{
		public const int		WindowSizeX			= 120;
		public const int		WindowSizeY			= 52;

		public static Range		GameAreaSizeX		= new Range(1, 90);
		public static Range		GameAreaSizeY		= new Range(1, 50);

		public static Range		InfoAreaSizeX		= new Range(92, 119);
		public static Range		InfoAreaSizeY		= new Range(1, 29);
		
		public static Range		TargetInfoAreaSizeX	= new Range(92, 119);
		public static Range		TargetInfoAreaSizeY	= new Range(31, 50);

		public static int		DefaultViewWidth	= 90;
		public static int		DefaultViewDistance = 25;

		public static StatsArea	UnitStatsArea		= new StatsArea(1, 1, 12, 26);
	}
}
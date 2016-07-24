namespace ASCII_Tactics.Config
{
	using ZConsole;
	using ZConsole.Table;


	public static class UIConfig
	{
		public static Size		WindowSize			= new Size(120, 62);
		
		public static Rect		GameAreaRect		= new Rect( 0,  0,  91, 61);

		public static Rect		UIAreaRect			= new Rect(92,  0, 120, 61);
		public static Rect		UnitInfoRect		= new Rect(93,  1, 118, 16);
		public static Rect		InventoryRect		= new Rect(93, 16, 118, 29);
		public static Rect		TargetInfoRect		= new Rect(93, 31, 118, 57);
		public static Rect		PositionInfoRect	= new Rect(93, 59, 118, 61);

		public static string	Buffer_Default		= "defaultBuffer";
		public static string	Buffer_Info			= "InfoBuffer";
		public static string	Buffer_TargetInfo	= "TargetInfoBuffer";
		public static string	Buffer_Inventory	= "InventoryBuffer";

		public static StatsArea	UnitStatsArea		= new StatsArea(1, 1, 12, 26);
		public static StatsArea	InventoryStatsArea	= new StatsArea(1, 1, 12, 26);

		public static Table		UITable	= new Table(UIAreaRect.Width, UIAreaRect.Height)
			{
				Borders = new FrameBorders(FrameType.Double),
				BorderColors = new ZCharAttribute(Color.Cyan, Color.Black),
				Cells = new[]
					{
						new Cell(0,  0, UIAreaRect.Width, 30),
						new Cell(0, 30, UIAreaRect.Width, UIAreaRect.Height-3),
						new Cell(0, PositionInfoRect.Top, UIAreaRect.Width,  PositionInfoRect.Bottom)
					}
			};
	}
}
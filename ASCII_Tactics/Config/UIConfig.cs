namespace ASCII_Tactics.Config
{
	using ZConsole;
	using ZConsole.Table;


	public static class UIConfig
	{
		public static Size		WindowSize		= new Size(120, 62);
		
		public static Rect		GameAreaRect	= new Rect( 0,  0,  91, 61);

		public static Rect		UIAreaRect		= new Rect(92,  0, 120, 61);
		public static Rect		UnitInfoRect	= new Rect(93,  1, 118, 29);
		public static Rect		TargetInfoRect	= new Rect(93, 31, 118, 60);

		public static StatsArea	UnitStatsArea	= new StatsArea(1, 1, 12, 26);

		public static Table		UITable	= new Table(UIAreaRect.Width, UIAreaRect.Height)
			{
				Borders = new FrameBorders(FrameType.Double),
				BorderColors = new ZCharAttribute(Color.Cyan, Color.Black),
				Cells = new[]
					{
						new Cell(0,  0, UIAreaRect.Width, 30),
						new Cell(0, 30, UIAreaRect.Width, UIAreaRect.Height)
					}
			};
	}
}
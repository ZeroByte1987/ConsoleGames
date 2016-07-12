namespace ZConsole.Table
{
	public class FrameBorders
	{
		public FrameType	TopBorder		{ get; set; }
		public FrameType	BottomBorder	{ get; set; }
		public FrameType	LeftBorder		{ get; set; }
		public FrameType	RightBorder		{ get; set; }

		public FrameBorders() : this(FrameType.Single)
		{
		}

		public FrameBorders(FrameType allBordersType)
		{
			TopBorder = BottomBorder = LeftBorder = RightBorder = allBordersType;
		}
	}
}
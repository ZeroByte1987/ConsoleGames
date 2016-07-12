namespace ZConsole.Table
{
	public class Cell
	{
		public Rect				Dimensions		{ get; set; }
		public FrameBorders		Borders			{ get; set; }
		public ZCharAttribute?	BorderColors	{ get; set; }
		public ZCharAttribute?	FillColors		{ get; set; }
		public string			Caption			{ get; set; }


		public Cell(int left, int top, int right, int bottom) : this(new Rect(left, top, right, bottom))
		{
		}

		public Cell(Rect rect)
		{
			Dimensions	= rect;
			Borders		= new FrameBorders();
		}
	}
}
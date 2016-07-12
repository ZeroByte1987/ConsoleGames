namespace ZConsole.Table
{
	using System;

	public class Table
	{
		public Size				Dimensions		{ get; set; }
		public FrameBorders		Borders			{ get; set; }
		public ZCharAttribute?	BorderColors	{ get; set; }
		public ZCharAttribute?	FillColors		{ get; set; }
		public Cell[]			Cells			{ get; set; }
		public string			Caption			{ get; set; }
		public bool				UseCellBordersForOuterBorder { get; set; }

		public Table(int width, int height)
		{
			Dimensions	= new Size(width, height);
			Borders	= new FrameBorders();
			Cells	= new Cell[0];
		}

		internal void _validate()
		{
			foreach (var cell in Cells)
			{
				cell.BorderColors	= cell.BorderColors ?? BorderColors;
				cell.FillColors		= cell.FillColors	?? FillColors;
				cell.Dimensions.Right  = Math.Min(cell.Dimensions.Right,  Dimensions.Width - 1);
				cell.Dimensions.Bottom = Math.Min(cell.Dimensions.Bottom, Dimensions.Height - 1);
			}
		}
	}
}
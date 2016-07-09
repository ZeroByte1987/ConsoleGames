namespace ZConsole 
{
	using System;
	using System.Collections.Generic;


	public static class ZTable
	{
		private static readonly Dictionary<NodeEnum, char> frameChars = new Dictionary<NodeEnum, char>();

		private static Node[,] currentBuffer;

		static ZTable()
		{
			frameChars.Add(NodeEnum.None, ' ');
			frameChars.Add(NodeEnum.UpSingle   | NodeEnum.DownSingle, '│');
			frameChars.Add(NodeEnum.UpSingle   | NodeEnum.DownSingle | NodeEnum.LeftSingle, '┤');
			frameChars.Add(NodeEnum.UpSingle   | NodeEnum.DownSingle | NodeEnum.LeftDouble, '╡');
			frameChars.Add(NodeEnum.UpDouble   | NodeEnum.DownDouble | NodeEnum.LeftSingle, '╢');
			frameChars.Add(NodeEnum.DownDouble | NodeEnum.LeftSingle, '╖');
			frameChars.Add(NodeEnum.DownSingle | NodeEnum.LeftDouble, '╕');
			frameChars.Add(NodeEnum.UpDouble   | NodeEnum.DownDouble | NodeEnum.LeftDouble, '╣');
			frameChars.Add(NodeEnum.UpDouble   | NodeEnum.DownDouble, '║');
			frameChars.Add(NodeEnum.DownDouble | NodeEnum.LeftDouble, '╗');
			frameChars.Add(NodeEnum.UpDouble   | NodeEnum.LeftDouble, '╝');
			frameChars.Add(NodeEnum.UpDouble   | NodeEnum.LeftSingle, '╜');
			frameChars.Add(NodeEnum.UpSingle   | NodeEnum.LeftDouble, '╛');
			frameChars.Add(NodeEnum.DownSingle | NodeEnum.LeftSingle, '┐');
			frameChars.Add(NodeEnum.UpSingle   | NodeEnum.RightSingle, '└');
			frameChars.Add(NodeEnum.UpSingle   | NodeEnum.LeftSingle | NodeEnum.RightSingle, '┴');
			frameChars.Add(NodeEnum.DownSingle | NodeEnum.LeftSingle | NodeEnum.RightSingle, '┬');
			frameChars.Add(NodeEnum.UpSingle   | NodeEnum.DownSingle | NodeEnum.RightSingle, '├');
			frameChars.Add(NodeEnum.LeftSingle | NodeEnum.RightSingle, '─');
			frameChars.Add(NodeEnum.UpSingle   | NodeEnum.DownSingle | NodeEnum.LeftSingle | NodeEnum.RightSingle, '┼');
			frameChars.Add(NodeEnum.UpSingle   | NodeEnum.DownSingle | NodeEnum.RightDouble, '╞');
			frameChars.Add(NodeEnum.UpDouble   | NodeEnum.DownDouble | NodeEnum.RightSingle, '╟');
			frameChars.Add(NodeEnum.UpDouble   | NodeEnum.RightDouble, '╚');
			frameChars.Add(NodeEnum.DownDouble | NodeEnum.RightDouble, '╔');
			frameChars.Add(NodeEnum.UpDouble   | NodeEnum.LeftDouble | NodeEnum.RightDouble, '╩');
			frameChars.Add(NodeEnum.DownDouble | NodeEnum.LeftDouble | NodeEnum.RightDouble, '╦');
			frameChars.Add(NodeEnum.UpDouble   | NodeEnum.DownDouble | NodeEnum.RightDouble, '╠');
			frameChars.Add(NodeEnum.LeftDouble | NodeEnum.RightDouble, '═');
			frameChars.Add(NodeEnum.UpDouble   | NodeEnum.DownDouble | NodeEnum.LeftDouble | NodeEnum.RightDouble, '╬');
			frameChars.Add(NodeEnum.UpSingle   | NodeEnum.LeftDouble | NodeEnum.RightDouble, '╧');
			frameChars.Add(NodeEnum.UpDouble   | NodeEnum.LeftSingle | NodeEnum.RightSingle, '╨');
			frameChars.Add(NodeEnum.DownSingle | NodeEnum.LeftDouble | NodeEnum.RightDouble, '╤');
			frameChars.Add(NodeEnum.DownDouble | NodeEnum.LeftSingle | NodeEnum.RightSingle, '╥');
			frameChars.Add(NodeEnum.UpDouble   | NodeEnum.RightSingle, '╙');
			frameChars.Add(NodeEnum.UpSingle   | NodeEnum.RightDouble, '╘');
			frameChars.Add(NodeEnum.DownSingle | NodeEnum.RightDouble, '╒');
			frameChars.Add(NodeEnum.DownDouble | NodeEnum.RightSingle, '╓');
			frameChars.Add(NodeEnum.UpDouble   | NodeEnum.DownDouble | NodeEnum.LeftSingle | NodeEnum.RightSingle, '╫');
			frameChars.Add(NodeEnum.UpSingle   | NodeEnum.DownSingle | NodeEnum.LeftDouble | NodeEnum.RightDouble, '╪');
			frameChars.Add(NodeEnum.UpSingle   | NodeEnum.LeftSingle, '┘');
			frameChars.Add(NodeEnum.DownSingle | NodeEnum.RightSingle, '┌');
		}


		public static void	DrawTable(int x, int y, Table table)
		{
			table.validate();

			var width  = table.Dimensions.Width;
			var height = table.Dimensions.Height;
			var buffer = new Node[height, width];
			putFrameIntoBuffer(buffer, new Rect(0, 0, width-1 , height-1), table.Borders, table.BorderColors, table.FillColors);

			foreach (var cell in table.Cells)
			{
				putFrameIntoBuffer(buffer, cell.Dimensions, cell.Borders, cell.BorderColors, cell.FillColors);
			}

			var newBuffer = new ZCharInfo[height,width];

			for (var i = 0; i < height; i++)
			{
				for (var j = 0; j < width; j++)
				{
					var charInfo = buffer[i, j];

					newBuffer[i,j] = new ZCharInfo(
						Tools.Get_Ascii_Byte(frameChars[charInfo.NodeEnumItem]), 
						new ZCharAttribute(charInfo.Colors.ForeColor, charInfo.Colors.BackColor));
				}
			}

			ZBuffer.SaveBuffer("TableBuffer", newBuffer);
			ZBuffer.WriteBuffer("TableBuffer", x, y);
		}

		public static void	HighlightCell(int left, int top, int right, int bottom, Color foreColor, Color backColor)
		{
			var attribute = new ZCharAttribute(foreColor, backColor);
			ZOutput.FillCharAttribute(left, top, attribute, right-left+1);

			for (var i = 1; i < bottom-top; i++)
			{
				ZOutput.FillCharAttribute(left,  top + i, attribute);
				ZOutput.FillCharAttribute(right, top + i, attribute);
			}
			ZOutput.FillCharAttribute(left, bottom, attribute, right-left+1);
		}

		public static void	HighlightCell(Rect rect, Color foreColor, Color backColor)
		{
			HighlightCell(rect.Left, rect.Top, rect.Right, rect.Bottom, foreColor, backColor);
		}
		
		private static void putFrameIntoBuffer(Node[,] buffer, Rect dimensions, FrameBorders borders, ZCharAttribute? borderColors, ZCharAttribute? fillColors)
		{
			currentBuffer = buffer;
			
			fillColors = fillColors ?? new ZCharAttribute();
			var node = new Node{ Colors = fillColors.Value, NodeEnumItem = NodeEnum.None };
			for (var i = dimensions.Top+1; i < dimensions.Bottom; i++)
				for (var j = dimensions.Left+1; j < dimensions.Right; j++)
					buffer[i, j] = node;

			applyBorders(    SidesEnum.Down | SidesEnum.Right, dimensions.Left,  dimensions.Top, borders.LeftBorder,  borders.TopBorder, borderColors);
			for (var i = dimensions.Left + 1; i < dimensions.Right; i++)
				applyBorders(SidesEnum.Left | SidesEnum.Right, i,                dimensions.Top, borders.LeftBorder,  borders.TopBorder, borderColors);
			applyBorders(    SidesEnum.Down | SidesEnum.Left,  dimensions.Right, dimensions.Top, borders.RightBorder, borders.TopBorder, borderColors);

			for (var i = dimensions.Top + 1; i < dimensions.Bottom; i++)
			{
				applyBorders(SidesEnum.Up | SidesEnum.Down, dimensions.Left,  i, borders.LeftBorder,  borders.TopBorder, borderColors);
				applyBorders(SidesEnum.Up | SidesEnum.Down, dimensions.Right, i, borders.RightBorder, borders.TopBorder, borderColors);
			}

			applyBorders(    SidesEnum.Up   | SidesEnum.Right, dimensions.Left,  dimensions.Bottom, borders.LeftBorder,  borders.BottomBorder, borderColors);
			for (var i = dimensions.Left + 1; i < dimensions.Right; i++)
				applyBorders(SidesEnum.Left | SidesEnum.Right, i,                dimensions.Bottom, borders.LeftBorder,  borders.BottomBorder, borderColors);
			applyBorders(    SidesEnum.Up   | SidesEnum.Left,  dimensions.Right, dimensions.Bottom, borders.RightBorder, borders.BottomBorder, borderColors);
		}

		private static void applyBorders(SidesEnum sides, int xPos, int yPos, FrameType verticalBorder, FrameType horizontalBorder, ZCharAttribute? colors)
		{
			var currentResult = currentBuffer[yPos, xPos];
			var currentNode = currentResult.NodeEnumItem;

			var resultLeft	= Math.Max((int)(currentNode & (NodeEnum.LeftSingle  | NodeEnum.LeftDouble)),  (int)(sides & SidesEnum.Left)  * (int)horizontalBorder);
			var resultRight = Math.Max((int)(currentNode & (NodeEnum.RightSingle | NodeEnum.RightDouble)), (int)(sides & SidesEnum.Right) * (int)horizontalBorder);
			var resultUp    = Math.Max((int)(currentNode & (NodeEnum.UpSingle    | NodeEnum.UpDouble)),    (int)(sides & SidesEnum.Up)    * (int)verticalBorder);
			var resultDown	= Math.Max((int)(currentNode & (NodeEnum.DownSingle  | NodeEnum.DownDouble)),  (int)(sides & SidesEnum.Down)  * (int)verticalBorder);

			currentResult.NodeEnumItem = (NodeEnum)(resultLeft | resultRight | resultUp | resultDown);
			currentResult.Colors = colors.HasValue ? colors.Value : currentResult.Colors;
			currentBuffer[yPos, xPos] = currentResult;
		}


		[Flags]
		internal enum	NodeEnum
		{
			None		= 0,
			UpSingle	= 1,
			UpDouble	= 2,
			DownSingle	= 4,
			DownDouble	= 8,
			LeftSingle	= 16,
			LeftDouble	= 32,
			RightSingle	= 64,
			RightDouble	= 128,
		}

		[Flags]
		internal enum	SidesEnum
		{
			None	= 0,
			Up		= 1,
			Down	= 4,
			Left	= 16,
			Right	= 64,
		}

		internal struct Node
		{
			internal NodeEnum		NodeEnumItem;
			internal ZCharAttribute Colors;
		}


		public class Table
		{
			public Size				Dimensions		{ get; set; }
			public FrameBorders		Borders			{ get; set; }
			public ZCharAttribute?	BorderColors	{ get; set; }
			public ZCharAttribute?	FillColors		{ get; set; }
			public Cell[]			Cells			{ get; set; }
			public string			Caption			{ get; set; }
			public bool				UseCellBordersForOuterBorder { get; set; }

			internal void validate()
			{
				foreach (var cell in Cells)
				{
					cell.BorderColors	= cell.BorderColors ?? BorderColors;
					cell.FillColors		= cell.FillColors	?? FillColors;
					cell.Dimensions.Right  = Math.Min(cell.Dimensions.Right,  Dimensions.Width - 1);
					cell.Dimensions.Bottom = Math.Min(cell.Dimensions.Bottom, Dimensions.Height - 1);
				}
			}

			public Table(int width, int height)
			{
				Dimensions	= new Size(width, height);
				Borders	= new FrameBorders();
				Cells	= new Cell[0];
			}
		}

		public class Cell
		{
			public Rect				Dimensions		{ get; set; }
			public FrameBorders		Borders			{ get; set; }
			public ZCharAttribute?	BorderColors	{ get; set; }
			public ZCharAttribute?	FillColors		{ get; set; }
			public string			Caption			{ get; set; }


			public Cell(int left, int top, int right, int bottom)
			{
				Dimensions	= new Rect(left, top, right, bottom);
				Borders		= new FrameBorders();
			}

			public Cell(Rect rect)
			{
				Dimensions	= rect;
				Borders		= new FrameBorders();
			}
		}
	
		public class FrameBorders
		{
			public FrameType	TopBorder			{ get; set; }
			public FrameType	BottomBorder		{ get; set; }
			public FrameType	LeftBorder			{ get; set; }
			public FrameType	RightBorder			{ get; set; }

			public FrameBorders() : this(FrameType.Single)
			{
			}

			public FrameBorders(FrameType allBordersType)
			{
				TopBorder = BottomBorder = LeftBorder = RightBorder = allBordersType;
			}
		}
	}	
}

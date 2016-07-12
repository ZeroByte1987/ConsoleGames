namespace ZConsole
{
	public static partial class ZMenu
	{
		public class ColorScheme
		{
			public Color FrameForeColor			{ get; set; }
			public Color FrameBackColor			{ get; set; }
		
			public Color TextForeColor			{ get; set; }
			public Color TextBackColor			{ get; set; }
		
			public Color NumberForeColor		{ get; set; }
			public Color NumberBackColor		{ get; set; }	

			public Color BracketsForeColor		{ get; set; }
			public Color BracketsBackColor		{ get; set; }

			public Color SelectedForeColor		{ get; set; }
			public Color SelectedBackColor		{ get; set; }

			public Color InactiveForeColor		{ get; set; }
			public Color InactiveBackColor		{ get; set; }

			public Color CaptionForeColor		{ get; set; }
			public Color CaptionBackColor		{ get; set; }
			

			public ColorScheme Copy()
			{
				return new ColorScheme
				{
					TextBackColor		= TextBackColor,
					FrameBackColor		= FrameBackColor,
					NumberBackColor		= NumberBackColor,
					BracketsBackColor	= BracketsBackColor,
					FrameForeColor		= FrameForeColor,
					TextForeColor		= TextForeColor,
					NumberForeColor		= NumberForeColor,
					BracketsForeColor	= BracketsForeColor,
					SelectedBackColor	= SelectedBackColor,
					SelectedForeColor	= SelectedForeColor,
					InactiveBackColor	= InactiveBackColor,
					InactiveForeColor	= InactiveForeColor,
					CaptionForeColor	= CaptionForeColor,
					CaptionBackColor	= CaptionBackColor
				};
			}

			public ColorScheme()
			{
				TextBackColor		= Color.Black;
				FrameBackColor		= Color.Black;
				NumberBackColor		= Color.Black;
				BracketsBackColor	= Color.Black;

				FrameForeColor		= Color.Yellow;
				TextForeColor		= Color.White;
				NumberForeColor		= Color.Cyan;
				BracketsForeColor	= Color.Gray;
				
				SelectedBackColor	= Color.DarkMagenta;
				SelectedForeColor	= Color.Yellow;

				InactiveBackColor	= Color.Black;
				InactiveForeColor	= Color.DarkGray;
				
				CaptionForeColor	= Color.Magenta;
				CaptionBackColor	= Color.Black;
			}
		}
	}
}
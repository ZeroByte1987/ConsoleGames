namespace ZConsole
{
	public static partial class ZFrame
	{
		public class	ColorScheme
		{
			public Color FrameForeColor			{ get; set; }
			public Color FrameBackColor			{ get; set; }
		
			public Color CaptionForeColor		{ get; set; }
			public Color CaptionBackColor		{ get; set; }

			public ColorScheme()
			{
				FrameBackColor		= Color.Black;
				FrameForeColor		= Color.Yellow;
				CaptionBackColor	= Color.Black;
				CaptionForeColor	= Color.Magenta;
			}
		}		
	}
}
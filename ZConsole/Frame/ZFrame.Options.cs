namespace ZConsole
{
	public static partial class ZFrame
	{
		public class	Options
		{
			private FrameType	_frameType;
			private int			_width;
			private int			_height;

			public Options()
			{
				ColorScheme		= defaultColorScheme;
				FrameType		= FrameType.Single;
				Width			= 0;
				Height			= 0;
				Caption			= null;
				IsFilled		= false;
			}


			public string		Caption		{ get; set; }
			public ColorScheme	ColorScheme	{ get; set; }
			public bool			IsFilled	{ get; set; }

			public int			Width		{	get { return _width;	}	set { _width  = (value < 5) ? 5 : value;	}}
			public int			Height		{	get { return _height;	}	set { _height = (value < 3) ? 3 : value;	}}
			public FrameType	FrameType	{	get { return _frameType; }	set { _frameType = _validate(value);		}}
		}
	}
}
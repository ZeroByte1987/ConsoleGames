namespace ZConsole
{
	public static class ZFrame
	{
		#region Private Consts and Default Values

		private static readonly string[] frameCharSets = new []
			{
				"        ",
				"┌─┐││└─┘",
				"╔═╗║║╚═╝",
				"┌─┐││└─┘"
			};

		public static readonly ColorScheme	DefaultColorScheme = new ColorScheme();

		#endregion

		#region =====  Public Methods   =====

		public static void		DrawFrame(int x, int y, int width, int height)
		{
			_drawFrame(x, y, width, height, FrameType.Single, false, null);
		}

		public static void		DrawFrame(int x, int y, Options options, int frameWidth = 0, int frameHeight = 0)
		{
			var colorScheme = options.ColorScheme;
			ZColors.SetAndStoreColors(colorScheme.FrameForeColor, colorScheme.FrameBackColor);

			frameWidth	= frameWidth  != 0 ? frameWidth  : options.Width;
			frameHeight	= frameHeight != 0 ? frameHeight : options.Height;
			_drawFrame(x, y, frameWidth, frameHeight, options.FrameType, options.IsFilled, options.Caption, colorScheme.CaptionForeColor, colorScheme.CaptionBackColor);
			
			ZColors.RestoreColors();
		}

		#endregion

		#region =====  Private Methods  =====

		private static void		_drawFrame(int x, int y, int width, int height, FrameType frameType, bool isFilled, string caption, Color captionColor = Color.White, Color captionBackColor = Color.Black)
		{
			var charSet = frameCharSets[(int) _validate(frameType)];

			ZOutput.Print(x, y, charSet[0].ToString().PadRight(width-1, charSet[1]) + charSet[2]);
			
			var fill = charSet[3].ToString().PadRight(width - 1, ' ') + charSet[4];
			for (var i = 1; i < height-1; i++)
			{
				if (isFilled)
				{
					ZOutput.Print(x, y+i, fill);
				}
				else
				{
					ZOutput.Print(x, y+i, charSet[3]);
					ZOutput.Print(x+width-1, y+i, charSet[4]);
				}
			}

			ZOutput.Print(x, y+height-1, charSet[5].ToString().PadRight(width-1, charSet[6]) + charSet[7]);

			if (!string.IsNullOrEmpty(caption))
			{
				ZOutput.Print(x + 2, y, " " + caption + " ", captionColor, captionBackColor);
			}
		}

		private static FrameType _validate(FrameType frameType)
		{
			return (frameType < 0) ? 0 : (frameType > FrameType.Custom) ? FrameType.Custom : frameType;
		}

		#endregion

		#region Option SubClasses and Enums

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

		public class	Options
		{
			#region Private Fields and Constructor

			private FrameType	_frameType;
			private int			_width;
			private int			_height;

			public Options()
			{
				ColorScheme		= DefaultColorScheme;
				FrameType		= FrameType.Single;
				Width			= 0;
				Height			= 0;
				Caption			= null;
				IsFilled		= false;
			}

			#endregion		

			#region Public Properties

			public string		Caption { get; set; }

			public int			Width
			{
				get { return _width; }
				set { _width = (value < 5) ? 5 : value; }
			}

			public int			Height
			{
				get { return _height; }
				set { _height = (value < 3) ? 3 : value; }
			}

			public ColorScheme	ColorScheme	{ get; set; }

			public FrameType	FrameType
			{
				get { return _frameType; }
				set { _frameType = _validate(value); }
			}

			public bool			IsFilled { get; set; }

			#endregion
		}

		#endregion
	}
}
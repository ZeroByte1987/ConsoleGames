namespace ZConsole
{
	public static partial class ZFrame
	{
		private static readonly string[] frameCharSets = new []
			{
				"        ",
				"┌─┐││└─┘",
				"╔═╗║║╚═╝",
				"┌─┐││└─┘"
			};

		private static readonly ColorScheme	defaultColorScheme = new ColorScheme();



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
	}
}
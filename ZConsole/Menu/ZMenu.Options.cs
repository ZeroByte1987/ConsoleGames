namespace ZConsole
{
	public static partial class ZMenu
	{
		public class	Options
		{
			#region Public Properties

			public ZFrame.Options	FrameOptions			{ get; set; }
			public ColorScheme		ColorScheme				{ get; set; }

			public MenuMode			Mode
			{
				get { return mode; }
				set { mode = validate(value); }
			}

			public int ItemSpacing
			{
				get { return itemSpacing; }
				set { itemSpacing = (value < 0) ? 0 : (value > 4) ? 4 : value; }
			}

			public int FrameSpacingHorizontal
			{
				get { return frameSpacingHorizontal; }
				set { frameSpacingHorizontal = (value < 0) ? 0 : (value > 4) ? 4 : value; }
			}

			public int FrameSpacingVertical
			{
				get { return frameSpacingVertical; }
				set { frameSpacingVertical = (value < 0) ? 0 : (value > 4) ? 4 : value; }
			}

			public string Brackets
			{
				get { return brackets; }
				set { brackets = value.Length > 1 ? value.Substring(0, 2) : value + " "; }
			}

			public bool				ForceDimensions			{ get; set; }
			public bool				UseSelectedForeColor	{ get; set; }
			public bool				UseSelectedBackColor	{ get; set; }
			public bool				UseSelectedCaps			{ get; set; }
			public Size				WindowSize				{ get; set; }

			/// <summary>
			/// Gets or sets whether all child menus will have the same style as the parent menu.
			/// </summary>
			public bool				IsMonoStyleColor		{ get; set; }
			
			#endregion

			#region Private Fields and Constructor

			private MenuMode	mode;
			private int			itemSpacing;
			private int			frameSpacingHorizontal;
			private int			frameSpacingVertical;
			private string		brackets;

			public Options()
			{
				Mode				= MenuMode.ShortkeysAndArrows;
				ColorScheme			= DefaultColorScheme;
				ItemSpacing			= 0;
				FrameSpacingHorizontal = 0;
				FrameSpacingVertical = 0;
				Brackets			= "[]";
				UseSelectedForeColor = true;
				UseSelectedBackColor = false;
				UseSelectedCaps		 = false;
				ForceDimensions		 = false;
				FrameOptions = new ZFrame.Options();
			}

			#endregion			
		}
	}
}
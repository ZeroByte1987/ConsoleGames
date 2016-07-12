namespace ZConsole
{
	using System;
	using System.Collections.Generic;
	using ZLinq;


	public static partial class ZMenu
	{
		public class	MenuItem
		{
			public string		Caption			{ get; set; }
			public bool			IsActive		{ get; set; }
			public Options		Options			{ get; set; }
			public MenuItemList	ChildMenuItems	{ get; set; }
			public bool			HasChilds		{ get { return ChildMenuItems.Count > 0; }}
			public MenuItem		Parent			{ get; set; }
			public MenuItemList	ParentList		{ get; set; }

			public int			GetCalculatedWidth { get
			{
				var numberOffset	= Options.Mode != MenuMode.ArrowsOnly ? 4 : 0;
				var frameOptions	= Options.FrameOptions;
				var frameSpacingX	= Options.FrameSpacingHorizontal;
				var maxItemLength	= ChildMenuItems.Max(a => a.Caption.Length);
				
				if (ChildMenuItems.Any(a => a.HasChilds)  &&  frameSpacingX < 2)
				{
					maxItemLength += 2 - frameSpacingX;
				}

				var frameWidth = maxItemLength + 2 + frameSpacingX*2 + numberOffset;

				if (frameOptions.Width  > frameWidth   ||  (Options.ForceDimensions  &&  frameOptions.Width  != 0))
				{
					if (frameOptions.Width < frameWidth)
					{
						if (Options.FrameSpacingHorizontal > 0)
						{
							Options.FrameSpacingHorizontal--;
							return GetCalculatedWidth;
						}
						frameWidth = Math.Max(frameWidth, frameOptions.Width);
					}
				}

				return Math.Max(frameWidth, Caption.Length + 6);
			}}

			public int			GetCalculatedHeight { get
			{
				var frameOptions	= Options.FrameOptions;
				var linesPerItem	= Options.ItemSpacing + 1;
				var frameHeight		= (ChildMenuItems.Count-1) * linesPerItem + 3 + Options.FrameSpacingVertical*2;

				if (frameOptions.Height > frameHeight  ||  (Options.ForceDimensions  &&  frameOptions.Height != 0))
				{
					if (frameOptions.Height < frameHeight)
					{
						if (Options.FrameSpacingVertical > 0)
						{
							Options.FrameSpacingVertical--;
							return GetCalculatedHeight;
						}
						if (Options.ItemSpacing > 0)
						{
							Options.ItemSpacing--;
							return GetCalculatedHeight;
						}
						frameHeight = ChildMenuItems.Count + 2;
					}
				}

				return Math.Max(frameHeight, frameOptions.Height);
			}}

			public int			GetYPosForMenuItem(int yMenuPosition, int itemIndex)
			{ 
				return yMenuPosition + 1 + Options.FrameSpacingVertical + itemIndex*(Options.ItemSpacing+1); 
			}

			internal void		_validate()
			{
				ChildMenuItems.Parent = this;

				foreach (var menuItem in ChildMenuItems)
				{
					menuItem.Parent = this;
					menuItem.ParentList = ChildMenuItems;
					menuItem._validate();

					if (Options.IsMonoStyleColor)
					{
						menuItem.Options.IsMonoStyleColor = true;
						menuItem.Options.ColorScheme = Options.ColorScheme.Copy();
						menuItem.Options.UseSelectedBackColor = Options.UseSelectedBackColor;
						menuItem.Options.UseSelectedForeColor = Options.UseSelectedForeColor;
						menuItem.Options.UseSelectedCaps	  = Options.UseSelectedCaps;
					}
				}
			}

			public override string ToString()
			{
				return string.Format("{0}, {1}", Caption, HasChilds ? ChildMenuItems.Count + " childs" : "no childs");
			}

			public MenuItem()
			{
				Caption		= string.Empty;
				IsActive	= true;
				ChildMenuItems	= new MenuItemList();
				Options		= new Options();
			}

			public MenuItem(string caption) : this()
			{
				Caption = caption;
			}
		}


		public class	MenuItemList : List<MenuItem>
		{
			public MenuItem		Parent			{ get; set; }
			public int			CurrentPosition { get; set; }

			internal void validateItems()
			{
				foreach (var menuItem in this)
				{
					menuItem._validate();
				}
			}

			public MenuItemList()
			{
			}

			public MenuItemList(int currentPosition)
			{
				CurrentPosition = currentPosition;
			}
		}
	}
}
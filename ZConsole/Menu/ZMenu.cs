namespace ZConsole
{
	using System;
	using System.Collections.Generic;
	using ZLinq;


	public static partial class ZMenu
	{
		#region Private Fields

		private static readonly Stack<MenuWithCoords>		menuStack = new Stack<MenuWithCoords>();
		private static	Dictionary<ConsoleKey, MenuAction>	hotKeyValues;
		
		private static MenuItem currentMenu
		{
			get { return menuStack.Peek().Menu; }
		}

		private static int		currentMenuPosition
		{
			get { return currentMenu.ChildMenuItems.CurrentPosition; }
			set { currentMenu.ChildMenuItems.CurrentPosition = value; }
		}

		#endregion

		#region Public - Default menu options & color scheme

		public static readonly	ColorScheme		DefaultColorScheme = new ColorScheme();
		public static readonly	Options			DefaultMenuOptions = new Options();

		public delegate void	MenuAction();

		#endregion

		#region Public Methods

		public static MenuResult	GetMenuResult(int x, int y, MenuItem menuItem, Dictionary<ConsoleKey, MenuAction> hotKeys = null)
		{
			hotKeyValues = hotKeys;
			return getMenuCycleResult(x, y, menuItem, true);
		}

		#endregion

		#region Private Methods

		private static MenuResult	getMenuCycleResult(int x, int y, MenuItem menuItem, bool isNewMenu)
		{
			x = Math.Min(x, ZConsoleMain.WindowSize.Width  - menuItem.GetCalculatedWidth);
			y = Math.Min(y, ZConsoleMain.WindowSize.Height - menuItem.GetCalculatedHeight);
			menuStack.Push(new MenuWithCoords(x, y, menuItem));

			var resultIndex = getMenuResult(x, y, menuItem, isNewMenu);
			var menuItemList = menuItem.ChildMenuItems;
			
			if (resultIndex >= 0)
			{
				var item = menuItemList[resultIndex];
				if (!item.HasChilds)
				{
					while (menuStack.Count > 0)
					{
						var cMenu = menuStack.Pop();
						hideMenu(cMenu.X, cMenu.Y, cMenu.Menu);
					}
					return new MenuResult(resultIndex, item.Caption);
				}
				
				return getMenuCycleResult(x + menuItem.GetCalculatedWidth, menuItem.GetYPosForMenuItem(y, resultIndex), item, true);
			}
			
			if (resultIndex == -1)
			{
				hideMenu(x, y, menuItem);
				menuStack.Pop();
				if (menuItem.Parent != null)
				{
					var parentMenu = menuStack.Pop();
					return getMenuCycleResult(parentMenu.X, parentMenu.Y, parentMenu.Menu, false);
				}
				return new MenuResult(resultIndex, string.Empty);
			}

			return null;
		}

		private static int			getMenuResult(int x, int y, MenuItem menuItem, bool isNewMenu)
		{
			#region Prepare Variables

			menuItem._validate();
			var menuItems		= menuItem.ChildMenuItems;
			var options			= menuItem.Options;
			var menuItemsText	= menuItems.Select(a => a.Caption).ToList();
			var maxItemLength	= menuItemsText.Max(a => a.Length);

			var useArrows		= options.Mode != MenuMode.ShortkeysOnly;
			var useShortkeys	= options.Mode != MenuMode.ArrowsOnly;
			var colorScheme		= options.ColorScheme;			
			
			#endregion

			#region Draw menu if necessary

			if (isNewMenu)
			{
				if (currentMenuPosition == 0)
				{
					currentMenuPosition = -1;
					for (var i = 0; i < menuItems.Count; i++)
					{
						if (menuItems[i].IsActive)
						{
							currentMenuPosition = i;
							break;
						}
					}
				}
				
				if (currentMenuPosition == -1)
					return -1;

				drawMenu(x, y, menuItem);
			}

			var linesPerItem = options.ItemSpacing+1;
			var xPos = x + 1 + options.FrameSpacingHorizontal + (options.Mode != MenuMode.ArrowsOnly ? 4 : 0);
			#endregion

			#region	Get action result

			while (true)
			{
				#region Highlight current choice if needed

				var yPos = y + 1 + options.FrameSpacingVertical + currentMenuPosition*linesPerItem;
				if (useArrows)
				{
					var currentMenuItemText = options.UseSelectedCaps ? menuItemsText[currentMenuPosition].ToUpper() : menuItemsText[currentMenuPosition];
					ZOutput.Print(
						xPos, yPos, 
						currentMenuItemText.PadRight(maxItemLength, ' '),
						options.UseSelectedForeColor ? colorScheme.SelectedForeColor :  colorScheme.TextForeColor,
						options.UseSelectedBackColor ? colorScheme.SelectedBackColor :  colorScheme.TextBackColor);
				}

				#endregion

				#region Escape key and any additional hotkeys

				var key = Console.ReadKey(true).Key;
				if (key == ConsoleKey.Escape  ||  key == ConsoleKey.Backspace)
					return -1;

				if (hotKeyValues != null  &&  hotKeyValues.ContainsKey(key))
				{
					hotKeyValues[key]();
				}

				#endregion

				#region Short Keys logic
				
				if (useShortkeys  &&  key >= ConsoleKey.D1  &&  key <= (ConsoleKey)(menuItems.Count+48))
				{
					var val = (int)key - 49;
					if (menuItems[val].IsActive)
						return val;
				}

				#endregion

				#region Arrow Control logic

				if (useArrows)
				{
					var oldPosition = currentMenuPosition;

					switch (key)
					{
						case ConsoleKey.DownArrow:
							do
							{
								currentMenuPosition = (currentMenuPosition < menuItems.Count-1) ? currentMenuPosition + 1 : 0;	
							} while (!menuItems[currentMenuPosition].IsActive);
							break;

						case ConsoleKey.UpArrow:
							do
							{
								currentMenuPosition = (currentMenuPosition > 0) ? currentMenuPosition - 1 : menuItems.Count-1;
							} while (!menuItems[currentMenuPosition].IsActive);
							break;

						case ConsoleKey.RightArrow:
							if (menuItems[currentMenuPosition].HasChilds)
								return currentMenuPosition;
							break;

						case ConsoleKey.LeftArrow:
							if (menuItem.Parent != null)
								return -1;
							break;

						case ConsoleKey.Enter:
						case ConsoleKey.Spacebar:
							return currentMenuPosition;
					}

					if (oldPosition != currentMenuPosition)
					{
						ZOutput.Print(xPos, yPos, menuItemsText[oldPosition].PadRight(maxItemLength, ' '), colorScheme.TextForeColor, colorScheme.TextBackColor);
					}
				}

				#endregion
			}

			#endregion
		}

		private static void			drawMenu(int x, int y, MenuItem menuItem)
		{
			#region	Get all values

			var menuItems		= menuItem.ChildMenuItems;
			var options			= menuItem.Options;
			var frameOptions	= options.FrameOptions;
			var colorScheme		= options.ColorScheme ?? DefaultColorScheme;

			frameOptions.ColorScheme.FrameForeColor   = colorScheme.FrameForeColor;
			frameOptions.ColorScheme.FrameForeColor   = colorScheme.FrameForeColor;
			frameOptions.ColorScheme.CaptionForeColor = colorScheme.CaptionForeColor;
			frameOptions.ColorScheme.CaptionBackColor = colorScheme.CaptionBackColor;
			
			#endregion

			#region	Draw the frame

			var frameHeight		= menuItem.GetCalculatedHeight;
			var frameWidth		= menuItem.GetCalculatedWidth;
			ZBuffer.ReadBuffer("Menu_" + menuItem.Caption, x, y, frameWidth, frameHeight);
			frameOptions.Caption = menuItem.Caption;
			frameOptions.IsFilled = true;
			ZFrame.DrawFrame(x, y, frameOptions, frameWidth, frameHeight);
			
			var frameSpacingX	= options.FrameSpacingHorizontal;
			var frameSpacingY	= options.FrameSpacingVertical;
			var linerPerItem	= options.ItemSpacing + 1;
			var numberOffset	= options.Mode != MenuMode.ArrowsOnly ? 4 : 0;

			#endregion

			#region	Draw menu items

			for (var i = 0; i < menuItems.Count; i++)
			{
				var item = menuItems[i];
				var xPos = x + 1 + frameSpacingX;
				var yPos = y + 1 + frameSpacingY + i*linerPerItem;
				if (options.Mode != MenuMode.ArrowsOnly)
				{
					var itemNumber = (i + 1).ToString();
					ZOutput.Print(xPos,     yPos, options.Brackets[0] + " " + options.Brackets[1], colorScheme.BracketsForeColor,	colorScheme.BracketsBackColor);
					ZOutput.Print(xPos + 1, yPos, itemNumber, colorScheme.NumberForeColor, colorScheme.NumberBackColor);
				}
				
				ZOutput.Print(xPos + numberOffset, yPos, item.Caption,
					item.IsActive ? colorScheme.TextForeColor : colorScheme.InactiveForeColor,
					item.IsActive ? colorScheme.TextBackColor : colorScheme.InactiveBackColor);

				if (item.HasChilds)
				{
					ZOutput.Print(x + frameWidth - 2, yPos, ">", 
						item.IsActive ? colorScheme.NumberForeColor : colorScheme.InactiveForeColor,
						item.IsActive ? colorScheme.NumberBackColor : colorScheme.InactiveBackColor);
				}
			}

			#endregion
		}

		private static void			hideMenu(int x, int y, MenuItem menuItem)
		{
			ZBuffer.WriteBuffer("Menu_" + menuItem.Caption, x, y);
		}

		private static MenuMode		validate (MenuMode mode)
		{
			return (mode < 0) ? 0 : (mode > MenuMode.Custom) ? MenuMode.Custom : mode;
		}

		#endregion

		#region Option SubClasses and Enums

		public class	MenuWithCoords
		{
			public MenuItem Menu	{ get; set; }
			public int		X		{ get; set; }
			public int		Y		{ get; set; }

			public MenuWithCoords(int x, int y, MenuItem menuItem)
			{
				X = x;
				Y = y;
				Menu = menuItem;
			}
		}

		public class	MenuResult
		{
			public string	Text	{ get; set; }
			public int		Index	{ get; set; }

			public MenuResult(int index, string text)
			{
				Index = index;
				Text = text;
			}
		}

		public enum		MenuMode
		{
			ShortkeysAndArrows,
			ShortkeysOnly,
			ArrowsOnly,
			Custom
		}
		
		#endregion
	}
}
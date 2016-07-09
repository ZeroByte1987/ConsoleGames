namespace ZFrontier.Logic.UI
{
	using System;
	using Objects.GameData;
	using Objects.Planet;
	using Objects.Units;
	using Windows;
	using ZConsole;


	public static class ShopLogic
	{
		#region Private Fields

		private static PlayerModel		Player			{	get	{	return ZFrontier.Player;		}}
		private static ActionPanel		ActionPanel		{	get	{	return ZFrontier.ActionPanel;	}}
		private static PlayerStats		PlayerStats		{	get	{	return ZFrontier.PlayerStats;	}}
		private static EventLog			EventLog		{	get	{	return ZFrontier.EventLog;		}}

		private const Color		InactiveItemColor		= Color.DarkGray;
		private const Color		ItemColor				= Color.White;
		private const Color		SelectedItemColor		= Color.Yellow;
		private const Color		ItemBackColor			= Color.Black;
		private const Color		SelectedItemBackColor	= Color.DarkMagenta;
		
		#endregion


		public delegate bool	BuyItem(ShopItem item);
		public delegate bool	SellItem(ShopItem item);


		private static void		PrintItem(int x, int y, Shop shop, ShopItem item, bool isBBC = false, bool isSelected = false)
		{
			if (item.IsNotItem)
			{
				ZOutput.PrintBB(x, y, item.Name, item.IsActive ? ItemColor : InactiveItemColor, SelectedItemColor, Color.DarkGray, isSelected ? SelectedItemBackColor : ItemBackColor);
				return;
			}

			if (isBBC)
			{
				var advert = (Advert) item.ItemObject;
				ZOutput.PrintBB(x, y, string.Format(item.Name, Enums.Get_Name(advert.Merchandise), advert.Price).PadRight(shop.Header.Length, ' '),
					item.IsActive ? ItemColor : InactiveItemColor, SelectedItemColor, Color.DarkGray, isSelected ? SelectedItemBackColor : ItemBackColor);
			}
			else
			{
				ZOutput.Print(x, y, item.Name.PadRight(shop.Header.Length-4, ' '), 
					isSelected ? SelectedItemColor : item.IsActive ? ItemColor : InactiveItemColor, 
					isSelected ? SelectedItemBackColor : ItemBackColor);
			
				if (item.IsActive)
					ZIOX.Draw_Currency(x + shop.Header.Length-5, y, item.Price, 4, false);
				else
					ZOutput.Print(x + shop.Header.Length-2, y, "--", InactiveItemColor);
			}
		}


		public static void		DoShopping(int x, int y, Shop shop, BuyItem buyMethod, SellItem sellMethod)
		{
			#region Draw list of items

			ActionPanel.ClearArea();
			var exitFlag = false;
			var current = 0;

			if (shop.DescriptionItems != null)
				foreach (var item in shop.DescriptionItems)
				{
					var caption = item.Split('\t')[0];
					var text = item.Split('\t')[1];
					shop.Add(new ShopItem(caption, 0, true, text, true));
				}

			if (shop.HasQuit)
			{
				shop.Add(new ShopItem(ActionPanel.Common_Quit, 0, true, null, true));
			}

			while (!shop[current].IsActive)
				current++;
			var oldPosition = current;
			var goodsX = x + 3;
			var goodsY = y + 2;
			var lastIndex = shop.Count - 1;
			var isBBC = shop.Type == ShopType.BBC;

			ZFrame.DrawFrame(x, y, new ZFrame.Options{ Caption = shop.Name, Width = shop.Header.Length+6, Height = lastIndex+4, FrameType = FrameType.Double});
			ZOutput.Print(goodsX, y+1, shop.Header, Color.Cyan, Color.Black);
			for (var i = 0; i < shop.Count; i++)
				PrintItem(goodsX, goodsY + i, shop, shop[i], isBBC);
			
			ZOutput.Print(x + (shop.Header.Length+6-shop.Footer.Length)/2, goodsY+shop.Count+1, shop.Footer, Color.Red, Color.Black);

			#endregion

			while (!exitFlag)
			{
				var item = shop[current];
				PrintItem(goodsX, goodsY + current, shop, item, isBBC, true);
				var key = ZInput.ReadKey();

				switch (key)
				{
					case ConsoleKey.UpArrow	:	do {	current = (current > 0) ? current - 1 : lastIndex;	}	while (!shop[current].IsActive);	break;
					case ConsoleKey.DownArrow:	do {	current = (current < lastIndex) ? current + 1 : 0;	}	while (!shop[current].IsActive);	break;
		
					case ConsoleKey.Enter	:
					case ConsoleKey.PageUp	:
					case ConsoleKey.RightArrow:
					case ConsoleKey.Add:
					case ConsoleKey.Insert:
						#region Buy Item

						if (item.Name == ActionPanel.Common_Quit)	{	exitFlag = true;	break;	}
						if (item.ItemObject is string)				{	EventLog.Print(item.ItemObject as string);	break;	}

						if (shop.Type != ShopType.BBC)
						{
							if (Player.Credits >= item.Price)
							{
								exitFlag = buyMethod(item) ? shop.ExitAfter : exitFlag;
							}
							else
							{
								EventLog.Print("Planet_NoMoneyToBuy");
							}
						}
						else
						{
							buyMethod(item);							
						}
						PlayerStats.Draw_PlayerStats();
						break;

						#endregion
					
					case ConsoleKey.PageDown:	
					case ConsoleKey.LeftArrow:
					case ConsoleKey.Subtract:
					case ConsoleKey.Delete:
						#region Sell Item

						if (item.Name == ActionPanel.Common_Quit)	{	exitFlag = true;	break;	}
						if (item.ItemObject is string)				{	EventLog.Print(item.ItemObject as string);	break;	}

						var result = (sellMethod != null) ? sellMethod(item) : buyMethod(item);
						exitFlag = result && shop.ExitAfter;
						PlayerStats.Draw_PlayerStats();
						break;

						#endregion

					case ConsoleKey.Escape:
					case ConsoleKey.Backspace: exitFlag = true;	break;

					case ConsoleKey.F1		:	HelpInfo.Show();		break;
					case ConsoleKey.F5		:	PlayerInfo.Show();		break;
					case ConsoleKey.F6		:	GalaxyInfo.Show();		break;
					case ConsoleKey.F10		:	ZFrontier.Quit_Game();	break;
				}

				while (!shop[current].IsActive)
					current = (current < lastIndex) ? current + 1 : 0;

				if (current != oldPosition)
				{
					PrintItem(goodsX, goodsY + oldPosition, shop, shop[oldPosition], isBBC);
					oldPosition = current;
				}
			}
			ActionPanel.ClearArea();
		}
	}
}

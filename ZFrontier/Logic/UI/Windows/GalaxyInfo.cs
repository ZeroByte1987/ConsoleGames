namespace ZFrontier.Logic.UI.Windows
{
	using System;
	using Objects.GameData;
	using ZConsole;
	using ZLinq;


	public static class GalaxyInfo
	{
		#region Private Fields

		private static TranslationSet	Lang		{	get {	return GameConfig.Lang;				}}
		private static readonly Rect	TableRect	= new Rect(11,  10, 112, 38);

		private const Color	BackColor	= Color.DarkBlue;
		private const Color	HeaderColor	= Color.Yellow;

		#endregion
		
		
		public static void		Show()
		{
			ZBuffer.ReadBuffer("Window", TableRect);
			DrawTable();
			DrawStarSystemsInfo();
			ZBuffer.WriteBuffer("Window", TableRect.Left, TableRect.Top);
			ZColors.SetBackColor(Color.Black);
		}
		

		private static void		DrawTable()
		{
			ZTable.DrawTable(TableRect.Left, TableRect.Top, new ZTable.Table(TableRect.Width, GameConfig.CurrentGalaxySizeX*GameConfig.CurrentGalaxySizeY + 4)
				{
					Caption = "Table",
					Borders = new ZTable.FrameBorders(FrameType.Double),
					BorderColors = new ZCharAttribute(Color.Cyan, BackColor),
					FillColors = new ZCharAttribute(Color.Cyan, BackColor), 
				});
		}


		private static void		DrawStarSystemsInfo()
		{
			var top  = TableRect.Top  + 3;
			var left = TableRect.Left + 2;
			ZColors.SetBackColor(BackColor);
			ZOutput.Print(left, top-2, Lang["GalaxyInfo_Header"], HeaderColor);

			var exploredSystemsUnsorted = ZFrontier.Galaxy.Get_AllSystems().Where(a => a.IsExplored).ToArray();
			var exploredSystems = exploredSystemsUnsorted;
			var exitFlag = false;
			var filterChanged = true;
			var oldKey = ConsoleKey.Spacebar;
			while (exitFlag == false)
			{
				if (filterChanged)
				{
					#region Draw systems info

					ZColors.SetColor(Color.White);
					for (var i = 0; i < exploredSystems.Length; i++)
					{
						ZColors.SetBackColor(i % 2 != 0 ? Color.DarkBlue : Color.DarkCyan);
						ZOutput.Print(left - 1, top + i, "".PadRight(TableRect.Width - 2, ' '));

						var system = exploredSystems[i];
						var eventName = Enums.Get_Name(system.CurrentEvent);

						ZOutput.Print(left, top + i, system.Name);
						ZOutput.Print(left + 12, top + i, Enums.Get_Name(system.Allegiance));
						ZOutput.Print(left + 27, top + i, system.TechLevel);
						ZOutput.Print(left + 37 - (eventName.Length / 2), top + i, eventName);
						ZOutput.Print(left + 45, top + i, system.CurrentEvent == GlobalEventType.Normal || system.CurrentEvent == GlobalEventType.AlienInvasion ? "  ---"
							: system.EventDuration + " " + Lang["Statistics_Days"]);

						var offset = left + 56;
						foreach (var goods in system.IllegalGoods)
						{
							ZOutput.Print(offset, top + i, Enums.Get_Name(goods));
							offset += 10;
						}
					}

					#endregion
				}

				#region Sorting

				var key = ZInput.ReadKey();
				filterChanged = key != oldKey;
				oldKey = key;
				switch (key)
				{
					case ConsoleKey.N	:	exploredSystems = exploredSystemsUnsorted.OrderBy(a => a.Name).ToArray();			break;
					case ConsoleKey.A	:	exploredSystems = exploredSystemsUnsorted.OrderBy(a => a.Allegiance).ToArray();		break;
					case ConsoleKey.L	:	exploredSystems = exploredSystemsUnsorted.OrderBy(a => a.TechLevel).ToArray();		break;
					case ConsoleKey.S	:	exploredSystems = exploredSystemsUnsorted.OrderBy(a => a.CurrentEvent).ToArray();	break;
					case ConsoleKey.T	:	exploredSystems = exploredSystemsUnsorted.OrderBy(a => a.EventDuration).ToArray();	break;
					case ConsoleKey.F	:	exploredSystems = exploredSystemsUnsorted.OrderBy(a => a.IllegalGoods.Count).ToArray();	break;
					case ConsoleKey.Escape:	exitFlag = true;		break;
					default:	filterChanged = false;				break;
				}

				#endregion
			}
		}
	}
}

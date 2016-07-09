namespace ZFrontier.Logic.UI.Common
{
	using System;
	using Objects.GameData;
	using ZConsole;


	public static class CommonMethods
	{
		#region Area Methods

		private static AreaUI	currentActiveArea = AreaUI.GalaxyMap;

		private static Rect		eventArea		{	get { return ZFrontier.EventArea;		}}
		private static Rect		galaxyArea		{	get { return ZFrontier.GalaxyArea;		}}
		private static Rect		battleArea		{	get { return ZFrontier.ActionArea;		}}
		private static Rect		statsArea		{	get { return ZFrontier.PlayerStatsArea;	}}


		public static void		HighlightArea(AreaUI area, bool mode = true)
		{
			switch (currentActiveArea)
			{
				case AreaUI.EventLog	:	ZTable.HighlightCell(eventArea,	  	Color.Cyan, Color.Black);	break;
				case AreaUI.GalaxyMap	:	ZTable.HighlightCell(galaxyArea,	Color.Cyan, Color.Black);	break;
				case AreaUI.ActionPanel	:	ZTable.HighlightCell(battleArea,	Color.Cyan, Color.Black);	break;
				case AreaUI.PlayerStats	:	ZTable.HighlightCell(statsArea,		Color.Cyan, Color.Black);	break;
			}

			currentActiveArea = area;
			switch (area)
			{
				case AreaUI.EventLog	:	ZTable.HighlightCell(eventArea,  mode ? Color.Yellow : Color.Cyan, Color.Black);	break;
				case AreaUI.GalaxyMap	:	ZTable.HighlightCell(galaxyArea, mode ? Color.Yellow : Color.Cyan, Color.Black);	break;
				case AreaUI.ActionPanel	:	ZTable.HighlightCell(battleArea, mode ? Color.Yellow : Color.Cyan, Color.Black);	break;
				case AreaUI.PlayerStats	:	ZTable.HighlightCell(statsArea,  mode ? Color.Yellow : Color.Cyan, Color.Black);	break;
			}
		}

		public static void		HideArea(AreaUI area)
		{
			ClearArea(area, '▒');
		}

		public static void		ClearArea(AreaUI area, char fillChar = ' ')
		{
			switch (area)
			{
				case AreaUI.EventLog	:	ZOutput.FillRect(eventArea.Left+1,  eventArea.Top+1,  eventArea.Width-2,  eventArea.Height-2,  fillChar);	break;
				case AreaUI.GalaxyMap	:	ZOutput.FillRect(galaxyArea.Left+1, galaxyArea.Top+1, galaxyArea.Width-2, galaxyArea.Height-2, fillChar);	break;
				case AreaUI.ActionPanel	:	ZOutput.FillRect(battleArea.Left+1, battleArea.Top+1, battleArea.Width-2, battleArea.Height-2, fillChar);	break;
			}
		}

		#endregion

		#region Stats Output Methods

		public delegate void	DrawComplexValue(int x, int y, int value1, int value2);
		public delegate void	DrawSingleValue(int x, int y, int value);
		public static void		Draw_StatDescr(StatsArea area, int statIndex, string statName)
		{
			ZOutput.Print(area.Left, area.Top+statIndex, statName, statIndex % 2 == 0 ? Color.Green : Color.Magenta);
			ZOutput.Print(area.Left+statName.Length, area.Top+statIndex, ":", Color.DarkGray);
		}
		public static void		Draw_Stat(StatsArea area, int statIndex, string statName, int statValue)
		{
			Draw_Stat(area, statIndex, statName, statValue.ToString());
		}
		public static void		Draw_Stat(StatsArea area, int statIndex, string statName, string statValue)
		{
			Draw_StatDescr(area, statIndex, statName);
			ZOutput.Print(area.ValueLeft, area.Top+statIndex, statValue.PadRight(area.ValueWidth, ' '), Color.White);
		}
		public static void		Draw_Stat(StatsArea area, int statIndex, string statName, DrawComplexValue drawMethod, int value1, int value2)
		{
			Draw_StatDescr(area, statIndex, statName);
			drawMethod(area.ValueLeft, area.Top+statIndex, value1, value2);
		}
		public static void		Draw_Stat(StatsArea area, int statIndex, string statName, DrawSingleValue drawMethod, int value)
		{
			Draw_StatDescr(area, statIndex, statName);
			drawMethod(area.ValueLeft, area.Top+statIndex, value);
		}
		public static void		Draw_Merchandise(StatsArea area, int statIndex, string goodsName, int value)
		{
			ZOutput.Print(area.Left, area.Top+statIndex, goodsName, Color.DarkGreen);
			if (value > 0)	ZIOX.Draw_Mass(area.ValueLeft, area.Top+statIndex, value);
			else			ZOutput.Print(area.ValueLeft, area.Top+statIndex, "--", Color.DarkGray);
		}

		public static void		Draw_Date(StatsArea area, int statIndex, string statName, DateTime value)
		{
			Draw_StatDescr(area, statIndex, statName);
			ZIOX.Draw_Date(area.ValueLeft-2, area.Top+statIndex, value);
		}

		#endregion
	}
}
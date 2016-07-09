namespace ASCII_Tactics.Models.Items
{
	using System.Collections.Generic;
	using Logic;
	using ZConsole;
	using ZLinq;


	public class Inventory : List<Item>
	{
		public int		CurrentItemId	{ get; set; }
		public Item 	CurrentItem		{ get { return this[CurrentItemId]; }}

		public void		Draw(StatsArea area, int statIndex)
		{
			ZIOX.Draw_Stat(area, 11, "Inventory", ZIOX.Draw_Mass, this.Sum(a => a.Weight) / 10);
			for (var i = 0; i < Count; i++)
			{
				ZIOX.Draw_Item(area, 12+i, this[i].Name, false);
			}
		}
	}
}

namespace ASCII_Tactics.Models.Items
{
	using System.Collections.Generic;
	using ZLinq;


	public class Inventory : List<Item>
	{
		public Item		ActiveItem		{ get; set; }
		public int		TotalWeight		{ get { return this.Sum(w => w.Type.Weight); } }


		public Inventory(IEnumerable<Item> items)
		{
			foreach (var item in items)
			{
				this.Add(item);
			}
		}
	}
}
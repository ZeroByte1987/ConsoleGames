namespace ASCII_Tactics.Models.Items
{
	using System.Collections.Generic;

	public class ItemTypes : Dictionary<string, Item>
	{
		public void		Add(Item itemType)
		{
			if (!this.ContainsKey(itemType.Name))
			{
				itemType.ItemTypeId = this.Count+1;
				this.Add(itemType.Name, itemType);
			}
		}


		public Item		GetByKey(string itemName)
		{
			return ContainsKey(itemName) ? this[itemName] : null;
		}
	}
}

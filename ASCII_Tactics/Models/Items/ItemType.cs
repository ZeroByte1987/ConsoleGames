namespace ASCII_Tactics.Models.Items
{
	using CommonEnums;


	public class ItemType
	{
		public string		Name		{ get; set; }
		public ItemClass	Class		{ get; set; }
		public int			Weight		{ get; set; }
		public int			Price		{ get; set; }


		public ItemType(string name, ItemClass itemClass, int weight, int price = 0)
		{
			Name = name;
			Class = itemClass;
			Weight = weight;
			Price = price;
		}
	}
}
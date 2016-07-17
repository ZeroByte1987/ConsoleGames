namespace ASCII_Tactics.Models.Map
{
	using System.Collections.Generic;
	using ZConsole;


	public class Room
	{
		public Rect				Area		{ get; set; }
		public List<Furniture>	Furnitures	{ get; set; }

		public Room(Rect area)
		{
			Area = area;
			Furnitures = new List<Furniture>();
		}
	}
}
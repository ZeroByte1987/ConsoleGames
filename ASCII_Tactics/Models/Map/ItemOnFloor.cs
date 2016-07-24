namespace ASCII_Tactics.Models.Map
{
	using Items;
	using ZConsole;
	

	public class ItemOnFloor : ActiveObject
	{
		public Item		Item		{ get; set; }
		public bool		IsTrap		{ get; set; }


		public ItemOnFloor(int levelId, Coord coord, Item item, bool isTrap = false) : base(levelId, coord)
		{
			Item	= item;
		}
	}
}
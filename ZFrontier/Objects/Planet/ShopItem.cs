namespace ZFrontier.Objects.Planet
{
	using System.Collections.Generic;
	using GameData;


	public class ShopItem
	{
		#region Public Properties

		public string	Name		{ get; set; }
		public int		Price		{ get; set; }
		public bool		IsActive	{ get; set; }
		public object	ItemObject	{ get; set; }
		public bool		IsNotItem	{ get; set; }

		#endregion

		#region Constructor
		
		public ShopItem(string name, int price, bool isActive = true, object itemObject = null, bool isNotItem = false)
		{
			Name		= name;
			Price		= price;
			IsActive	= isActive;
			ItemObject	= itemObject;
			IsNotItem	= isNotItem;
		}

		#endregion
	}


	public class Shop : List<ShopItem>
	{
		#region Public Properties

		public string		Name		{ get; set; }
		public string		Header		{ get; set; }
		public string		Footer		{ get; set; }
		public bool			ExitAfter	{ get; set; }
		public ShopType		Type		{ get; set; }
		public bool			HasQuit		{ get; set; }
		public string[]		DescriptionItems	{ get; set; }

		#endregion

		#region Constructor
		
		public Shop(string name, IEnumerable<ShopItem> items, ShopType shopType = ShopType.Shop, string header = null, string footer = null, bool exitAfter = false, bool hasQuit = false, string[] descriptionItems = null) : base(items)
		{
			Name	= name;
			Header	= header ?? GameConfig.Lang["Planet_DefaultHeader"];
			Footer	= footer ?? GameConfig.Lang["Planet_DefaultFooter"];
			Type	= shopType;
			ExitAfter = exitAfter;
			HasQuit = hasQuit;
			DescriptionItems = descriptionItems;
		}

		#endregion
	}


	public enum ShopType
	{
		Shop,
		Shipyard,
		BBC
	}
}

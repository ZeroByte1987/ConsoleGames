namespace ASCII_Tactics.Models.Items
{
	using CommonEnums;
	using Config;
	using Logic.Extensions;


	public class Item 
	{
		public ItemType Type	{ get; set; }
		public int		Value	{ get; set; }


		public Item(ItemType type)
		{
			Type = type;

			if (type.Class == ItemClass.Weapon)
			{
				Value = type.AsWeapon().AmmoCapacity;
			}
				
			if (type.Class == ItemClass.Ammo)
			{
				var weapon = ItemConfig.ItemTypes[type.AsAmmo().WeaponName];
				Value = weapon.AsWeapon().AmmoCapacity;
			}
				
		}

		public Item(string itemName) : this(ItemConfig.ItemTypes[itemName])
		{
		}
	}
}
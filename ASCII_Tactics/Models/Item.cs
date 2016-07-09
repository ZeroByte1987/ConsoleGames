namespace ASCII_Tactics.Models
{
	using Items;


	public class Item
	{
		public int		ItemTypeId	{ get; set; }
		public int		Id			{ get; set; }
		public string	Name		{ get; set; }
		public int		Weight		{ get; set; }

		public bool		IsWeapon	{ get; set; }


		public Item(string name, int weight)
		{
			Name = name;
			Weight = weight;
		}

		public Item Copy()
		{
			var ammo = this as Ammo;
			if (ammo != null)
			{
				return new Ammo(Name, Weight, ammo.WeaponTypeId, ammo.Amount) { ItemTypeId = ItemTypeId, Id = Id };
			}

			var weapon = this as Weapon;
			if (weapon != null)
			{
				return new Weapon(Name, Weight, weapon.DamageMin, weapon.DamageMax, weapon.Range, weapon.MaxAmmo, weapon.Time_Reload)
				{ ItemTypeId = ItemTypeId, Id = Id };
			}

			return new Item(Name, Weight) { ItemTypeId = ItemTypeId, Id = Id };
		}
	}
}

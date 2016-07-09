namespace ASCII_Tactics.Models.Items
{
	using Logic;


	public class Ammo : Item
	{
		public int WeaponTypeId	{ get; set; }

		public int Amount		{ get; set; }


		public Ammo(string name, int weight, int weaponTypeId, int amount) : base(name, weight)
		{
			WeaponTypeId = weaponTypeId;
			Amount = amount;
		}

		public Ammo(string name, int weight, string weaponTypeName, int amount) : base(name, weight)
		{
			var weaponType = MainGame.ItemTypes.GetByKey(weaponTypeName);
			if (weaponType != null)
				WeaponTypeId = weaponType.ItemTypeId;
			Amount = amount;
		}
	}
}

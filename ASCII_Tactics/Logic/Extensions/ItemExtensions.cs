namespace ASCII_Tactics.Logic.Extensions
{
	using Models;
	using Models.Items;


	public static class ItemExtensions
	{
		public static Ammo		AsAmmo(this Item item)
		{
			return item as Ammo;
		}

		public static Weapon	AsWeapon(this Item item)
		{
			return item as Weapon;
		}
	}
}

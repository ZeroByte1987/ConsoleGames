namespace ASCII_Tactics.Logic.Extensions
{
	using Models.Items;


	public static class ItemExtensions
	{
		public static Ammo		AsAmmo(this ItemType item)
		{
			return item as Ammo;
		}

		public static Weapon	AsWeapon(this ItemType item)
		{
			return item as Weapon;
		}
	}
}
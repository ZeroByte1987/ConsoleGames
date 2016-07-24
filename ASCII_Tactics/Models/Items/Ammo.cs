namespace ASCII_Tactics.Models.Items
{
	using CommonEnums;


	public class Ammo : ItemType
	{
		public string	WeaponName	{ get; set; }


		public Ammo(string name, string weaponName, int weight, int price = 0) : base(name, ItemClass.Ammo, weight, price)
		{
			WeaponName = weaponName;
		}
	}
}
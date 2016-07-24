namespace ASCII_Tactics.Models.Items
{
	using CommonEnums;
	using ZConsole;


	public class Weapon : ItemType
	{
		public Range	Damage			{ get; set; }
		public int		Range			{ get; set; }
		public int		AmmoCapacity	{ get; set; }
		public int		Time_Reload		{ get; set; }
		public int		Time_SnapShot	{ get; set; }
		public int		Time_AimShot	{ get; set; }
		public int		Time_AutoShot	{ get; set; }


		public Weapon(string name, int weight, int damageMin, int damageMax, int range, int maxAmmo, int timeReload, int price = 0) 
			: base(name, ItemClass.Weapon, weight, price)
		{
			Damage = new Range(damageMin, damageMax);
			Range = range;
			AmmoCapacity = maxAmmo;
			Time_Reload = timeReload;
		}
	}
}
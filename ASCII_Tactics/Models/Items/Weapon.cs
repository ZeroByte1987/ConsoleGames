namespace ASCII_Tactics.Models.Items
{
	public class Weapon : Item
	{
		public int DamageMin		{ get; set; }
		public int DamageMax		{ get; set; }
		public int Range			{ get; set; }

		public int MaxAmmo			{ get; set; }
		public int CurrentAmmo		{ get; set; }

		public int Time_SnapShot	{ get; set; }
		public int Time_AimShot		{ get; set; }
		public int Time_AutoShot	{ get; set; }
		public int Time_Reload		{ get; set; }



		public Weapon(string name, int weight, int damageMin, int damageMax, int range, int maxAmmo, int timeReload) 
			: base(name, weight)
		{
			DamageMin = damageMin;
			DamageMax = damageMax;
			Range = range;
			MaxAmmo = maxAmmo;
			Time_Reload = timeReload;
			IsWeapon = true;
		}

		public new Weapon Copy()
		{
			return new Weapon(Name, Weight, DamageMin, DamageMax, Range, MaxAmmo, Time_Reload)
				    {
						ItemTypeId = this.ItemTypeId,
						Id = this.Id,
						CurrentAmmo = this.CurrentAmmo,
						Time_SnapShot = this.Time_SnapShot,
						Time_AimShot =  this.Time_AimShot,
						Time_AutoShot = this.Time_AutoShot
				    };
		}
	}
}

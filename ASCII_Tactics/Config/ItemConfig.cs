namespace ASCII_Tactics.Config
{
	using System.Collections.Generic;
	using Models.Items;


	public static class ItemConfig
	{
		public static Dictionary<string, ItemType>	ItemTypes = new Dictionary<string, ItemType>
		{
			{ "Pistol",			new Weapon("Pistol",	15, 14, 22, 15, 12, 25)	{ Time_SnapShot = 30, Time_AimShot = 50, Time_AutoShot =  0 }},
			{ "Rifle",			new Weapon("Rifle",		45, 20, 30, 30, 20, 40)	{ Time_SnapShot = 40, Time_AimShot = 60, Time_AutoShot = 70 }},
			{ "Shotgun",		new Weapon("Shotgun",	45, 25, 40, 18, 10, 50)	{ Time_SnapShot = 40, Time_AimShot = 60, Time_AutoShot =  0 }},
			
			{ "Pistol Ammo",	new Ammo(  "Pistol Ammo",		"Pistol",		 3)},
			{ "Rifle Ammo",		new Ammo(  "Rifle Ammo",		"Rifle",		10)},
			{ "Shotgun Ammo",	new Ammo(  "Shotgun Ammo",		"Shotgun",		 8)},
		};


		public static string[][]	DefaultInventories = new []
		{
			new [] { "Rifle",		"Rifle Ammo",	"Rifle Ammo"	},
			new [] { "Rifle",		"Rifle Ammo",	"Rifle Ammo" 	},
			new [] { "Shotgun",		"Shotgun Ammo", "Shotgun Ammo" 	},
			new [] { "Pistol",		"Pistol Ammo",	"Pistol Ammo"	}
		};
	}
}
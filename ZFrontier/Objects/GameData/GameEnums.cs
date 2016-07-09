namespace ZFrontier.Objects.GameData
{
	using System;
	using System.Collections.Generic;
	using Logic;
	using ZLinq;


	#region Enums

	public enum Difficulty
	{
		Easy,
		Normal,
		Hard
	}
	public enum GalaxySize
	{
		Small,
		Normal,
		Big
	}
	
	public enum NPC_Type
	{
		Pirate,
		Trader,
		Police,
		Assassin,
		Alien,
		Guard
	}
	public enum FlightEvent
	{
		Pirate,
		Trader,
		Police,
		Assassin,
		Alien,
		Asteroid
	}

	public enum Merchandise
	{
		Food,
		Minerals,
		Medicine,
		Robots,
		Luxury,
		Drugs,
		Weapons,
		Slaves,
	}
	public enum Equipment
	{
		Missile,
		LaserUnit,
		Shield,
		ECM,
		Scanner,
		MiningLaser,
		EscapeBoat
	}

	public enum EquipmentState
	{
		No,
		Yes,
		Damaged
	}
	
	public enum Allegiance
	{
		Independent		= 0,
		Alliance		= 1,
		Empire			= 2
	}
	public enum GlobalEventType
	{
		Normal			= 0,
		Epidemy			= 1, 
		Starvation		= 2,
		CivilWar		= 3, 
		AlienInvasion	= 4,
		LevelUp			= 5,
		LevelDown		= 6,
		IllegalAdd		= 7,
		IllegalRemove	= 8
	}
	
	public enum LegalStatus
	{
		Clean		= 0,
		Offender	= 1, 
		Criminal	= 10,
		Terrorist	= 30
	}

	public enum CombatRating
	{
		Harmless		= 0,
		MostlyHarmless	= 20, 
		Poor			= 60,
		BelowAverage	= 120,
		Average			= 240,
		AboveAverage	= 400,
		Competent		= 700,
		Dangerous		= 1000,
		Deadly			= 1500,
		Elite			= 2000
	}

	public enum Reputation
	{
		Terrible		= 0,
		Bad				= 10,
		Neutral			= 20, 
		Good			= 30,
		Excellent		= 40
	}

	public enum MilitaryRank
	{
		None			= 0,
		Private			= 1,
		Corporal		= 10,
		Sergeant		= 30,
		Lieutenant		= 50,
		Captain			= 80,
		Major			= 120,
		Colonel			= 160,
		Commodore		= 220,
		Admiral			= 300
	}

	public enum MissionType
	{
		PackageDelivery,
		GoodsDelivery,
		Passenger,
		Assassination,
		Military_Delivery,
		Military_Assassination
	}

	public enum AdvertType
	{
		BuyIllegal,
		PackageDelivery,
		GoodsDelivery,
		Passenger,
		Assassination,
		Military_Delivery,
		Military_Assassination
	}

	public enum AreaUI
	{
		EventLog,
		GalaxyMap,
		ActionPanel,
		PlayerStats
	}

	public enum AlienStrength
	{
		BeforeFather,
		FatherIsHere,
		FatherIsKilled
	}
	
	#endregion
	

	public static class Enums
	{
		#region List of all enum values for Main Enums

		public static List<Difficulty>		All_Difficulties		{ get; set; }
		public static List<GalaxySize>		All_GalaxySizes			{ get; set; }
		public static List<Merchandise>		All_Merchandise			{ get; set; }
		public static List<Merchandise>		All_MerchandiseLegal	{ get; set; }
		public static List<Merchandise>		All_MerchandiseIllegal	{ get; set; }
		public static List<Equipment>		All_Equipment			{ get; set; }
		public static List<Allegiance>		All_Allegiances			{ get; set; }
		public static List<GlobalEventType>	All_GlobalEvents		{ get; set; }
		public static List<GlobalEventType>	All_GlobalEventsWithDuration { get; set; }
		public static List<FlightEvent>		All_FlightEvents		{ get; set; }
		public static List<NPC_Type>		All_NPC_Types			{ get; set; }
		public static List<AdvertType>		All_BBCAdvertTypes		{ get; set; }
		public static List<AdvertType>		All_MilitaryAdvertTypes	{ get; set; }
		
		#endregion

		public static string		Get_Name<T>(T value)
		{
			var localizationPrefix =  Get_LocalizationPrefix<T>();
			return GameConfig.Lang.Single(a => a.Key == localizationPrefix + value).Value;
		}
		public static T				Get_Random<T>(this List<T> list)
		{
			return list[RNG.GetNumber(list.Count)];
		}
		public static List<T>		Get_AllEnumValues<T>(T enumType)
		{
			return Enum.GetValues(typeof(T)).Cast<T>().ToList();
		}
		public static T				Get_Value<T>(string name)
		{
			var localizationPrefix =  Get_LocalizationPrefix<T>();
			var itemName = GameConfig.Lang.Single(a => a.Value == name).Key.Substring(localizationPrefix.Length);
			return (T) Enum.Parse(typeof (T), itemName);
		}
		public static T				Get_Value<T>(int value)
		{
			var valueList = Enum.GetValues(typeof (T)).Cast<int>().ToList();
			return (T) (object) valueList.Where(a => value >= a).Max();
		}
	
		public static Merchandise	Get_MerchandiseForEvent(this GlobalEventType @event)
		{
			return @event == GlobalEventType.Epidemy	? Merchandise.Medicine
				:  @event == GlobalEventType.Starvation ? Merchandise.Food
				:  @event == GlobalEventType.CivilWar	? Merchandise.Weapons
				:  Merchandise.Food;
		}

		private static string		Get_LocalizationPrefix<T>()
		{
			return    typeof(T) == typeof (GlobalEventType)	? "GlobalEventName_"
					: typeof(T) == typeof (FlightEvent)		? "FlightEventName_"
					: typeof(T) == typeof (NPC_Type)		? "NPC_"
					: typeof(T).Name + "_";
		}

		public static string		Get_Name(MilitaryRank rank, Allegiance allegiance)
		{
			return GameConfig.Lang.Single(a => a.Key == "MilitaryRank_" + allegiance + "_" + rank).Value;
		}
	}
}
namespace ZFrontier.Objects.GameData
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Text;
	using Logic;
	using Logic.Events;
	using Logic.UI;
	using Units;
	using Units.PlayerData;
	using ZConsole;
	using ZConsole.Common;
	using ZLinq;


	public static class GameConfig
	{
		#region Data Paths

		private const string DataPath_LanguagesDirectory	= @"Data\Resources";
		private const string DataPath_Main					= "Main";
		private const string DataPath_Help					= "Help";
		private const string DataPath_Intro					= "Intro";
		private const string DataPath_NPC_Names				= "Names_NPC";
		private const string DataPath_Alien_Names			= "Names_Aliens";
		private const string DataPath_Star_Names			= "Names_Stars";
		private const string DataPath_Config				= @"Data\Config.txt";
		private const string DataPath_Prices				= @"Data\Prices.txt";
		public static string SaveGamesDirectoty				= "Saves";

		#endregion

		#region Private Fields - Names

		private static Dictionary<string, List<string>>	npcNames;
		private static Dictionary<string, List<string>> alienNames;
		private static Dictionary<string, List<string>> starNames;
		private static List<string> unusedStarNames;

		#endregion

		#region Structures - Languages, Players, Ships, Prices
		
		public static int				MaxSaveGamesCount = 10;
		public static string			CurrentLanguageName;
		public static LanguageSet		Languages;
		public static TranslationSet	Lang;

		public static Dictionary<string, PropertyList>		HelpSets;
		public static Dictionary<string, List<string>>		IntroSets;
		public static Dictionary<int, GoodsPriceSet>		GoodsPrices;
		public static Dictionary<int, EquipPriceSet>		EquipmentPrices;
		public static List<ShipModel>						ShipModels;
		public static Dictionary<Difficulty, PlayerModel>	DefaultPlayers;

		#endregion

		#region Planet Data

		public static Range TechLevelRange = new Range(1, 6);

		public static int	ShipInShopChance		= 3;
		public static int	AdvertCount				= 4;
		public static int	AdvertTrapChance		= 1;
		public static int	AdvertIllegalPriceBonus = 5;
		public static int	Fine_IllegalGoods		= 3;
		public static int	MaxPriceChange			= 1;
		public static int	GoodIsIllegalChance		= 3;
		public static int	GlobalEventPriceBonus	= 5;
		public static int	IndependentIllegalBonus	= 1;

		public static int	BuyMap_OptionChance		= 2;
		public static int	BuyMap_SystemCount		= 2;
		public static int	BuyMap_Price			= 20;
		
		#endregion

		#region Galaxy Data  and  Global Data

		public static Range		GalaxySizeRangeX	= new Range(2, 5);
		public static Range		GalaxySizeRangeY	= new Range(2, 6);
		public static Range		EventCount			= new Range(1, 5);
		public static Difficulty CurrentDifficulty	= Difficulty.Normal;
		public static int		CurrentGalaxySizeX	= 4;
		public static int		CurrentGalaxySizeY	= 4;
		public static bool		GalaxyIsExplored	= false;
		public static int		EventsPerFlight		= 3;

		public static int	GlobalEventsCooldown	= 15;
		public static int	BaseGlobalEventDuration = 25;
		public static int	AlienInvasionShipCount	= 3;
		public static int	AlienInvasionReward		= 50;

		public static Range		AttackRange			= new Range(3, 12);
		public static Range		DefenseRange		= new Range(1, 8);
		public static int		FuelMax				= 50;
		public static int		FuelPrice			= 3;
		public static DateTime	GameStartDate		= new DateTime(2300, 1, 1);
		public static DateTime	AlienAttackDate		= new DateTime(2301, 1, 1);
		public static int		AlienAttackTriggerCount	= 4;

		#endregion

		#region Flight Events Data

		public static int	PirateAttackChance		= 5;
		public static int	PoliceExaminationChance = 3;
		public static int	PoliceFindIllegalChance = 4;
		public static int	MiningFuelCost			= 5;
		public static int	AsteroidRichness		= 4;

		#endregion

		#region Battle Settings

		public static int	EnemyMissileUsage	= 2;
		public static int	MissileAccuracy		= 5;
		public static int	MissileDamage		= 6;
		public static int	ECM_Effeciency		= 4;
		public static int	EnemyPanicLevel		= 3;
		public static int	DropCargoBonus		= 5;
		public static int	MaxDropCargoBonus	= 3;
		public static int	TraderDropChance	= 4;
		public static int	PlayerEscapeChance	= 4;
		public static int	CriticalChanceLaser	= 2;
		public static int	CriticalChanceMissile = 1;

		#endregion

		#region NPC stats modificators

		public static Dictionary<NPC_Type, NPC_StatsConfig> NPC_StatsConfigs = new Dictionary<NPC_Type, NPC_StatsConfig> {
		{	NPC_Type.Pirate,	new NPC_StatsConfig { Bounty = new Range( 0,  5), ShipsUsage = new Range(0, 4), Bonus_Attack = 1, Bonus_Defense = 0, Bonus_ECM = 2, EscapeChance = 4 }},
		{	NPC_Type.Assassin,	new NPC_StatsConfig { Bounty = new Range( 5, 10), ShipsUsage = new Range(2, 5), Bonus_Attack = 3, Bonus_Defense = 1, Bonus_ECM = 5, EscapeChance = 2 }},
		{	NPC_Type.Alien,		new NPC_StatsConfig	{ Bounty = new Range(10, 10), ShipsUsage = new Range(4, 7), Bonus_Attack = 5, Bonus_Defense = 2, Bonus_ECM = 6, EscapeChance = 4 }},
		{	NPC_Type.Trader,	new NPC_StatsConfig	{ Bounty = new Range( 0,  0), ShipsUsage = new Range(2, 7), Bonus_Attack = 1, Bonus_Defense = 0, Bonus_ECM = 2, EscapeChance = 4, FineForAttack = 3 }},
		{	NPC_Type.Police,	new NPC_StatsConfig	{ Bounty = new Range( 0,  0), ShipsUsage = new Range(2, 6), Bonus_Attack = 2, Bonus_Defense = 0, Bonus_ECM = 3, EscapeChance = 3, FineForAttack = 5 }},
		{	NPC_Type.Guard,		new NPC_StatsConfig { Bounty = new Range( 0,  0), ShipsUsage = new Range(3, 5), Bonus_Attack = 3, Bonus_Defense = 1, Bonus_ECM = 4, EscapeChance = 2, FineForAttack = 4 }}};

		#endregion

		#region Mission content

		public enum CombatRating1
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

		public static List<MissionTypeModel> MissionTypes = new List<MissionTypeModel>
		{						//  Type							Difficulty		   Rwd RpN CR Needed						RCh As AS
			new MissionTypeModel(MissionType.PackageDelivery,		Difficulty.Easy,   10,  0, (int)CombatRating.Harmless,		1),
			new MissionTypeModel(MissionType.PackageDelivery,		Difficulty.Normal, 20, 10, (int)CombatRating.Poor,			1,  1, 0),
			new MissionTypeModel(MissionType.PackageDelivery,		Difficulty.Hard,   25, 20, (int)CombatRating.Average,		2,  1, 2),
			new MissionTypeModel(MissionType.GoodsDelivery,			Difficulty.Easy,   17,  0, (int)CombatRating.Harmless,		1),
			new MissionTypeModel(MissionType.GoodsDelivery,			Difficulty.Normal, 25, 10, (int)CombatRating.Poor,			1,  1, 0),
			new MissionTypeModel(MissionType.GoodsDelivery,			Difficulty.Hard,   30, 20, (int)CombatRating.Average,		2,  1, 2),
			new MissionTypeModel(MissionType.Passenger,				Difficulty.Easy,   12, 10, (int)CombatRating.Poor,			1),
			new MissionTypeModel(MissionType.Passenger,				Difficulty.Normal, 30, 20, (int)CombatRating.Average,		2,  1, 1),
			new MissionTypeModel(MissionType.Passenger,				Difficulty.Hard,   40, 30, (int)CombatRating.AboveAverage,	3,  2, 1),
			new MissionTypeModel(MissionType.Assassination,			Difficulty.Easy,   30, 20, (int)CombatRating.Average,		2),
			new MissionTypeModel(MissionType.Assassination,			Difficulty.Normal, 45, 30, (int)CombatRating.AboveAverage,	3,  1, 2),
			new MissionTypeModel(MissionType.Assassination,			Difficulty.Hard,   60, 40, (int)CombatRating.Competent,		4,  2, 2),
								//  Type							Difficulty		   Rwd RpN CR Needed						MR Needed					  RCh MCh  As AS
			new MissionTypeModel(MissionType.Military_Delivery,		Difficulty.Easy,    5,  0, (int)CombatRating.Harmless,		(int)MilitaryRank.None,		  1,  1),
			new MissionTypeModel(MissionType.Military_Delivery,		Difficulty.Normal, 10, 10, (int)CombatRating.Poor,			(int)MilitaryRank.Private,    1,  2,   1, 0),
			new MissionTypeModel(MissionType.Military_Delivery,		Difficulty.Hard,   20, 15, (int)CombatRating.Average,		(int)MilitaryRank.Sergeant,   2,  3,   1, 2),			
			new MissionTypeModel(MissionType.Military_Assassination,Difficulty.Easy,   20, 20, (int)CombatRating.Average,		(int)MilitaryRank.Lieutenant, 2,  3),
			new MissionTypeModel(MissionType.Military_Assassination,Difficulty.Normal, 30, 25, (int)CombatRating.AboveAverage,  (int)MilitaryRank.Captain,	  3,  4,   1, 2),
			new MissionTypeModel(MissionType.Military_Assassination,Difficulty.Hard,   45, 30, (int)CombatRating.Competent,		(int)MilitaryRank.Major,	  4,  5,   2, 2),
		};

	#endregion

		#region Military content

		public static Allegiance[]	FractionsWithNavy = { Allegiance.Alliance, Allegiance.Empire };

		#endregion


		public static void			Initialize()
		{
			Read_Languages();
			Read_Prices();
			Initialize_AllEnumLists();

			CheckFile(DataPath_Config);
			var propertyList = ZConfig.ReadConfig(DataPath_Config);
			try
			{
				Read_GlobalProps(propertyList);
				Read_Ships(propertyList);
				Read_PlayerSettings(propertyList);
				Read_GalaxyProps(propertyList);
				Read_FlightProps(propertyList);
				Read_NPC_Props(propertyList);
				Read_BattleProps(propertyList);
				Read_PlanetaryProps(propertyList);
				Read_StatsRanges(propertyList);
			}
			catch
			{
				ZOutput.ErrorMsgWait("Error! Config file is corrupted or has invalid data.");
			}
			
			Apply_Languages();
			Reset();
		}

		public static void			Reset()
		{
			unusedStarNames = new List<string>(starNames[CurrentLanguageName]);
		}

		public static void			Initialize_AllEnumLists()
		{
			Enums.All_Difficulties			= Enums.Get_AllEnumValues(Difficulty.Normal);
			Enums.All_GalaxySizes			= Enums.Get_AllEnumValues(GalaxySize.Normal);

			Enums.All_Merchandise			= Enums.Get_AllEnumValues(Merchandise.Food);
			Enums.All_MerchandiseLegal		= Enums.All_Merchandise.Where(a => a <= Merchandise.Robots).ToList();
			Enums.All_MerchandiseIllegal	= Enums.All_Merchandise.Where(a => a >  Merchandise.Robots).ToList();
			Enums.All_Equipment				= Enums.Get_AllEnumValues(Equipment.Missile);
			Enums.All_Allegiances			= Enums.Get_AllEnumValues(Allegiance.Alliance);
			Enums.All_GlobalEvents			= Enums.Get_AllEnumValues(GlobalEventType.Normal);
			Enums.All_FlightEvents			= Enums.Get_AllEnumValues(FlightEvent.Trader);
			Enums.All_NPC_Types				= Enums.Get_AllEnumValues(NPC_Type.Trader);
			Enums.All_BBCAdvertTypes		= Enums.Get_AllEnumValues(AdvertType.BuyIllegal).Where(a => a < AdvertType.Military_Delivery).ToList();;
			Enums.All_MilitaryAdvertTypes	= Enums.Get_AllEnumValues(AdvertType.Military_Delivery).Where(a => a >= AdvertType.Military_Delivery).ToList();
			Enums.All_GlobalEventsWithDuration = new List<GlobalEventType> { GlobalEventType.Starvation, GlobalEventType.Epidemy, GlobalEventType.CivilWar };
		}


		#region Public - Get Value Methods

		public static string		Get_NPC_Name(NPC_Type npcType)
		{
			return npcType == NPC_Type.Alien 
				? alienNames[CurrentLanguageName][RNG.GetNumber(alienNames[CurrentLanguageName].Count)] 
				: npcNames[CurrentLanguageName][RNG.GetNumber(npcNames[CurrentLanguageName].Count)];
		}

		public static string		Get_StarSystem_Name()
		{
			var resultName = unusedStarNames[RNG.GetNumber(unusedStarNames.Count)];
			unusedStarNames.Remove(resultName);
			return resultName;
		}

		public static int			Get_MerchPrice(int techLevel, Merchandise merch)
		{
			return GoodsPrices[techLevel][merch];
		}

		public static int			Get_FuelConsumption(int x, int y, PlayerModel player)
		{
			return (Math.Abs(x - player.PosX) + Math.Abs(y - player.PosY) + 1) * player.Ship.FuelConsumption;
		}

		public static int			Get_Distance(int x, int y, PlayerModel player)
		{
			var dx = (x - player.PosX) * 2;
			var dy = (y - player.PosY) * 2;
			var sum = (Math.Pow(dx, 2) + Math.Pow(dy, 2));
			return (int) Math.Ceiling(Math.Sqrt(sum));
		}

		#endregion


		#region Private - Read Sections

		private static void				Read_Languages()
		{
			#region Errors

			if (!Directory.Exists(DataPath_LanguagesDirectory))
				ZOutput.ErrorMsgWait("Error! Cannot find language resources.");

			var languages = Directory.GetDirectories(DataPath_LanguagesDirectory);
			if (languages.Length == 0)
			{
				ZOutput.ErrorMsgWait("Error! Cannot find language resources.");
			}

			#endregion

			Languages	= new LanguageSet();
			HelpSets	= new Dictionary<string, PropertyList>();
			IntroSets	= new Dictionary<string, List<string>>();
			npcNames	= new Dictionary<string, List<string>>();
			alienNames	= new Dictionary<string, List<string>>();
			starNames	= new Dictionary<string, List<string>>();

			foreach (var languagePath in languages)
			{
				var fileNames = Directory.GetFiles(languagePath, "*.txt");
				var languageName = Path.GetFileName(languagePath) ?? "Unknown";

				foreach (var fullFileName in fileNames)
				{
					var codePage = int.Parse(File.ReadAllLines(fullFileName)[0]);
					var encoding = Encoding.GetEncoding(codePage);

					var fileName = Path.GetFileNameWithoutExtension(fullFileName);
					switch (fileName)
					{
						case DataPath_Main:
							#region Main resources

							var resources = File.ReadAllLines(fullFileName, encoding);
							var translations = new TranslationSet();
							foreach (var resource in resources.Where(a => !string.IsNullOrEmpty(a)))
							{
								var tokens = resource.Split(new[] {'\t'}, StringSplitOptions.RemoveEmptyEntries);
								if (tokens.Length > 1 && !translations.ContainsKey(tokens[0]))
									translations.Add(tokens[0], tokens[1].Replace("\\r\\n", "\r\n"));
							}
							Languages.Add(languageName, translations);
							break;

							#endregion

						case DataPath_Help:
							var propertyList = ZConfig.ReadConfig(fullFileName, encoding);
							HelpSets.Add(languageName, propertyList);
							break;

						case DataPath_Intro		:	IntroSets.Add(languageName,  File.ReadAllLines(fullFileName, encoding).Where(a => a.Any(c => !char.IsDigit(c))).ToList());			break;
						case DataPath_NPC_Names	:	npcNames.Add(languageName,   File.ReadAllLines(fullFileName, encoding).Where(a => !string.IsNullOrEmpty(a)  &&  a.Any(c => !char.IsDigit(c))).ToList());	break;
						case DataPath_Alien_Names:	alienNames.Add(languageName, File.ReadAllLines(fullFileName, encoding).Where(a => !string.IsNullOrEmpty(a)  &&  a.Any(c => !char.IsDigit(c))).ToList());	break;
						case DataPath_Star_Names:	starNames.Add(languageName,  File.ReadAllLines(fullFileName, encoding).Where(a => !string.IsNullOrEmpty(a)  &&  a.Any(c => !char.IsDigit(c))).ToList());	break;
					}
				}
			}
		}

		private static void				Read_Prices()
		{
			CheckFile(DataPath_Prices);
			var propertyList = ZConfig.ReadConfig(DataPath_Prices);

			var goods = propertyList["Goods"];
			GoodsPrices = new Dictionary<int, GoodsPriceSet>();
			for (var i = 0; i < goods.Count; i++)
			{
				var levelPrices = new GoodsPriceSet();
				var tokens = GetValueArray(goods[i]);
				for (var j = 0; j < tokens.Length; j++)
					levelPrices.Add((Merchandise)j, int.Parse(tokens[j]));
				GoodsPrices.Add(i+1, levelPrices);
			}

			var equipment = propertyList["Equipment"];
			EquipmentPrices = new Dictionary<int, EquipPriceSet>();
			for (var i = 0; i < equipment.Count; i++)
			{
				var levelPrices = new EquipPriceSet();
				var tokens = GetValueArray(equipment[i]);
				for (var j = 0; j < tokens.Length; j++)
					levelPrices.Add((Equipment)j, int.Parse(tokens[j]));
				EquipmentPrices.Add(i+1, levelPrices);
			}
		}

		private static void				Read_GlobalProps(PropertyList propertyList)
		{
			var props = Get_ConfigProperties(propertyList["GlobalData"]);
			CurrentLanguageName		= props["Language"];
			MaxPriceChange			= int.Parse(props["MaxPriceChange"]);
			GoodIsIllegalChance		= int.Parse(props["GoodIsIllegalChance"]);
			BaseGlobalEventDuration	= int.Parse(props["BaseGlobalEventDuration"]);
			FuelMax					= int.Parse(props["FuelMax"]);
			FuelPrice				= int.Parse(props["FuelPrice"]);
			Lang = Languages[CurrentLanguageName];
		}

		private static void				Read_Ships(PropertyList propertyList)
		{
			ShipModels = new List<ShipModel>();
			var shipProps = Get_ConfigProperties(propertyList["Ships"]);

			foreach (var shipProp in shipProps)
			{
				var tokens = shipProp.Value.Split(',').Select(a => int.Parse(a.Trim())).ToArray();
				ShipModels.Add(new ShipModel
					{
						ModelName		= shipProp.Key,
						MaxHP			= tokens[0],
						MaxMissiles		= tokens[1],
						MaxCargoLoad	= tokens[2],
						FuelConsumption = tokens[3],
						Price			= tokens[4]
					});
			}
		}

		private static void				Read_PlayerSettings(PropertyList propertyList)
		{
			DefaultPlayers = new Dictionary<Difficulty, PlayerModel>
				{
					{ Difficulty.Easy,	 Get_PlayerSettings(propertyList["Player_Easy"])   },
					{ Difficulty.Normal, Get_PlayerSettings(propertyList["Player_Normal"]) },
					{ Difficulty.Hard,   Get_PlayerSettings(propertyList["Player_Hard"])   }
				};
		}

		private static void				Read_GalaxyProps(PropertyList propertyList)
		{
			var props = Get_ConfigProperties(propertyList["Galaxy"]);
			GalaxySizeRangeX	= Range.GetFromString(props["SizeRangeX"]);
			GalaxySizeRangeY	= Range.GetFromString(props["SizeRangeY"]);
			CurrentGalaxySizeX	= int.Parse(props["DefaultSizeX"]);
			CurrentGalaxySizeY	= int.Parse(props["DefaultSizeY"]);
			GalaxyIsExplored	= int.Parse(props["ExploredGalaxy"]) > 0;
		}

		private static void				Read_FlightProps(PropertyList propertyList)
		{
			var props = Get_ConfigProperties(propertyList["Flight_Events"]);
			EventCount				= Range.GetFromString(props["EventCount"]);
			EventsPerFlight			= int.Parse(props["EventsPerFlight"]);
			PoliceExaminationChance	= int.Parse(props["PoliceExaminationChance"]);
			PoliceFindIllegalChance = int.Parse(props["PoliceFindIllegalChance"]);
			MiningFuelCost			= int.Parse(props["MiningFuelCost"]);
			AsteroidRichness		= int.Parse(props["AsteroidRichness"]);
			PirateAttackChance		= int.Parse(props["PirateAttackChance"]);
		}

		private static void				Read_NPC_Props(PropertyList propertyList)
		{
			var props = Get_ConfigProperties(propertyList["NPC_Stats"]);
			foreach (var npcType in Enums.All_NPC_Types)
				NPC_StatsConfigs[npcType] = Get_NPC_Config(props, npcType);
		}

		private static void				Read_BattleProps(PropertyList propertyList)
		{
			var props = Get_ConfigProperties(propertyList["Battle_Settings"]);
			EnemyMissileUsage		= int.Parse(props["EnemyMissileUsage"]);
			MissileAccuracy 		= int.Parse(props["MissileAccuracy"]);
			MissileDamage 			= int.Parse(props["MissileDamage"]);
			ECM_Effeciency 			= int.Parse(props["ECM_Effeciency"]);
			EnemyPanicLevel 		= int.Parse(props["EnemyPanicLevel"]);
			DropCargoBonus			= int.Parse(props["DropCargoBonus"]);
			MaxDropCargoBonus		= int.Parse(props["MaxDropCargoBonus"]);
			TraderDropChance		= int.Parse(props["TraderDropChance"]);
			PlayerEscapeChance		= int.Parse(props["PlayerEscapeChance"]);
			CriticalChanceLaser		= int.Parse(props["CriticalChanceLaser"]);
			CriticalChanceMissile	= int.Parse(props["CriticalChanceMissile"]);
		}

		private static void				Read_StatsRanges(PropertyList propertyList)
		{
			var props = Get_ConfigProperties(propertyList["Stats_Range"]);
			AttackRange		= Range.GetFromString(props["Attack"]);
			DefenseRange	= Range.GetFromString(props["Defense"]);
		}

		private static void				Read_PlanetaryProps(PropertyList propertyList)
		{
			var props = Get_ConfigProperties(propertyList["Planetary_Settings"]);
			ShipInShopChance		= int.Parse(props["ShipInShopChance"]);
			AdvertCount				= int.Parse(props["AdvertCount"]);
			AdvertTrapChance		= int.Parse(props["AdvertTrapChance"]);
			AdvertIllegalPriceBonus	= int.Parse(props["AdvertIllegalPriceBonus"]);
			MaxPriceChange			= int.Parse(props["MaxPriceChange"]);			
			GlobalEventPriceBonus	= int.Parse(props["GlobalEventPriceBonus"]);
			Fine_IllegalGoods		= int.Parse(props["FineIllegalGoods"]);
		}

		public static void				Apply_Languages()
		{
			ActionPanel.Common_Quit					= Lang["Common_Quit"];
			ZIOX.PressAnyKeyMessage					= Lang["Common_PressAnyKey"];

			ActionPanel.FinesPayment_PayAllFines	= Lang["FinesPayment_PayAll"];
			ActionPanel.RepairShop_RepairAll		= Lang["Repair_RepairAll"];
			ActionPanel.RepairShop_RepairSingle		= Lang["Repair_RepairSingle"];
			ActionPanel.Tavern_LatestNews			= Lang["Tavern_LatestNews"];
			ActionPanel.Tavern_Rumors				= Lang["Tavern_Rumors"];
			ActionPanel.Tavern_SpecialServices		= Lang["Tavern_SpecialServices"];

			BattleLogic.Menu_Attack_Laser			= Lang["Battle_Menu_LaserAttack"];
			BattleLogic.Menu_Attack_Missile			= Lang["Battle_Menu_MissileAttack"];
			BattleLogic.Menu_RunAway				= Lang["Battle_Menu_RunAway"];
			BattleLogic.Menu_DropCargoAndRun		= Lang["Battle_Menu_DropCargoAndRun"];
			BattleLogic.Menu_DropAllCargo			= Lang["Battle_Menu_DropAllCargo"];
			BattleLogic.Menu_DropOneCargo			= Lang["Battle_Menu_DropOneCargo"];
			BattleLogic.Menu_ScoopCargo				= Lang["Battle_Menu_ScoopCargo"];

			ActionPanel.MilitaryHQ_About			= Lang["MilitaryHQ_About"];
		}

		#endregion


		#region Read Config Files
		
		private static void				CheckFile(string fileName)
		{
			if (!File.Exists(fileName))
			{
				ZOutput.ErrorMsgWait("Error! File  \\" + fileName + "  not found.");
			}
		}

		private static PlayerModel		Get_PlayerSettings(IEnumerable<string> lines)
		{
			var playerInitStats = Get_ConfigProperties(lines);
			var player = new PlayerModel
				{
					Credits		= int.Parse(playerInitStats["Credits"]),
					Ship 		= ShipModels.Single(a => a.ModelName == playerInitStats["Ship"]),
					Attack		= int.Parse(playerInitStats["Attack"]),
					Defense		= int.Parse(playerInitStats["Defense"]),
					ECM			= int.Parse(playerInitStats["ECM"]) > 0			? EquipmentState.Yes : EquipmentState.No,
					MiningLaser	= int.Parse(playerInitStats["MiningLaser"]) > 0	? EquipmentState.Yes : EquipmentState.No,
					Scanner		= int.Parse(playerInitStats["Scanner"]) > 0		? EquipmentState.Yes : EquipmentState.No,
					EscapeBoat	= int.Parse(playerInitStats["EscapeBoat"]) > 0	? EquipmentState.Yes : EquipmentState.No,
					CombatRating	= int.Parse(playerInitStats["CombatRating"]),
					ReputationRating	= int.Parse(playerInitStats["Reputation"]),
				};

			if (!string.IsNullOrEmpty(playerInitStats["CurrentCargo"]))
			{
				foreach (var cargoToken in playerInitStats["CurrentCargo"].Split(',').Select(int.Parse))
					player.CurrentCargo.Add((Merchandise) cargoToken, 1);
			}

			if (!string.IsNullOrEmpty(playerInitStats["FineAmount"]))
			{
				var fines = playerInitStats["FineAmount"].Split(',').Select(int.Parse).ToArray();
				for (var i = 0; i < Enums.All_Allegiances.Count; i++)
					player.LegalRecords[(Allegiance)i].FineAmount = fines[i];
			}

			if (!string.IsNullOrEmpty(playerInitStats["MilitaryPoints"]))
			{
				var points = playerInitStats["MilitaryPoints"].Split(',').Select(int.Parse).ToArray();
				for (var i = 0; i < points.Length; i++)
					player.MilitaryRanks[FractionsWithNavy[i]].Rating = points[i];
			}

			return player;
		}

		private static NPC_StatsConfig	Get_NPC_Config(Dictionary<string, string> lines, NPC_Type npcType)
		{
			var stats = lines[npcType.ToString()].Split(',').Select(a => a.Trim()).ToArray();
			return new NPC_StatsConfig
				       {
					       Bounty		= Range.GetFromString(stats[0]),
						   ShipsUsage	= Range.GetFromString(stats[1]),
						   Bonus_Attack	= int.Parse(stats[2]),
						   Bonus_Defense= int.Parse(stats[3]),
						   Bonus_ECM	= int.Parse(stats[4]),
						   EscapeChance	= int.Parse(stats[5])
				       };
		}

		private static Dictionary<string, string> Get_ConfigProperties(IEnumerable<string> lines)
		{
			return lines.Select(stat => stat.Split('=')).ToDictionary(tokens => tokens[0].Trim(), tokens => tokens[1].Trim());
		}

		private static string[]			GetValueArray(string line)
		{
			return line.Split('=')[1].Split(',').Select(a => a.Trim()).ToArray();
		}

		#endregion
	}
}
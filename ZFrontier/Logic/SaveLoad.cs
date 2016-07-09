namespace ZFrontier.Logic
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using Objects.Galaxy;
	using Objects.GameData;
	using Objects.Units;
	using Objects.Units.PlayerData;
	using UI;
	using UI.Windows;
	using ZConsole;
	using ZLinq;


	public static class SaveLoad
	{
		#region Private Fields

		private static TranslationSet	Lang			{	get {	return ZFrontier.Lang;			}}
		private static EventLog			EventLog		{	get {	return ZFrontier.EventLog;		}}
		private static GalaxyMap		GalaxyMap		{	get {	return ZFrontier.GalaxyMap;		}}
		private static ActionPanel		ActionPanel		{	get {	return ZFrontier.ActionPanel;	}}
		private static PlayerStats		PlayerStats		{	get {	return ZFrontier.PlayerStats;	}}
		private static PlayerModel		Player			{	get {	return ZFrontier.Player;		}}
		private static GalaxyModel		Galaxy			{	get {	return ZFrontier.Galaxy;		}}

		private const Color menuBackColor = Color.DarkBlue;

		private static readonly ZMenu.Options SaveLoadMenuOptions = new ZMenu.Options
			{
				Mode = ZMenu.MenuMode.ArrowsOnly,
				ItemSpacing = 2,
				FrameSpacingHorizontal = 3,
				FrameSpacingVertical = 2,
				UseSelectedBackColor = true,
				ColorScheme = new ZMenu.ColorScheme{ CaptionBackColor = menuBackColor, TextBackColor = menuBackColor },
				FrameOptions = new ZFrame.Options { FrameType = FrameType.Double, ColorScheme = new ZFrame.ColorScheme { FrameBackColor = menuBackColor }},						
			};

		#endregion


		#region Global Save/Load Methods

		public static ZMenu.MenuItemList	GetSaveGames()
		{
			if (!Directory.Exists(GameConfig.SaveGamesDirectoty))
				Directory.CreateDirectory(GameConfig.SaveGamesDirectoty);
			var fileNames = Directory.GetFiles(GameConfig.SaveGamesDirectoty, "*.sav");
			
			var saveCount = fileNames.Length;
			var saveNames = fileNames.Select(Path.GetFileNameWithoutExtension).ToArray();

			var saveGames = new ZMenu.MenuItemList();
			for (var i = 0; i < Math.Min(saveCount, GameConfig.MaxSaveGamesCount); i++)
			{
				saveGames.Add(new ZMenu.MenuItem(saveNames[i]));
			}

			return saveGames;
		}


		public static void			SaveGameFull()
		{
			#region Get Slot To Save

			EventLog.HighlightArea();
			var saveMenuItems = GetSaveGames();
			if (saveMenuItems.Count < GameConfig.MaxSaveGamesCount)
			{
				saveMenuItems.Insert(0, new ZMenu.MenuItem(Lang["SaveGame_EmptySlot"]));
			}

			var fileName = ZMenu.GetMenuResult(19, 12-saveMenuItems.Count,
				new ZMenu.MenuItem(Lang["SaveGame_ChooseSlot"])
					{
						Options = SaveLoadMenuOptions,
						ChildMenuItems = saveMenuItems
					}).Text;

			if (fileName == "")
			{
				GalaxyMap.HighlightArea();
				return;
			}
			
			#endregion

			#region Read File Name for Empty Slot

			if (fileName == Lang["SaveGame_EmptySlot"])
			{
				ZBuffer.ReadBuffer("SaveGameName", 16, 18, 31, 9);
				ZOutput.FillRect(16, 18, 31, 9, ' ');
				ZFrame.DrawFrame(19, 20, new ZFrame.Options { Caption = Lang["Common_EnterName"],  FrameType = FrameType.Double, Width = 25, Height = 5 });
				ZCursor.SetCursorVisibility(true);
				fileName = ZInput.ReadLine(21, 22, 20, Color.White);
				ZCursor.SetCursorVisibility(false);
				ZBuffer.WriteBuffer("SaveGameName", 16, 18);
				if (string.IsNullOrEmpty(fileName))
					return;				
			}
			else
			{
				if (!MessageBox_YesNo.GetResult("SaveGame_OverwriteConfirmation"))
					return;				
			}
			
			#endregion

			SaveGame(fileName);
			GalaxyMap.HighlightArea();
		}

		public static void			SaveGame(string fileName)
		{
			fileName = Path.GetFullPath(GameConfig.SaveGamesDirectoty + "\\" + fileName + ".sav");
			if (File.Exists(fileName))
				File.Delete(fileName);
			var dateTimeNow = DateTime.Now;

			try
			{
				using (var fileStream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write))
				{
					var writer = new BinaryWriter(fileStream);
					writer.Write(0x4753465A);
					writer.Write(("v" + ZFrontier.ReleaseInfo_Version).ToCharArray());
					writer.Write((byte)(dateTimeNow.Year-2000));
					writer.Write((byte)dateTimeNow.Month);
					writer.Write((byte)dateTimeNow.Day);
					writer.Write((byte)dateTimeNow.Hour);
					writer.Write((byte)dateTimeNow.Minute);
					writer.Write((byte)dateTimeNow.Second);

					Galaxy.Save(writer);
					Player.Save(writer);
					writer.Close();
					EventLog.Print("SaveGame_Saved");
				}
			}
			catch
			{
				EventLog.Print("SaveGame_CannotSave");
			}
		}


		public static void			LoadGameFull()
		{
			#region Get Slot To Load

			EventLog.HighlightArea();
			var saveMenuItems = GetSaveGames();
			if (saveMenuItems.Count == 0)
			{
				EventLog.Print("LoadGame_NoGames");
				return;
			}

			var fileName = ZMenu.GetMenuResult(19, 12-saveMenuItems.Count,
				new ZMenu.MenuItem(Lang["LoadGame_ChooseGame"])
					{
						Options = SaveLoadMenuOptions,
						ChildMenuItems = saveMenuItems
					}).Text;

			if (fileName == "")
			{
				return;
			}

			if (!MessageBox_YesNo.GetResult("LoadGame_Confirmation"))
			{
				return;
			}

			#endregion

			LoadGame(fileName);
			GalaxyMap.HighlightArea();
		}

		public static bool			LoadGame(string fileName)
		{
			fileName = Path.GetFullPath(GameConfig.SaveGamesDirectoty + "\\" + fileName + ".sav");
			try
			{
				GameConfig.Reset();
				using (var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
				{
					var reader = new BinaryReader(fileStream);
					if (reader.ReadInt32() != 0x4753465A)
						throw new Exception();
					reader.ReadChars(6);	//	game version - useless now, can be used for conversion
					reader.ReadBytes(6);	//	DateTime of save - useless now, can be used in future

					Galaxy.Load(reader);
					Player.Load(reader);
					
					GalaxyMap.TargetX = Player.PosX;
					GalaxyMap.TargetY = Player.PosY;
					GalaxyMap.Draw_GalaxyMap();
					EventLog.ClearArea();
					ActionPanel.ClearArea();
					PlayerStats.Draw_PlayerStats();
					EventLog.Print("LoadGame_Loaded");
					reader.Close();
				}
				return true;
			}
			catch
			{
				EventLog.Print("LoadGame_Corrupted");
				return false;
			}
		}

		#endregion


		#region Player - Save/Load Methods

		public static void			Save(this PlayerModel player, BinaryWriter writer)
		{
			writer.WriteField(SaveField.PlayerName,		player.Name);
			writer.WriteField(SaveField.ShipName,		player.Ship.ModelName);
			writer.WriteField(SaveField.PosX,			player.PosX);
            writer.WriteField(SaveField.PosY,			player.PosY);
			writer.WriteField(SaveField.IsLanded,		player.IsLanded);
			writer.WriteField(SaveField.Credits,		player.Credits);
            writer.WriteField(SaveField.FuelLeft,		player.FuelLeft);
			writer.WriteField(SaveField.CurrentHP,		player.CurrentHP);
			writer.WriteField(SaveField.CurrentMissiles, player.CurrentMissiles);
			writer.WriteField(SaveField.Attack,			player.Attack);
			writer.WriteField(SaveField.Defense,		player.Defense);
			writer.WriteField(SaveField.HasECM,			(int)player.ECM);
			writer.WriteField(SaveField.HasScanner,		(int)player.Scanner);
			writer.WriteField(SaveField.HasMiningLaser, (int)player.MiningLaser);
			writer.WriteField(SaveField.HasEscapeBoat,	(int)player.EscapeBoat);
			writer.WriteField(SaveField.CombatRating,	player.CombatRating);
			writer.WriteField(SaveField.Reputation,		player.ReputationRating);
			
			writer.WriteField(SaveField.CurrentCargo, Enums.All_Merchandise.Count);
			foreach (var merch in Enums.All_Merchandise)
				writer.Write((byte)player.CurrentCargo[merch]);					

			writer.WriteField(SaveField.LegalStatus, Enums.All_Allegiances.Count);
			foreach (var allegiance in Enums.All_Allegiances)
				writer.Write((short)player.LegalRecords[allegiance].FineAmount);

			writer.WriteField(SaveField.Missions, player.Missions.Count);
			player.SaveMissions(writer);

			writer.WriteField(SaveField.MilitaryRanks, GameConfig.FractionsWithNavy.Length);
			for (var i = 0; i < GameConfig.FractionsWithNavy.Length; i++)
			{
				writer.Write((byte)GameConfig.FractionsWithNavy[i]);
				writer.Write((short)player.MilitaryRanks[GameConfig.FractionsWithNavy[i]].Rating);
			}
				
			writer.WriteField(SaveField.PlayerDataEnd, 0);
		}


		public static void			Load(this PlayerModel player, BinaryReader reader)
		{
			player.CurrentCargo.Clear();

			var playerDataEnd = false;
			while (!playerDataEnd)
			{
				var field = reader.ReadInt32();
				var fieldType	= (SaveField)(field >> 24);
				var fieldValue	= field & 0x00FFFFFF;

				switch (fieldType)
				{
					case SaveField.PlayerName	:	player.Name = new string(reader.ReadChars(20)).TrimEnd();	break;
					case SaveField.ShipName		:
						var shipName = new string(reader.ReadChars(20)).TrimEnd();
						player.Ship = GameConfig.ShipModels.FirstOrDefault(a => a.ModelName == shipName);
						break;
					case SaveField.PosX			:	player.PosX			= fieldValue;		break;
					case SaveField.PosY			:	player.PosY			= fieldValue;		break;
					case SaveField.IsLanded		:	player.IsLanded		= fieldValue > 0;	break;
					case SaveField.Credits		:	player.Credits		= fieldValue;		break;
					case SaveField.FuelLeft		:	player.FuelLeft		= fieldValue;		break;
					case SaveField.CurrentHP	:	player.CurrentHP	= fieldValue;		break;
					case SaveField.CurrentMissiles:	player.CurrentMissiles = fieldValue;	break;
					case SaveField.Attack		:	player.Attack		= fieldValue;		break;
					case SaveField.Defense		:	player.Defense		= fieldValue;		break;
					case SaveField.HasECM		:	player.ECM			= (EquipmentState)fieldValue;	break;
					case SaveField.HasScanner	:	player.Scanner		= (EquipmentState)fieldValue;	break;
					case SaveField.HasMiningLaser:	player.MiningLaser	= (EquipmentState)fieldValue;	break;
					case SaveField.HasEscapeBoat:	player.EscapeBoat	= (EquipmentState)fieldValue;	break;
					case SaveField.CombatRating	:	player.CombatRating	= fieldValue;	break;
					case SaveField.Reputation	:	player.ReputationRating		= fieldValue;	break;
					case SaveField.CurrentCargo	:	for (var i = 0; i < fieldValue; i++)	player.CurrentCargo[(Merchandise)i] = reader.ReadByte();			break;
					case SaveField.LegalStatus	:	for (var i = 0; i < fieldValue; i++)	player.LegalRecords[(Allegiance)i].FineAmount = reader.ReadInt16();	break;
					case SaveField.Missions		:	player.Missions = LoadMissions(reader, fieldValue);		break;
					case SaveField.MilitaryRanks:	for (var i = 0; i < fieldValue; i++)	player.MilitaryRanks[(Allegiance)reader.ReadByte()].Rating = reader.ReadInt16();	break;

					case SaveField.PlayerDataEnd:	playerDataEnd = true;				break;
				}
			}
		}

		#endregion


		#region Galaxy - Save/Load Methods

		public static void			Save(this GalaxyModel galaxy, BinaryWriter writer)
		{
			var galaxySizeX = galaxy.StarSystems.GetLength(1);
			var galaxySizeY = galaxy.StarSystems.GetLength(0);
			writer.WriteField(SaveField.GameDate,		galaxy.GameDate);
			writer.WriteField(SaveField.LastEventDate,	galaxy.LastEventDate);
			writer.WriteField(SaveField.RandomSeed,	0);	writer.Write(galaxy.RandomSeed);
			writer.WriteField(SaveField.AlienStrength,	(int)galaxy.AlienStrength);
			writer.WriteField(SaveField.StarSystems,	(galaxySizeX << 8) + galaxySizeY);

			#region Star System Array

			for (var i = 0; i < galaxySizeY; i++)
				for (var j = 0; j < galaxySizeX; j++)
				{
					var starSystem = galaxy.StarSystems[i,j];
					writer.WriteField(SaveField.StarName,		starSystem.Name);
					writer.WriteField(SaveField.TechLevel,		starSystem.TechLevel);
					writer.WriteField(SaveField.IsExplored,		starSystem.IsExplored);
					writer.WriteField(SaveField.Allegiance,		(int)starSystem.Allegiance);
					writer.WriteField(SaveField.CurrentEvent,	(int)starSystem.CurrentEvent);
					writer.WriteField(SaveField.EventDuration,	starSystem.EventDuration);

					writer.WriteField(SaveField.GoodsLegality,	Enums.All_Merchandise.Count);
					foreach (var merch in Enums.All_Merchandise)
						writer.Write((byte)(starSystem.IllegalGoods.Contains(merch) ? 1 : 0));

					writer.WriteField(SaveField.PriceDeviations, Enums.All_Merchandise.Count);
					for (var k = 0; k < Enums.All_Merchandise.Count; k++)
						writer.Write((sbyte)(starSystem.PriceChanges[k]));

					writer.WriteField(SaveField.StarSystemDataEnd,	0);
				}

			#endregion

			#region Global Event Log

			writer.WriteField(SaveField.GlobalEventsLog,	galaxy.GlobalEventLog.Count);
			foreach (var t in galaxy.GlobalEventLog)
			{
				writer.Write((byte)t.StarSystemIndex);
				writer.Write((byte)t.Event);
				writer.Write((short)t.EventValue);
				writer.WriteField(SaveField.CustomDate, t.EventDate);
			}

			#endregion

			writer.WriteField(SaveField.GalaxyDataEnd,	0);
		}
		

		public static void			Load(this GalaxyModel galaxy, BinaryReader reader)
		{
			var galaxyDataEnd = false;
			while (!galaxyDataEnd)
			{
				var field = reader.ReadUInt32();
				var fieldType	= (SaveField)(field >> 24);
				var fieldValue	= (int)field & 0x00FFFFFF;

				switch (fieldType)
				{
					case SaveField.GameDate		:	galaxy.GameDate		= GetDateFromValue(fieldValue);		break;
					case SaveField.LastEventDate:	galaxy.LastEventDate = GetDateFromValue(fieldValue);	break;
					case SaveField.RandomSeed	:	galaxy.RandomSeed	= reader.ReadInt32();				break;
					case SaveField.AlienStrength:	galaxy.AlienStrength = (AlienStrength)fieldValue;		break;
					case SaveField.StarSystems	:
						#region Read Star Systems

						var galaxySizeX = fieldValue >> 8;
						var galaxySizeY = fieldValue & 0x000000FF;
						galaxy.StarSystems = new StarSystemModel[galaxySizeY,galaxySizeX];
						for (var i = 0; i < galaxySizeY; i++)
							for (var j = 0; j < galaxySizeX; j++)
							{
								var system = new StarSystemModel();
								system.Coords = new Coord(j, i);
								var starSystemDataEnd = false;
								while (!starSystemDataEnd)
								{
									field = reader.ReadUInt32();
									fieldType	= (SaveField)(field >> 24);
									fieldValue	= (int)field & 0x00FFFFFF;
									switch (fieldType)
									{
										case SaveField.StarName			:	system.Name = new string(reader.ReadChars(20)).TrimEnd();	 break;
										case SaveField.TechLevel		:	system.TechLevel	= fieldValue;		break;
										case SaveField.IsExplored		:	system.IsExplored	= fieldValue > 0;	break;
										case SaveField.Allegiance		:	system.Allegiance	= (Allegiance)fieldValue;		break;
										case SaveField.CurrentEvent		:	system.CurrentEvent = (GlobalEventType)fieldValue;	break;
										case SaveField.EventDuration	:	system.EventDuration = fieldValue;		break;
										case SaveField.GoodsLegality	:
											for (var k = 0; k < fieldValue; k++)	
												if (reader.ReadByte() > 0)
													system.IllegalGoods.Add((Merchandise)k);
											break;
										case SaveField.PriceDeviations	:
											for (var k = 0; k < fieldValue; k++)	
												system.PriceChanges[k] = reader.ReadSByte();
											break;
										case SaveField.StarSystemDataEnd:	starSystemDataEnd = true;				break;
									}
								}

								system.LegalGoods.AddRange(Enums.All_Merchandise.Where(a => !system.IllegalGoods.Contains(a)));
								galaxy.StarSystems[i, j] = system;
							}
						break;

						#endregion

					case SaveField.GlobalEventsLog:
						#region Global Event Log

						galaxy.GlobalEventLog = new List<GlobalEvent>();
						for (var i = 0; i < fieldValue; i++)
						{
							var starIndex = reader.ReadByte();
							var @event = (GlobalEventType) reader.ReadByte();
							var eventvalue = reader.ReadInt16();
							var dateField = reader.ReadInt32();
							galaxy.GlobalEventLog.Add(new GlobalEvent(starIndex, @event, eventvalue, GetDateFromValue(dateField)));
						}
						break;

						#endregion
				
					case SaveField.GalaxyDataEnd:	galaxyDataEnd = true;				break;
				}
			}
		}

		#endregion


		#region Missions - Save/Load Methods

		public static void			SaveMissions(this PlayerModel player, BinaryWriter writer)
		{
			foreach (var mission in player.Missions)
			{
				writer.Write(mission.Id);
				writer.WriteField(SaveField.CustomDate, mission.Date);
				writer.Write(mission.ClientName.PadRight(10, ' ').ToCharArray());
				writer.Write((short)mission.RewardAmount);
				writer.Write((byte)mission.Type);
				writer.Write((byte)mission.Difficulty);
				writer.Write((byte)mission.TargetStarSystem.Coords.Y);
				writer.Write((byte)mission.TargetStarSystem.Coords.X);
				switch (mission.Type)
				{
					case MissionType.PackageDelivery:	writer.Write(mission.PackageDescription.PadRight(20).ToCharArray());	break;
					case MissionType.GoodsDelivery	:	writer.Write((byte)mission.GoodsToDeliver_Type);	writer.Write((byte)mission.GoodsToDeliver_Amount);	break;
					case MissionType.Assassination	:	mission.AssassinationTarget.Save(writer);	break;
				}
			}
		}

		public static MissionList	LoadMissions(BinaryReader reader, int missionCount)
		{
			var missions = new MissionList();
			for (var i = 0; i < missionCount; i++)
			{
				var mission = new MissionModel();
				mission.Id = reader.ReadInt32();
				mission.Date = GetDateFromValue(reader.ReadInt32());
				mission.ClientName		= new string(reader.ReadChars(10)).TrimEnd();
				mission.RewardAmount	= reader.ReadInt16();
				mission.Type			= (MissionType)reader.ReadByte();
				mission.Difficulty		= (Difficulty)reader.ReadByte();
				mission.TargetStarSystem = Galaxy.StarSystems[reader.ReadByte(), reader.ReadByte()];
				switch (mission.Type)
				{
					case MissionType.PackageDelivery:	mission.PackageDescription = new string(reader.ReadChars(20)).TrimEnd();	break;
					case MissionType.GoodsDelivery	:	mission.GoodsToDeliver_Type = (Merchandise)reader.ReadByte();	mission.GoodsToDeliver_Amount = reader.ReadByte();	break;
					case MissionType.Assassination	:	mission.AssassinationTarget = LoadNPC(reader);		break;
				}
				missions.Add(mission);
			}

			return missions;
		}

		#endregion


		#region NPC - Save/Load Methods

		public static void			Save(this NPC_Model npc, BinaryWriter writer)
		{
			writer.WriteField(SaveField.NPC_Type,		(int)npc.NPC_Type);
			writer.WriteField(SaveField.PlayerName,		npc.Name);
			writer.WriteField(SaveField.ShipName,		npc.Ship.ModelName);
			writer.WriteField(SaveField.CurrentHP,		npc.CurrentHP);
			writer.WriteField(SaveField.CurrentMissiles, npc.CurrentMissiles);
			writer.WriteField(SaveField.Attack,			npc.Attack);
			writer.WriteField(SaveField.Defense,		npc.Defense);
			writer.WriteField(SaveField.HasECM,			(int)npc.ECM);
			writer.WriteField(SaveField.Bounty,			npc.Bounty);
			writer.WriteField(SaveField.EscapeChance,	npc.EscapeChance);
			writer.WriteField(SaveField.IsRelevealedECM, npc.IsRelevealedECM);
			
			writer.WriteField(SaveField.CurrentCargo, Enums.All_Merchandise.Count);
			foreach (var merch in Enums.All_Merchandise)
				writer.Write((byte)npc.CurrentCargo[merch]);	
			
			writer.WriteField(SaveField.NPC_DataEnd, 0);
		}


		public static NPC_Model		LoadNPC(BinaryReader reader)
		{
			var npc = new NPC_Model();

			var npcDataEnd = false;
			while (!npcDataEnd)
			{
				var field = reader.ReadUInt32();
				var fieldType	= (SaveField)(field >> 24);
				var fieldValue	= (int)field & 0x00FFFFFF;

				switch (fieldType)
				{
					case SaveField.NPC_Type		:	npc.NPC_Type = (NPC_Type)fieldValue;					break;
					case SaveField.PlayerName	:	npc.Name = new string(reader.ReadChars(20)).TrimEnd();	break;
					case SaveField.ShipName		:
						var shipName = new string(reader.ReadChars(20)).TrimEnd();
						npc.Ship = GameConfig.ShipModels.FirstOrDefault(a => a.ModelName == shipName);
						break;
					case SaveField.CurrentHP	:	npc.CurrentHP		= fieldValue;		break;
					case SaveField.CurrentMissiles:	npc.CurrentMissiles = fieldValue;		break;
					case SaveField.Attack		:	npc.Attack			= fieldValue;		break;
					case SaveField.Defense		:	npc.Defense			= fieldValue;		break;
					case SaveField.HasECM		:	npc.ECM	= (EquipmentState)fieldValue;	break;
					case SaveField.Bounty		:	npc.Bounty			= fieldValue;		break;
					case SaveField.EscapeChance	:	npc.EscapeChance	= fieldValue;		break;
					case SaveField.IsRelevealedECM:	npc.IsRelevealedECM	= fieldValue > 0;	break;
					case SaveField.CurrentCargo	:	for (var i = 0; i < fieldValue; i++)	npc.CurrentCargo[(Merchandise)i] = reader.ReadByte();	break;
					case SaveField.NPC_DataEnd	:	npcDataEnd = true;				break;
				}
			}

			return npc;
		}

		#endregion


		#region Write/Read Fields

		public static void			WriteField(this BinaryWriter writer, SaveField fieldType, int value)
		{
			var fieldValue = ((int)fieldType << 24) + value;
			writer.Write((uint)fieldValue);
		}
		public static void			WriteField(this BinaryWriter writer, SaveField fieldType, bool value)
		{
			var fieldValue = ((int)fieldType << 24) + (value ? 1 : 0);
			writer.Write(fieldValue);
		}
		public static void			WriteField(this BinaryWriter writer, SaveField fieldType, string value)
		{
			var fieldValue = ((int)fieldType << 24) + value.Length;
			writer.Write(fieldValue);
			writer.Write(value.PadRight(20, ' ').ToCharArray());
		}
		public static void			WriteField(this BinaryWriter writer, SaveField fieldType, DateTime value)
		{
			var fieldValue = ((int)fieldType << 24) + ((value.Year-2200) << 16) + (value.Month << 8) + value.Day;
			writer.Write((uint)fieldValue);
		}

		public static DateTime		GetDateFromValue(int value)
		{
			value = value & 0x00FFFFFF;
			var year = (value >> 16) + 2200;
			var month = (value & 0x0000FF00) >> 8;
			var day = value & 0x000000FF;
			return new DateTime(year, month, day);
		}

		#endregion
	}


	public enum SaveField
	{
		PlayerName		=  1,
		ShipName		=  2,
		PosX			=  3,
		PosY			=  4,
		IsLanded		=  5, 
		Credits			=  7,
		FuelLeft		=  8,
		CurrentHP		=  9,
		CurrentMissiles	= 10,
		Attack			= 11,
		Defense			= 12,
		HasECM			= 13,
		HasScanner		= 14,
		HasMiningLaser	= 15,
		HasEscapeBoat	= 16,
		CurrentCargo	= 17,
		LegalStatus		= 18,
		CombatRating	= 19,
		Reputation		= 20,
		Missions		= 21,
		MilitaryRanks	= 22,
		PlayerDataEnd	= 99,

		GameDate		= 101,
		LastEventDate	= 102,
		RandomSeed		= 103,
		StarSystems		= 104,
		StarName		= 105,
		TechLevel		= 106,
		IsExplored		= 107,
		Allegiance		= 108,
		CurrentEvent	= 109,
		EventDuration	= 110,
		GoodsLegality	= 111,
		PriceDeviations	= 112,
		StarSystemDataEnd = 113,
		GlobalEventsLog = 114,
		AlienStrength	= 115,
		GalaxyDataEnd	= 199,

		CustomString	= 201,
		CustomDate		= 202,
		NPC_Type		= 211,
		Bounty			= 212,
		EscapeChance	= 213,
		IsRelevealedECM = 214,
		NPC_DataEnd		= 219,
	}
}
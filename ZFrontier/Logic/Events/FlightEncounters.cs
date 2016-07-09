namespace ZFrontier.Logic.Events
{
	using Objects.Galaxy;
	using Objects.GameData;
	using Objects.Units;
	using UI;
	using ZConsole;
	using ZLinq;


	public class FlightEncounters
	{
		#region Public Properties from Main class

		private static PlayerModel		Player				{	get {	return ZFrontier.Player;		}}
		private static GalaxyModel		Galaxy				{	get {	return ZFrontier.Galaxy;		}}
		private static StarSystemModel	CurrentSystem		{	get {	return ZFrontier.CurrentStarSystem;	}}
		private static LegalRecord		CurrentLegalRecord	{	get {	return Player.LegalRecords[CurrentSystem.Allegiance];	}}
		private static EventLog			EventLog			{	get {	return ZFrontier.EventLog;		}}
		private static PlayerStats		PlayerStats			{	get {	return ZFrontier.PlayerStats;	}}
		
		#endregion


		public static void		CreateEncounter()
		{
			var eventType = Enums.All_FlightEvents.Get_Random();

			var alienChance		= Tools.SetIntoRange(Galaxy.Get_AllSystems().Count(a => a.CurrentEvent == GlobalEventType.AlienInvasion), 0, RNG.DiceSize);
			var assassinChance	= Tools.SetIntoRange((int)Player.CombatRank, 0, RNG.DiceSize);

			switch (eventType)
			{
				case FlightEvent.Pirate  :	DoPirateEncounter();		break;

				case FlightEvent.Assassin:
					if (RNG.GetDice() <= assassinChance)							DoAssassinEncounter();	
					else if (CurrentLegalRecord.Status >= LegalStatus.Criminal)		DoPoliceEncounter();
					else															DoPirateEncounter();	break;
				
				case FlightEvent.Alien	:
					if (RNG.GetDice() <= alienChance)								DoAlienEncounter();		
					else if (CurrentLegalRecord.Status >= LegalStatus.Terrorist)	DoPoliceEncounter();
					else															DoTraderEncounter();	break;
				
				case FlightEvent.Trader	:	DoTraderEncounter();		break;
				
				case FlightEvent.Police	:	DoPoliceEncounter();		break;
				
				case FlightEvent.Asteroid:	DoAsteroidEncounter();		break;
			}

		}


		#region Basic Encounters

		private static void		DoPirateEncounter()
		{
			var pirateShip = NPC_Model.Create(NPC_Type.Pirate, CurrentSystem);

			var attackPlayer = RNG.GetDice() <= GameConfig.PirateAttackChance;
			if (attackPlayer)
			{
				BattleLogic.DoBattle(pirateShip);
				ZIOX.PressAnyKey(Player.CurrentHP > 0);
				return;
			}

			var toAttack = EventLog.Get_YesNo("Pirate_FliesBy", pirateShip.Name, pirateShip.Ship.ModelName, true, true);
			if (toAttack)
			{
				BattleLogic.DoBattle(pirateShip, true);
				ZIOX.PressAnyKey(Player.CurrentHP > 0);
			}
		}


		private static void		DoAssassinEncounter()
		{
			var assassinShip = NPC_Model.Create(NPC_Type.Assassin, CurrentSystem);
			BattleLogic.DoBattle(assassinShip);
			ZIOX.PressAnyKey(Player.CurrentHP > 0);
		}


		private static void		DoAlienEncounter()
		{
			var alienShip = NPC_Model.Create(NPC_Type.Alien, CurrentSystem);
			BattleLogic.DoBattle(alienShip);
			ZIOX.PressAnyKey(Player.CurrentHP > 0);
		}


		private static void		DoPoliceEncounter()
		{
			var policeShip = NPC_Model.Create(NPC_Type.Police, CurrentSystem);
			EventLog.Print("Police_Encounter", policeShip.Name, false);
			var toBeExamined = RNG.GetDice() <= GameConfig.PoliceExaminationChance;

			#region Player is Offender
			
			if (CurrentLegalRecord.Status == LegalStatus.Offender)
			{
				var toPayFines = EventLog.Get_YesNo("Police_DemandPayFines", policeShip.Name, CurrentLegalRecord.FineAmount.ToString(), true);
				if (!FinesPayment(policeShip, toPayFines))
					return;
			}

			#endregion

			#region Player is Criminal or Terrorist

			if (CurrentLegalRecord.Status == LegalStatus.Criminal  ||  CurrentLegalRecord.Status == LegalStatus.Terrorist)
			{
				EventLog.Print("Police_YouAreCriminal", policeShip.Name);
				BattleLogic.DoBattle(policeShip);
				ZIOX.PressAnyKey(Player.CurrentHP > 0);
				return;
			}

			#endregion
			
			if (toBeExamined)
			{
				var toAgree = EventLog.Get_YesNo("Police_ExamineConfirmation", policeShip.Name, true);
				if (toAgree)
				{
					#region Examination

					EventLog.Print("Police_ExamineConfirmationYes", Player.Name, false);
					if (Player.CurrentCargo.HasAny(CurrentSystem.IllegalGoods)  &&  RNG.GetDice() <= GameConfig.PoliceFindIllegalChance)
					{
						EventLog.Print("Police_ExamineFound", policeShip.Name);
						CurrentLegalRecord.FineAmount += GameConfig.Fine_IllegalGoods;
						Player.Statistics.IllegalGoodsFound = true;
						PlayerStats.Draw_PlayerStats();

						var toPayFines = EventLog.Get_YesNo("Police_ExamineFoundPayFines", policeShip.Name, CurrentLegalRecord.FineAmount.ToString(), true);
						if (FinesPayment(policeShip, toPayFines))
						{
							EventLog.Print("Police_Confiscation", policeShip.Name);
							Player.CurrentCargo.RemoveAllSpecified(CurrentSystem.IllegalGoods);
							PlayerStats.Draw_PlayerStats();
							ZIOX.PressAnyKey();
						}
						return;
					}
					EventLog.Print("Police_ExamineIsFine", policeShip.Name);

					#endregion				
				}
				else
				{
					#region Refuse to allow cargo examination

					EventLog.Print("Police_ExamineConfirmationNo", Player.Name, false);
					EventLog.Print("Police_ExamineConfirmationNoResult", policeShip.Name);
					BattleLogic.DoBattle(policeShip);
					ZIOX.PressAnyKey(Player.CurrentHP > 0);
					return;

					#endregion
				}
			}
			else
			{
				#region Police flies by

				EventLog.Print("Police_FliesBy", false);
				var toAttack = EventLog.Get_YesNo("Police_AttackConfirmation", true, true);
				if (toAttack)
				{
					BattleLogic.DoBattle(policeShip, true);
					ZIOX.PressAnyKey(Player.CurrentHP > 0);
				}
				return;

				#endregion			
			}

			ZIOX.PressAnyKey();
		}


		private static void		DoTraderEncounter()
		{
			var traderShip = NPC_Model.Create(NPC_Type.Trader, CurrentSystem);
			var toAttack = EventLog.Get_YesNo("Trader_AttackConfirmation", traderShip.Name, traderShip.Ship.ModelName, true, true);
			if (toAttack)
			{
				BattleLogic.DoBattle(traderShip);
				if (Player.CurrentHP > 0)
					EventLog.Print("Common_YourLegalStatusIs", Enums.Get_Name(CurrentLegalRecord.Status));
				ZIOX.PressAnyKey(Player.CurrentHP > 0);
			}
		}


		private static void		DoAsteroidEncounter()
		{			
			EventLog.Print("Asteroid_Encounter", false);
			if (Player.MiningLaser != EquipmentState.Yes)
				EventLog.Print("Asteroid_NeedMiningLaser");

			if (Player.MiningLaser == EquipmentState.Yes  &&  EventLog.Get_YesNo("Asteroid_MiningConfirmation", GameConfig.MiningFuelCost, true))
			{
				if (Player.FuelLeft < GameConfig.MiningFuelCost)
				{
					EventLog.Print("Asteroid_MiningNoFuel", GameConfig.MiningFuelCost);
				}
				else
				{
					Player.Statistics.AsteroidsMined++;
					Player.FuelLeft -= GameConfig.MiningFuelCost;
					EventLog.Print("Asteroid_MiningStarted", false);
					var result = Tools.SetIntoRange(RNG.GetDice() - RNG.DiceSize + GameConfig.AsteroidRichness, 0, 6);
					if (result > 0)		EventLog.Print("Asteroid_MineralsFound", result);
					else				EventLog.Print("Asteroid_NothingFound");

					Player.CurrentCargo[Merchandise.Minerals] += result;
					if (Player.CurrentCargo.CurrentLoad > Player.MaxCargoLoad)
					{
						EventLog.Print("Asteroid_NotEnoughSpace", Player.CurrentCargo.CurrentLoad - Player.MaxCargoLoad);
						Player.CurrentCargo[Merchandise.Minerals] -= Player.CurrentCargo.CurrentLoad - Player.MaxCargoLoad;
					}
					
					PlayerStats.Draw_PlayerStats();
				}
				ZIOX.PressAnyKey();
				return;
			}

			ZIOX.PressAnyKey(Player.MiningLaser != EquipmentState.Yes);				
		}


		private static bool		FinesPayment(NPC_Model policeShip, bool toPayFines)
		{
			if (toPayFines  &&  Player.Credits >= CurrentLegalRecord.FineAmount)
			{
				Player.Credits -= CurrentLegalRecord.FineAmount;
				CurrentLegalRecord.FineAmount = 0;
				PlayerStats.Draw_PlayerStats();				
				EventLog.Print("Police_FinesPaymentYes", policeShip.Name);
				return true;
			}

			EventLog.Print("Police_FinesPaymentNo", policeShip.Name);
			BattleLogic.DoBattle(policeShip);
			return false;
		}

		#endregion

		#region Alien Invasion

		public static void		DoAlienInvasion()
		{
			ZFrontier.Draw_Controls(false);

			EventLog.ShadowOldMessages();
			EventLog.Print("Flight_Start", CurrentSystem.Name);

			var isSuccess = true;
			for (var i = 0; i < GameConfig.AlienInvasionShipCount; i++)
			{
				var alienShip = NPC_Model.Create(NPC_Type.Alien, CurrentSystem);
				BattleLogic.DoBattle(alienShip, true);
				
				EventLog.PrintDivider();
				EventLog.ShadowOldMessages();

				Galaxy.GameDate = Galaxy.GameDate.AddDays(1);
				PlayerStats.Draw_PlayerStats();

				#region If Player is Dead or has fled

				if (Player.IsDead)
				{
					break;
				}
				if (!alienShip.IsDead)
				{
					isSuccess = false;
					break;
				}
				
				#endregion				
			}

			#region Big Boss

			if (Galaxy.AlienStrength == AlienStrength.FatherIsHere  &&  !Player.IsDead)
			{
				isSuccess = false;
				var alienShip = NPC_Model.Create(NPC_Type.Alien, CurrentSystem, "Panther");
				alienShip.Name		= ZFrontier.Lang["NPC_AlienBossName"];
				alienShip.CurrentHP	= alienShip.MaxHP;
				alienShip.CurrentMissiles = alienShip.MaxMissiles;
				alienShip.Attack	= 11;
				alienShip.Defense	= 8;
				alienShip.ECM		= EquipmentState.Yes;
				alienShip.IsImmuneToCrits = true;
				BattleLogic.DoBattle(alienShip, true);
				ZIOX.PressAnyKey();

				if (alienShip.IsDead)
				{
					Galaxy.AlienStrength = AlienStrength.FatherIsKilled;
					EventLog.Print("Common_AlienBossIsKilled");
					ZIOX.PressAnyKey();
					EventLog.PrintDivider();
					isSuccess = true;
				}
			}

			#endregion

			if (Player.IsDead)
			{
				EventLog.Print("Common_PlayerIsDead");
				ZIOX.PressAnyKey();
				EventLog.PrintDivider();
				return;
			}

			#region If you have won the battle

			if (isSuccess)
			{
				Player.Credits += GameConfig.AlienInvasionReward;
				Player.LegalRecords[CurrentSystem.Allegiance].FineAmount = 0;
				CurrentSystem.CurrentEvent = GlobalEventType.Normal;

				EventLog.PrintPlain(string.Format(
					GameConfig.Lang["GlobalEvent_AlienInvasionSuccess"],
					CurrentSystem.Name,
					GameConfig.AlienInvasionReward,
					Enums.Get_Name(CurrentSystem.Allegiance),
					Enums.Get_Name(LegalStatus.Clean)));
				ZIOX.PressAnyKey();
				ZFrontier.LandOnPlanet();
			}
			else
			{
				EventLog.Print("GlobalEvent_AlienInvasionFail");
			}

			#endregion
		}

		#endregion
	}
}

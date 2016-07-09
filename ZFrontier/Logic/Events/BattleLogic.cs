namespace ZFrontier.Logic.Events
{
	using System.Collections.Generic;
	using Objects.GameData;
	using Objects.Units;
	using UI;
	using ZConsole;
	using ZLinq;


	public class BattleLogic
	{
		#region Menu Items

		public static string Menu_Attack_Laser;
		public static string Menu_Attack_Missile;
		public static string Menu_RunAway;
		public static string Menu_DropCargoAndRun;
		public static string Menu_DropAllCargo;
		public static string Menu_DropOneCargo;
		public static string Menu_ScoopCargo;

		#endregion

		#region Public Properties from Main class

		private static TranslationSet	Lang			{	get {	return GameConfig.Lang;				}}
		private static PlayerModel		Player			{	get {	return ZFrontier.Player;			}}
		
		private static Rect				NPCStatsArea	{	get {	return ZFrontier.BattleStatsArea;	}}
		private static ActionPanel		ActionPanel		{	get {	return ZFrontier.ActionPanel;		}}
		private static EventLog			EventLog		{	get {	return ZFrontier.EventLog;			}}
		private static PlayerStats		PlayerStats		{	get {	return ZFrontier.PlayerStats;		}}
		private static BattleStats		BattleStats		{	get {	return ZFrontier.BattleStats;		}}
		private static LegalRecord		CurrentLegalRecord	{	get {	return Player.LegalRecords[ZFrontier.CurrentStarSystem.Allegiance];	}}

		private static CargoDrop		droppedTraderCargo;

		#endregion



		private static void			BattleInitiation(NPC_Model enemy, bool playerIsAttacker)
		{
			ActionPanel.HighlightArea();
			EventLog.ShadowOldMessages();
			
			var npcType = enemy.NPC_Type;
			switch (npcType)
			{
				case NPC_Type.Pirate:
				case NPC_Type.Assassin:
				case NPC_Type.Alien:
				case NPC_Type.Guard:
					if (enemy.BattleInitMessage == null)
						EventLog.Print(!playerIsAttacker ? "Battle_Init_StandardAttack" : "Battle_Init_PlayerAttack", enemy.Name);
					else
						EventLog.Print(enemy.BattleInitMessage, enemy.Name);
					break;

				case NPC_Type.Trader:
					EventLog.Print("Battle_Init_AttackTrader", enemy.Name);
					Player.Statistics.AttackedTrader = true;
					break;

				case NPC_Type.Police:
					EventLog.Print("Battle_Init_AttackPolice", enemy.Name);
					Player.Statistics.AttackedPolice = true;
					break;
			}

			CurrentLegalRecord.FineAmount += GameConfig.NPC_StatsConfigs[npcType].FineForAttack;
			EventLog.Print("Battle_Init_YouAreFightingWith", Lang["NPC_" + npcType], enemy.Name);
			BattleStats.Draw_NPC_Stats(enemy, Player);
			BattleStats.Draw_Player_Stats(Player);
		}


		public static void			DoBattle(NPC_Model enemy, bool playerIsAttacker = false)
		{
			BattleInitiation(enemy, playerIsAttacker);
			
			var enemyIsTryingToEscape = false;
			var playerIsTryingToEscape = false;
			var battleIsEnded = false;
			var cargoDropped = 0;
			droppedTraderCargo = new CargoDrop();

			while (!battleIsEnded)
			{
				#region Player's Action

				var action = Get_FightMenuResult(enemy);
				EventLog.ShadowOldMessages();

				if		(action == Menu_Attack_Laser)		DoAttack(Player, enemy, enemy, AttackType.Laser);
				else if (action == Menu_Attack_Missile)		DoAttack(Player, enemy, enemy, AttackType.Missile);
				else if (action == Menu_RunAway)		{	EventLog.Print("Battle_TryToEscape");	playerIsTryingToEscape = true;	}
				else if (action == Menu_ScoopCargo)		{	ScoopDroppedCargo(true);				battleIsEnded = true;			}
				else if (action == Menu_DropAllCargo)
				{
					cargoDropped = Player.CurrentCargo.CurrentLoad;
					Player.CurrentCargo.Clear();
					battleIsEnded = cargoDropped >= GameConfig.MaxDropCargoBonus;
					EventLog.Print(battleIsEnded ? "Battle_DroppedAllCargoEscaped" : "Battle_DroppedCargoTryToEscape");
					playerIsTryingToEscape = true;
					PlayerStats.Draw_PlayerStats();
					ZIOX.PressAnyKey(battleIsEnded);
					
				}
				if (action.StartsWith(Menu_DropOneCargo))
				{
					var result = Enums.Get_Value<Merchandise>(action.Substring(Menu_DropOneCargo.Length));
					EventLog.Print("Battle_DroppedOneCargo", Enums.Get_Name(result));
					Player.CurrentCargo[result]--;
					cargoDropped = 1;
					playerIsTryingToEscape = true;
					PlayerStats.Draw_PlayerStats();
				}				

				#endregion

				#region Enemy's Action

				if (battleIsEnded)
					break;
				droppedTraderCargo.Clear();

				if (enemy.CurrentHP > 0)
				{
					if ((enemy.CurrentHP < GameConfig.EnemyPanicLevel  ||  (enemy.NPC_Type == NPC_Type.Trader  &&  enemy.CurrentHP < enemy.MaxHP / 2))  &&  enemy.NPC_Type != NPC_Type.Alien  &&  enemy.EscapeChance > 0)
					{
						#region Enemy is trying to escape

						if (!enemyIsTryingToEscape)
						{
							if (enemy.NPC_Type == NPC_Type.Trader)
							{
								#region Trader Cargo Drop

								var enemyCargoDropped = RNG.GetDice() <= GameConfig.TraderDropChance;
								if (enemyCargoDropped)
								{
									EventLog.Print("Battle_EnemyDroppedCargo", enemy.Name);
									droppedTraderCargo = enemy.CurrentCargo.GetRandonDrop(0);
									ShowDroppedCargo();
									EventLog.Print("Battle_ScoopCargoOrLoseIt");
								}
								else
									EventLog.Print("Battle_EnemyRunsAway", enemy.Name);
								
								#endregion								
							}
							else
								EventLog.Print("Battle_EnemyRunsAway", enemy.Name);
							enemyIsTryingToEscape = true;
						}
						else
						{
							battleIsEnded = RNG.GetDice() <= GameConfig.PlayerEscapeChance;
							EventLog.Print(battleIsEnded ? "Battle_EnemyEscapeSuccess"
														 : "Battle_EnemyEscapeFail", enemy.Name);
						}

						#endregion
					}
					else
					{
						#region Enemy's Attack

						var willMakeShot = (enemy.NPC_Type != NPC_Type.Pirate || cargoDropped == 0)  ||  RNG.GetDice() > GameConfig.DropCargoBonus + cargoDropped-1;
						if (cargoDropped > 0)
							EventLog.Print(willMakeShot ? "Battle_EnemyIgnoresDroppedCargo"
														: "Battle_EnemyScoopsCargo", enemy.Name);
						if (willMakeShot)
						{
							DoAttack(enemy, Player, enemy, 
								enemy.CurrentMissiles > 0  &&  RNG.GetDice() <= GameConfig.EnemyMissileUsage ? AttackType.Missile : AttackType.Laser);
						}

						#endregion						
					}
				}
				else
				{
					#region Enemy is Dead

					battleIsEnded = true;
					var npcType = enemy.NPC_Type;
					Player.Statistics.ShipDestroyed[enemy.Ship.ModelName]++;
					Player.Statistics.NPC_Defeated[npcType]++;
					Player.CombatRating += enemy.Ship.MaxHP;

					Player.Credits += enemy.Bounty;
					if (enemy.Bounty > 0)
					{
						EventLog.Print("Battle_EarnedBounty", enemy.Bounty);
					}

					CurrentLegalRecord.FineAmount += GameConfig.NPC_StatsConfigs[npcType].FineForAttack;
					if (npcType == NPC_Type.Trader  &&  enemy.EscapeChance > 0)
					{
						droppedTraderCargo = enemy.CurrentCargo.GetRandonDrop(1);
						ShowDroppedCargo();
						ScoopDroppedCargo(false);
					}

					#endregion					
				}

				#endregion

				#region Player's Death or Escape

				if (Player.CurrentHP == 0)
				{
					battleIsEnded = true;
					playerIsTryingToEscape = false;
				}
				if (playerIsTryingToEscape)
				{
					battleIsEnded = RNG.GetDice() <= enemy.EscapeChance;
					EventLog.Print(battleIsEnded ? "Battle_EscapeSuccess" : "Battle_EscapeFail");
					playerIsTryingToEscape = false;
				}				

				#endregion								
			}

			PlayerStats.Draw_PlayerStats();
			ActionPanel.ClearArea();
			EventLog.HighlightArea();			
		}


		private static string		Get_FightMenuResult(NPC_Model enemy)
		{
			#region Get items for menu

			var mainMenuItems = new ZMenu.MenuItemList
				{
					new ZMenu.MenuItem(Menu_Attack_Laser),
					new ZMenu.MenuItem(Menu_Attack_Missile) { IsActive = Player.CurrentMissiles > 0 },
					new ZMenu.MenuItem(Menu_RunAway)
				};

			if (enemy.NPC_Type == NPC_Type.Pirate)
			{
				var dropCargoMenuItems = new ZMenu.MenuItemList();
				dropCargoMenuItems.AddRange(from merch in Enums.All_Merchandise 
											where Player.CurrentCargo[merch] > 0
											select new ZMenu.MenuItem(Menu_DropOneCargo + Lang["Merchandise_" + merch]));
				dropCargoMenuItems.Add(new ZMenu.MenuItem(Menu_DropAllCargo));
				mainMenuItems.Add(new ZMenu.MenuItem { Caption = Menu_DropCargoAndRun,  IsActive = Player.CurrentCargo.CurrentLoad > 0, ChildMenuItems = dropCargoMenuItems });
			}

			if (enemy.NPC_Type == NPC_Type.Trader  &&  droppedTraderCargo.Count > 0)
			{
				mainMenuItems.Add(new ZMenu.MenuItem { Caption = Menu_ScoopCargo });
			}

			#endregion

			return ZMenu.GetMenuResult(NPCStatsArea.Left + 24, NPCStatsArea.Top+1,
				new ZMenu.MenuItem(Lang["Common_YourAction"])
					{
						Options = new ZMenu.Options
							{
								ColorScheme = new ZMenu.ColorScheme { FrameForeColor = Color.Cyan, CaptionForeColor = Color.Yellow },
								IsMonoStyleColor = true,
								UseSelectedBackColor = true
							},

						ChildMenuItems = mainMenuItems
					},
				ZFrontier.HotKeys).Text;
		}

		private static void			DoAttack(BasicUnitModel attacker, BasicUnitModel defender, NPC_Model enemy, AttackType attackType)
		{
			var resultDamage = 0;
			switch (attackType)
			{
				case AttackType.Laser	:
					resultDamage = DoLaserAttack(attacker, defender);
					break;

				case AttackType.Missile	:
					resultDamage = DoMissileAttack(attacker, defender);
					if (attacker.IsPlayer)
						Player.Statistics.MissilesUsed++;
					break;
			}

			if (attacker.IsPlayer)	Player.Statistics.DamageInflicted	+= resultDamage;
			else					Player.Statistics.DamageTaken		+= resultDamage;

			defender.CurrentHP -= (resultDamage);
			if (defender.IsDead)
			{
				defender.CurrentHP = 0;
				EventLog.Print(defender.IsPlayer ? "Battle_YouAreDead" : "Battle_EnemyIsDead");
				ZIOX.PressAnyKey(defender.IsPlayer);
			}
			
			PlayerStats.Draw_PlayerStats();
			BattleStats.Draw_Player_Stats(Player);
			BattleStats.Draw_NPC_Stats(enemy, Player);
		}

		private static int			DoLaserAttack(BasicUnitModel attacker, BasicUnitModel defender)
		{
			var attackBonus  = RNG.GetDiceDiv2();
			var defenseBonus = RNG.GetDiceDiv2();
			var dmg = attacker.Attack  + attackBonus;
			var def = defender.Defense + defenseBonus;
			var rawResultDamage = dmg - def;
			var resultDamage = rawResultDamage > 0 ? dmg - def : 0;
			
			EventLog.PrintPlain(string.Format(Lang["Battle_Laser_Attack"], 
				attacker.IsPlayer ? Player.Name : attacker.Name, 
				Enums.Get_Name((BattleActionSuccess)attackBonus),
				defender.IsPlayer ? Player.Name : defender.Name,
				attackBonus),
				false);
			
			EventLog.PrintPlain(string.Format(Lang["Battle_Laser_Defense"], 
				defender.IsPlayer ? Player.Name : defender.Name,
				Enums.Get_Name((BattleActionSuccess)defenseBonus),
				defenseBonus),
				false);

			EventLog.PrintPlain(string.Format(Lang["Battle_Laser_Result"], 
				attacker.Attack, attackBonus, defender.Defense, defenseBonus, resultDamage, rawResultDamage));

			if (attackBonus == 3  &&  defenseBonus == 1  &&  RNG.GetDice() <= GameConfig.CriticalChanceLaser)
				{
					if (attacker.IsPlayer  &&  !defender.IsImmuneToCrits)
					{
						EventLog.Print("Battle_Laser_CriticalPlayer");
						defender.CurrentHP = 0;
					}
					else
					{
						EventLog.Print("Battle_Laser_CriticalEnemy", Enums.Get_Name(Get_EquipToDamage()));
					}
				}

			return resultDamage;			
		}

		private static int			DoMissileAttack(BasicUnitModel attacker, BasicUnitModel defender)
		{
			EventLog.Print("Battle_Missile_Fired", attacker.IsPlayer ? Player.Name : attacker.Name,  defender.IsPlayer ? Player.Name : defender.Name, false);

			attacker.CurrentMissiles--;
			var isHit = RNG.GetDice() <= GameConfig.MissileAccuracy;

			if (isHit)
			{
				if (!defender.IsPlayer)
				{
					((NPC_Model) defender).IsRelevealedECM = true;
				}

				if (defender.ECM == EquipmentState.Yes)
				{
					EventLog.Print("Battle_Missile_UseECM", defender.IsPlayer ? Player.Name : defender.Name, false);
					if (RNG.GetDice() <= GameConfig.ECM_Effeciency)
					{
						EventLog.Print("Battle_Missile_Destroyed");
						return 0;
					}
					EventLog.Print("Battle_Missile_ECM_Failed", GameConfig.MissileDamage);									
				}
				else
				{
					EventLog.Print("Battle_Missile_DirectHit", GameConfig.MissileDamage);
				}

				if (RNG.GetDice() <= GameConfig.CriticalChanceMissile)
				{
					if (attacker.IsPlayer  &&  !defender.IsImmuneToCrits)
					{
						EventLog.Print("Battle_Missile_CriticalPlayer");
						defender.CurrentHP = 0;
					}
					else
					{
						EventLog.Print("Battle_Missile_CriticalEnemy", Enums.Get_Name(Get_EquipToDamage()));
					}
				}
				
				return GameConfig.MissileDamage;
			}
			EventLog.Print("Battle_Missile_Missed"); 
			return 0;	
		}

		private static void			ScoopDroppedCargo(bool isTraderAlive)
		{
			if (droppedTraderCargo.Count > 0)
			{
				droppedTraderCargo.Shuffle();
				foreach (var t in droppedTraderCargo)
					Player.CurrentCargo[t.Key] += Tools.SetIntoRange(t.Value, 0, Player.MaxCargoLoad - Player.CurrentCargo.CurrentLoad);
				EventLog.Print("Battle_ScoopedAllCargo", false);
			}
			EventLog.Print(isTraderAlive ? "Battle_TraderRunsAway": "Battle_TraderIsDead");
		}

		private static void			ShowDroppedCargo()
		{
			if (droppedTraderCargo.Count > 0)
			{
				EventLog.Print("Battle_CargoDropped", false);
				foreach (var t in droppedTraderCargo)
					EventLog.Print("Battle_CargoDroppedMerch", t.Value.ToString(), Enums.Get_Name(t.Key), false);
				EventLog.PrintLine();
			}
			else
			{
				EventLog.Print("Battle_NoCargoDropped");
			}
		}


		private static Equipment	Get_EquipToDamage()
		{
			var equipList = new List<Equipment>();
			if (Player.ECM			== EquipmentState.Yes)	equipList.Add(Equipment.ECM);
			if (Player.Scanner		== EquipmentState.Yes)	equipList.Add(Equipment.Scanner);
			if (Player.MiningLaser	== EquipmentState.Yes)	equipList.Add(Equipment.MiningLaser);
			if (Player.EscapeBoat	== EquipmentState.Yes)	equipList.Add(Equipment.EscapeBoat);

			var result = (equipList.Count > 0) ? equipList.Get_Random() : Equipment.Shield;
			switch (result)
			{
				case Equipment.ECM			:	Player.ECM			= EquipmentState.No;	break;
				case Equipment.Scanner		:	Player.Scanner		= EquipmentState.No;	break;
				case Equipment.MiningLaser	:	Player.MiningLaser	= EquipmentState.No;	break;
				case Equipment.EscapeBoat	:	Player.EscapeBoat	= EquipmentState.No;	break;
				case Equipment.Shield		:	Player.Defense--;							break;
			}

			return result;
		}
	}

	
	public enum BattleActionSuccess
	{
		Bad		  = 1,
		Good	  = 2,
		Brilliant = 3
	}

	public enum AttackType
	{
		None,
		Laser,
		Missile
	}
}

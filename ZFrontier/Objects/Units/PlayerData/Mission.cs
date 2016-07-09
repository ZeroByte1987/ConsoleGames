namespace ZFrontier.Objects.Units.PlayerData
{
	using System;
	using System.Collections.Generic;
	using Galaxy;
	using GameData;
	using Logic;
	using Logic.Events;
	using ZLinq;


	public class MissionModel
	{
		#region Public Properties

		public int				Id						{ get; set; }
		public string			ClientName				{ get; set; }
		public int				RewardAmount			{ get; set; }
		public MissionType		Type					{ get; set; }
		public Difficulty		Difficulty				{ get; set; }
		public StarSystemModel	TargetStarSystem		{ get; set; }
		public DateTime			Date					{ get; set; }
		public int				DaysBeforeDate			{ get { return (int) (Date - ZFrontier.Galaxy.GameDate).TotalDays; }}
		public MissionTypeModel MissionTypeData			{ get { return GameConfig.MissionTypes.Single(a => a.Type == Type  &&  a.Difficulty == Difficulty); }}

		public string			PackageDescription		{ get; set; }
		public Merchandise		GoodsToDeliver_Type		{ get; set; }
		public int				GoodsToDeliver_Amount	{ get; set; }
		public NPC_Model		AssassinationTarget		{ get; set; }
		public Allegiance		MilitaryAllegiance		{ get; set; }

		#endregion

		public static MissionModel		CreateRandom(MissionType missionType, bool isMilitary = false, Allegiance militaryAllegiance = Allegiance.Independent)
		{
			var currentSystemAllegiance = ZFrontier.CurrentStarSystem.Allegiance;
			var system = ZFrontier.Galaxy.Get_RandomSystemForEvent(GlobalEventType.AlienInvasion);
			var mission = new MissionModel
				            {
								Id = RNG.GetNumber(int.MaxValue),
					            Type = missionType,
								ClientName = isMilitary ? ZFrontier.Lang["MilitaryHQ_ClientName_" + currentSystemAllegiance] : GameConfig.Get_NPC_Name(NPC_Type.Trader),
								TargetStarSystem = system,
								Difficulty = (Difficulty)RNG.GetDiceDiv2Zero(),
								MilitaryAllegiance = militaryAllegiance
							};

			var distance = GameConfig.Get_Distance(system.Coords.X, system.Coords.Y, ZFrontier.Player);
			var randomDelta = RNG.GetNumber(15)/10f;
			mission.Date = ZFrontier.Galaxy.GameDate.AddDays((distance + GameConfig.EventsPerFlight) * (1 + randomDelta));
			mission.RewardAmount = (int)((distance + mission.MissionTypeData.RewardAmount) * (1.75 - randomDelta/2));

			switch (missionType)
			{
				case MissionType.PackageDelivery:
				case MissionType.Military_Delivery:
					mission.PackageDescription = string.Empty;
					break;

				case MissionType.GoodsDelivery	:
					mission.GoodsToDeliver_Type = (system.IllegalGoods.Count > 0) ? system.IllegalGoods.Get_Random() : system.LegalGoods.Get_Random();
					mission.GoodsToDeliver_Amount = RNG.GetDice();
					mission.RewardAmount += GameConfig.GoodsPrices[system.TechLevel][mission.GoodsToDeliver_Type];
					break;

				case MissionType.Assassination	:
				case MissionType.Military_Assassination:
					var shipNames = GameConfig.ShipModels.Select(a => a.ModelName).ToArray();
					var shipName = shipNames[GameConfig.ShipModels.Count - 4 + (int)mission.Difficulty + RNG.GetNumber(2)];
					mission.AssassinationTarget = NPC_Model.Create(NPC_Type.Trader, system.TechLevel, shipName);
					mission.AssassinationTarget.EscapeChance = 0;
					mission.AssassinationTarget.CurrentCargo.Clear();
					break;
			}

			return mission;
		}
	}


	public class MissionList : List<MissionModel>
	{
		public void UpdateAndRemove()
		{
			foreach (var mission in this.Where(mission => mission.Date < ZFrontier.Galaxy.GameDate))
			{
				ZFrontier.EventLog.Print("MissionFailed_" + mission.Type, mission.ClientName,
					  mission.Type == MissionType.GoodsDelivery ? Enums.Get_Name(mission.GoodsToDeliver_Type)
					: mission.Type == MissionType.Passenger		? mission.TargetStarSystem.Name
					: mission.Type == MissionType.Assassination ? mission.AssassinationTarget.Name : string.Empty);
				ZFrontier.EventLog.Print("MissionFailed_InfoMessage", Enums.Get_Name(mission.Type), mission.TargetStarSystem.Name);
				ZFrontier.Player.ReputationRating -= mission.MissionTypeData.ReputationChange*2;
				if (mission.Type >= MissionType.Military_Delivery)
					ZFrontier.Player.MilitaryRanks[mission.MilitaryAllegiance].Rating -= mission.MissionTypeData.MilitaryRankChange*2;
			}
			RemoveAll(a => a.Date < ZFrontier.Galaxy.GameDate);
		}

		public void Process(bool isLanding)
		{
			var player			= ZFrontier.Player;
			
			foreach (var mission in this.Where(mission => mission.Date >= ZFrontier.Galaxy.GameDate  &&  mission.TargetStarSystem.Name == ZFrontier.CurrentStarSystem.Name))
			{
				var missionData = mission.MissionTypeData;
				var missionIsDone = false;

				if (isLanding)
				{
					#region Check the mission on landing

					if (mission.Type == MissionType.PackageDelivery  ||  mission.Type == MissionType.Passenger  ||  mission.Type == MissionType.Military_Delivery)
						missionIsDone = true;						

					if (mission.Type == MissionType.GoodsDelivery  &&  player.CurrentCargo[mission.GoodsToDeliver_Type] >= mission.GoodsToDeliver_Amount)
					{
						missionIsDone = true;
						player.CurrentCargo[mission.GoodsToDeliver_Type] -= mission.GoodsToDeliver_Amount;
					}

					#endregion
				}
				else
				{
					#region Assassins Attack

					if (mission.Type == MissionType.Assassination  ||  mission.Type == MissionType.Military_Assassination)
					{
						ZFrontier.EventLog.Print("MissionStart_Assassination");
						ZIOX.PressAnyKey();
					}
						
					for (var i = 0; i < missionData.Assassins; i++)
					{
						var enemyNPC = NPC_Model.Create((mission.Type == MissionType.Assassination  ||  mission.Type == MissionType.Military_Assassination) ? NPC_Type.Guard : NPC_Type.Assassin, 1);
						enemyNPC.Attack  += missionData.AssassinStrength;
						enemyNPC.Defense += missionData.AssassinStrength;
						enemyNPC.BattleInitMessage = "MissionBattleMessage_" + mission.Type;
						BattleLogic.DoBattle(enemyNPC);
						ZIOX.PressAnyKey(!player.IsDead);
						if (player.IsDead)
							break;
					}

					if ((mission.Type == MissionType.Assassination  ||  mission.Type == MissionType.Military_Assassination)  &&  !player.IsDead)
					{
						BattleLogic.DoBattle(mission.AssassinationTarget, true);
						missionIsDone = mission.AssassinationTarget.IsDead;
					}

					#endregion
				}	

				#region If Missions Is Done

				if (missionIsDone)
				{
					ZFrontier.EventLog.Print("MissionDone_" + mission.Type, mission.ClientName, mission.RewardAmount.ToString());
					player.Credits += mission.RewardAmount;
					player.ReputationRating += missionData.ReputationChange;
					if (mission.Type >= MissionType.Military_Delivery)
						ZFrontier.Player.MilitaryRanks[mission.MilitaryAllegiance].Rating += mission.MissionTypeData.MilitaryRankChange;
					mission.Date = new DateTime(1, 1, 1);
				}
				
				if (player.IsDead)
					break;

				#endregion
			}

			ZFrontier.PlayerStats.Draw_PlayerStats();
			RemoveAll(a => a.Date < ZFrontier.Galaxy.GameDate);
		}
	}


	public class MissionTypeModel
	{
		public MissionType	Type					{ get; set; }
		public Difficulty	Difficulty				{ get; set; }
		public int			RewardAmount			{ get; set; }
		public int			ReputationNeeded		{ get; set; }
		public int			CombatRatingNeeded		{ get; set; }
		public int			MilitaryRatingNeeded	{ get; set; }
		public int			ReputationChange		{ get; set; }
		public int			MilitaryRankChange		{ get; set; }
		public int			Assassins				{ get; set; }
		public int			AssassinStrength		{ get; set; }
		
		public MissionTypeModel(MissionType type, Difficulty difficulty, int rewardAmount, int reputationNeeded = 0, int combatRatingNeeded = 0, int militaryRatingNeeded = 0, 
			int reputationChange = 0, int militaryRankChange = 0, int assassins = 0, int assassinStrength = 0)
		{
			Type				= type;
			Difficulty			= difficulty;
			RewardAmount		= rewardAmount;
			ReputationNeeded	= reputationNeeded;
			CombatRatingNeeded	= combatRatingNeeded;
			MilitaryRatingNeeded = militaryRatingNeeded;
			ReputationChange	= reputationChange;
			MilitaryRankChange	= militaryRankChange;
			Assassins			= assassins;
			AssassinStrength	= assassinStrength;
		}

		public MissionTypeModel()
		{
		}
	}
}
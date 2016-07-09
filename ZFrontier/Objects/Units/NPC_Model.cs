namespace ZFrontier.Objects.Units
{
	using Galaxy;
	using GameData;
	using Logic;
	using PlayerData;
	using ZConsole;
	using ZLinq;


	public class NPC_Model : BasicUnitModel
	{
		#region Public Properties

		public NPC_Type		NPC_Type			{ get; set; }

		public bool			IsRelevealedECM		{ get; set; }

		public int			Bounty				{ get; set; }

		public int			EscapeChance		{ get; set; }

		public string		BattleInitMessage	{ get; set; }

		#endregion

		#region Constructors
		
		public NPC_Model()
		{
			CurrentCargo = new CargoStorage();
		}

		private NPC_Model(NPC_Type npcType, string shipModelName = null)
		{
			var shipsUsage  = GameConfig.NPC_StatsConfigs[npcType].ShipsUsage;
			NPC_Type		= npcType;
			Name			= GameConfig.Get_NPC_Name(npcType);
			Ship			= shipModelName == null
								? GameConfig.ShipModels[RNG.GetNumber(shipsUsage.Min, shipsUsage.Max+1)]
								: GameConfig.ShipModels.Single(a => a.ModelName == shipModelName);
			CurrentHP		= Tools.SetIntoRange(Ship.MaxHP - RNG.GetDiceZero(), Ship.MaxHP * 2 / 3, Ship.MaxHP);
			CurrentMissiles = RNG.GetNumber(Ship.MaxMissiles);
			IsRelevealedECM = false;
		}

		#endregion
		

		public static NPC_Model Create(NPC_Type	npcType, StarSystemModel system, string shipModelName = null)
		{
			return Create(npcType, system.TechLevel, shipModelName);
		}
		public static NPC_Model Create(NPC_Type	npcType, int techLevel, string shipModelName = null, int strengthBonus = 0)
		{
			strengthBonus += 
				  npcType == NPC_Type.Pirate ? (RNG.DiceSize - techLevel) / 2
				: npcType == NPC_Type.Police ? (techLevel+1) / 2
				: 0;

			var npcConfig = GameConfig.NPC_StatsConfigs[npcType];

			var model = new NPC_Model(npcType, shipModelName)
				{
					Bounty	= RNG.GetNumber(npcConfig.Bounty),
					Attack	= Tools.SetIntoRange(RNG.GetDice()   + npcConfig.Bonus_Attack  + strengthBonus, GameConfig.AttackRange.Min,  GameConfig.AttackRange.Max),
					Defense = Tools.SetIntoRange(RNG.GetDice()-2 + npcConfig.Bonus_Defense + strengthBonus, GameConfig.DefenseRange.Min, GameConfig.DefenseRange.Max),
					ECM		= RNG.GetDice() + strengthBonus > (RNG.DiceSize - npcConfig.Bonus_ECM) ? EquipmentState.Yes : EquipmentState.No,
					EscapeChance = npcConfig.EscapeChance
				};

			if (npcType == NPC_Type.Trader)
			{
				model.CurrentCargo = new CargoStorage();
				var merchCount = Tools.SetIntoRange(model.Ship.MaxCargoLoad / 3, 1, 4);
				for (var i = 0; i < merchCount; i++)
				{
					model.CurrentCargo[(Merchandise) RNG.GetDiceZero()] += RNG.GetDiceDiv2();
				}				
			}

			return model;
		}
	}	
}

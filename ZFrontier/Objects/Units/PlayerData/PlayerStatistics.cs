namespace ZFrontier.Objects.Units.PlayerData
{
	using System.Collections.Generic;
	using GameData;
	using ZLinq;


	public class PlayerStatistics
	{
		public Dictionary<string, int>		ShipDestroyed	{ get; set; }
		public Dictionary<NPC_Type, int>	NPC_Defeated	{ get; set; }

		public bool		IllegalGoodsFound	{ get; set; }
		public bool		AttackedTrader		{ get; set; }
		public bool		AttackedPolice		{ get; set; }
		public bool		KilledTrader		{ get { return NPC_Defeated[NPC_Type.Trader] > 0; }}
		public bool		KilledPolice		{ get { return NPC_Defeated[NPC_Type.Police] > 0; }}

		public int		FinesPaid			{ get; set; }
		public int		DamageInflicted		{ get; set; }
		public int		DamageTaken			{ get; set; }

		public int		MissilesUsed		{ get; set; }
		public int		AsteroidsMined		{ get; set; }


		public PlayerStatistics()
		{
			NPC_Defeated	= Enums.All_NPC_Types.ToDictionary(npcType => npcType, type => 0);
			ShipDestroyed	= GameConfig.ShipModels.ToDictionary(ship => ship.ModelName, type => 0);
		}
	}
}

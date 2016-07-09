namespace ZFrontier.Logic.UI
{
	using Common;
	using Objects.GameData;
	using Objects.Units;
	using ZConsole;


	public class BattleStats : AreaUI_Basic
	{
		#region Private Fields & Constructor

		private readonly StatsArea enemyCoord;
		private readonly StatsArea playerCoord;

		
		public BattleStats(Rect areaRect) : base(areaRect)
		{
			AreaType = AreaUI.PlayerStats;
			const int playerOffset = 49;
			enemyCoord = new StatsArea(areaRect.Left+2, areaRect.Top+1, areaRect.Left+14, areaRect.Left+23);
			playerCoord = new StatsArea(areaRect.Left+playerOffset, areaRect.Top+1, areaRect.Left+12+playerOffset, areaRect.Left+21+playerOffset);
		}
		
		#endregion

		
		public void		Draw_NPC_Stats(NPC_Model npc, PlayerModel player)
		{
			ZOutput.Print(AreaRect.Left+2, AreaRect.Top+1, Lang["Stats_Enemy"], Color.Red);
			CommonMethods.Draw_Stat(enemyCoord, 1, Lang["Stats_Name"],		npc.Name);
			CommonMethods.Draw_Stat(enemyCoord, 2, Lang["Stats_ShipModel"],	npc.Ship.ModelName);

			if (player.Scanner == EquipmentState.Yes)
			{
				CommonMethods.Draw_Stat(enemyCoord, 3, Lang["Stats_ShipState"],	ZIOX.Draw_State, npc.CurrentHP, npc.MaxHP);
				CommonMethods.Draw_Stat(enemyCoord, 4, Lang["Stats_Attack"],	npc.Attack);
				CommonMethods.Draw_Stat(enemyCoord, 5, Lang["Stats_Defense"],	npc.Defense);
				CommonMethods.Draw_Stat(enemyCoord, 6, Lang["Stats_Missiles"],	ZIOX.Draw_State,	npc.CurrentMissiles, npc.MaxMissiles);
				CommonMethods.Draw_Stat(enemyCoord, 7, Lang["Stats_ECM"],		npc.IsRelevealedECM ? (Lang["EquipmentState_"] + npc.ECM) : Lang["Common_Unknown"]);
				if (npc.Bounty > 0)
					CommonMethods.Draw_Stat(enemyCoord, 9, Lang["Stats_Bounty"],	ZIOX.Draw_Currency, npc.Bounty);
			}			
		}

		public void		Draw_Player_Stats(PlayerModel player)
		{
			ZOutput.Print(playerCoord.Left, playerCoord.Top, Lang["Stats_Player"], Color.Red);
			CommonMethods.Draw_Stat(playerCoord, 1, Lang["Stats_Name"],		player.Name);
			CommonMethods.Draw_Stat(playerCoord, 2, Lang["Stats_ShipModel"],player.Ship.ModelName);
			CommonMethods.Draw_Stat(playerCoord, 3, Lang["Stats_ShipState"],ZIOX.Draw_State, player.CurrentHP, player.MaxHP);
			CommonMethods.Draw_Stat(playerCoord, 4, Lang["Stats_Attack"],	player.Attack);
			CommonMethods.Draw_Stat(playerCoord, 5, Lang["Stats_Defense"],	player.Defense);
			CommonMethods.Draw_Stat(playerCoord, 6, Lang["Stats_Missiles"],	ZIOX.Draw_State, player.CurrentMissiles, player.MaxMissiles);
			CommonMethods.Draw_Stat(playerCoord, 7, Lang["Stats_ECM"],		Lang["EquipmentState_" + player.ECM]);
		}
	}
}

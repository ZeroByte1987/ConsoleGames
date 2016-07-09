namespace ZFrontier.Logic.UI
{
	using Common;
	using Objects.GameData;
	using Objects.Units;
	using ZConsole;


	public class PlayerStats : AreaUI_Basic
	{
		#region Private Fields & Constructor

		private readonly StatsArea	coord;
		private PlayerModel			Player		{	get {	return ZFrontier.Player;	}}
		private GalaxyMap			GalaxyMap	{	get {	return ZFrontier.GalaxyMap;	}}


		public PlayerStats(Rect areaRect) : base(areaRect)
		{
			AreaType = AreaUI.PlayerStats;
			coord = new StatsArea(areaRect.Left+2, areaRect.Top+1, areaRect.Left+16, areaRect.Right);
		}

		#endregion

		
		public void		Draw_PlayerStats()
		{
			ZColors.SetBackColor(Color.Black);
			CommonMethods.Draw_Stat(coord,  0, Lang["Stats_Name"],			Player.Name);
			CommonMethods.Draw_Stat(coord,  1, Lang["Stats_Credits"],		ZIOX.Draw_Currency,	Player.Credits);
			CommonMethods.Draw_Stat(coord,  2, Lang["Stats_Fuel"],			ZIOX.Draw_State, Player.FuelLeft, GameConfig.FuelMax);
			CommonMethods.Draw_Stat(coord,  4, Lang["Stats_ShipModel"],		Player.Ship.ModelName);		// +1
			CommonMethods.Draw_Stat(coord,  5, Lang["Stats_ShipState"],		ZIOX.Draw_State,	Player.CurrentHP, Player.MaxHP);
			CommonMethods.Draw_Stat(coord,  6, Lang["Stats_Attack"],		Player.Attack);
			CommonMethods.Draw_Stat(coord,  7, Lang["Stats_Defense"],		Player.Defense);
			CommonMethods.Draw_Stat(coord,  8, Lang["Stats_Missiles"],		ZIOX.Draw_State,	Player.CurrentMissiles, Player.MaxMissiles);
			CommonMethods.Draw_Stat(coord,  9, Lang["Stats_ECM"],			Lang["EquipmentState_" + Player.ECM]);
			CommonMethods.Draw_Stat(coord, 10, Lang["Stats_Scanner"],		Lang["EquipmentState_" + Player.Scanner]);
			CommonMethods.Draw_Stat(coord, 11, Lang["Stats_MiningLaser"],	Lang["EquipmentState_" + Player.MiningLaser]);
			CommonMethods.Draw_Stat(coord, 12, Lang["Stats_EscapeBoat"],	Lang["EquipmentState_" + Player.EscapeBoat]);

			CommonMethods.Draw_Stat(coord, 14, Lang["Stats_Cargo"],			ZIOX.Draw_CargoLoad, Player.CurrentCargo.CurrentLoad, Player.MaxCargoLoad);
			for (var i = 0; i < Enums.All_Merchandise.Count; i++)
			{
				var t = (Merchandise)i;
				CommonMethods.Draw_Merchandise(coord, 15+i, Enums.Get_Name(t), Player.CurrentCargo[t]);
			}

			ZOutput.Print(ZFrontier.xControls+1, ZFrontier.yReleaseInfo+1, Lang["Stats_GameDate"], Color.Magenta);
			ZOutput.Print(ZFrontier.xControls+10, ZFrontier.yReleaseInfo+1, ':', Color.Gray);
			ZIOX.Draw_Date(ZFrontier.xControls+12, ZFrontier.yReleaseInfo+1, ZFrontier.Galaxy.GameDate);

			GalaxyMap.Draw_CurrentSystemInfo();
		}	
	}
}

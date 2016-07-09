namespace ZFrontier.Logic.UI.Windows
{
	using Common;
	using Objects.GameData;
	using Objects.Units;
	using Objects.Units.PlayerData;
	using ZConsole;


	public static class PlayerInfo
	{
		#region Private Fields

		private static PlayerModel		Player		{	get {	return ZFrontier.Player;			}}
		private static PlayerStatistics	Stats		{	get {	return ZFrontier.Player.Statistics;	}}
		private static TranslationSet	Lang		{	get {	return GameConfig.Lang;				}}

		private static readonly Rect	TableRect		= new Rect(13,   9,108, 39);

		private static readonly Rect	MissionsCell	= new Rect( 0,   0, 95, 18);
		private static readonly Rect	KillsCell		= new Rect( 0,  18, 95, 30);
		private static readonly Rect	StatusesCell	= new Rect( 61, 18, 95, 30);

		private const Color	BackColor	= Color.DarkBlue;
		private const Color	HeaderColor	= Color.Yellow;

		#endregion
		
		
		public static void		Show()
		{
			ZBuffer.ReadBuffer("Window", TableRect);
			
			DrawTable();
			DrawMissions();
			DrawMiscStats();

			ZInput.ReadKey();
			ZBuffer.WriteBuffer("Window", TableRect.Left, TableRect.Top);
			ZColors.SetBackColor(Color.Black);
		}
		

		private static void		DrawTable()
		{
			ZTable.DrawTable(TableRect.Left, TableRect.Top, new ZTable.Table(TableRect.Width, TableRect.Height) 
				{
					Caption = "Table",
					Borders = new ZTable.FrameBorders(FrameType.Double),
					BorderColors = new ZCharAttribute(Color.Cyan, BackColor),
					FillColors = new ZCharAttribute(Color.Cyan, BackColor), 
					Cells = new []
						{
							new ZTable.Cell(MissionsCell),
							new ZTable.Cell(KillsCell),
							new ZTable.Cell(StatusesCell)
						}
				});
		}


		private static void		DrawMissions()
		{
			var top  = TableRect.Top  + MissionsCell.Top  + 3;
			var left = TableRect.Left + MissionsCell.Left + 2;
			ZColors.SetBackColor(BackColor);
			ZOutput.Print(left, top-2, Lang["Statistics_MissionsHeader"], HeaderColor);
			
			for (var i = 0; i < Player.Missions.Count; i++)
			{
				ZColors.SetColor(Color.White);
				var mission = Player.Missions[i];
				var targetNPC = mission.AssassinationTarget;
				var topCurrent = Player.Missions.Count > 8 ? top-1+i :top + i*2;
				ZOutput.Print(left,		topCurrent, Enums.Get_Name(mission.Type));
				ZOutput.Print(left+19,	topCurrent, mission.ClientName);
				ZIOX.Draw_Currency(	left+32,	topCurrent, mission.RewardAmount);
				ZOutput.Print(left+38,	topCurrent, mission.TargetStarSystem.Name, Color.White);
				ZOutput.Print(left+49,	topCurrent, (int)(mission.Date - ZFrontier.Galaxy.GameDate).TotalDays);
				ZOutput.Print(left+52,	topCurrent, Lang["Statistics_Days"], Color.Gray);
				ZColors.SetColor(Color.White);

				var detailsText = Lang["MissionDetails_" + mission.Type];
				switch (mission.Type)
				{
					case MissionType.GoodsDelivery:	detailsText = string.Format(detailsText, mission.GoodsToDeliver_Amount, Enums.Get_Name(mission.GoodsToDeliver_Type));	break;
					case MissionType.Assassination:	
					case MissionType.Military_Assassination:		
						detailsText = string.Format(detailsText, Enums.Get_Name(targetNPC.NPC_Type), targetNPC.Name, targetNPC.Ship.ModelName); break;
				}

				ZOutput.PrintBB(left+61, topCurrent, detailsText, Color.White, Color.Yellow, Color.DarkGray, BackColor);
			}
		}


		private static void		DrawMiscStats()
		{
			var top  = TableRect.Top  + KillsCell.Top  + 2;
			var left = TableRect.Left + KillsCell.Left + 2;
			var leftNPC = left+19;
			var leftMisc = left+36;
			var leftLegal = left+62;
			ZColors.SetBackColor(BackColor);
			
			ZOutput.Print(left, top-1, Lang["Statistics_ShipsDestroyed"], HeaderColor);
			var index = 0;
			var statsArea = new StatsArea(left, top, left + 11, left+15);
			foreach (var ship in Stats.ShipDestroyed)
				CommonMethods.Draw_Stat(statsArea, index++, ship.Key, ship.Value);

			ZOutput.Print(leftNPC, top-1, Lang["Statistics_NPC_Killed"], HeaderColor);
			index = 0;
			statsArea = new StatsArea(leftNPC, top, leftNPC + 11, leftNPC+23);
			foreach (var ship in Stats.NPC_Defeated)
				CommonMethods.Draw_Stat(statsArea, index++, Enums.Get_Name(ship.Key), ship.Value);

			ZOutput.Print(leftMisc, top-1, Lang["Statistics_Miscellaneous"], HeaderColor);
			statsArea = new StatsArea(leftMisc, top, leftMisc + 18, leftMisc+22);
			CommonMethods.Draw_Stat(statsArea, 0, Lang["Statistics_DamageInflicted"],	ZIOX.Draw_Mass,		Stats.DamageInflicted);
			CommonMethods.Draw_Stat(statsArea, 1, Lang["Statistics_DamageTaken"],		ZIOX.Draw_Mass, 	Stats.DamageTaken);
			CommonMethods.Draw_Stat(statsArea, 2, Lang["Statistics_FinesPaid"],			ZIOX.Draw_Currency, Stats.FinesPaid);
			CommonMethods.Draw_Stat(statsArea, 3, Lang["Statistics_MissilesUsed"],		Stats.MissilesUsed);
			CommonMethods.Draw_Stat(statsArea, 4, Lang["Statistics_AsteroidMined"],		Stats.AsteroidsMined);

			ZOutput.Print(leftLegal, top-1, Lang["Statistics_LegalStatus"], HeaderColor);
			index = 0;
			statsArea = new StatsArea(leftLegal, top, leftLegal + 15, leftLegal+30);
			foreach (var record in Player.LegalRecords)
				CommonMethods.Draw_Stat(statsArea, index++, Enums.Get_Name(record.Key), Enums.Get_Name(record.Value.Status));

			ZOutput.Print(leftLegal, top+4, Lang["Statistics_MilitaryRank"], HeaderColor);
			index = 5;
			foreach (var record in Player.MilitaryRanks)
				CommonMethods.Draw_Stat(statsArea, index++, Enums.Get_Name(record.Key), Enums.Get_Name(record.Value.Rank, record.Key));

			CommonMethods.Draw_Stat(statsArea, 8, Lang["Statistics_CombatRating"],	Enums.Get_Name(Player.CombatRank));
			CommonMethods.Draw_Stat(statsArea, 9, Lang["Statistics_Reputation"],	Enums.Get_Name(Player.Reputation));
		}
	}
}

namespace ZFrontier.Objects.Galaxy
{
	using System;
	using System.Collections.Generic;
	using GameData;
	using Logic;
	using ZConsole;
	using ZLinq;


	public class StarSystemModel
	{
		#region Public Properties

		public string			Name			{ get; set; }
		public Coord			Coords			{ get; set; }
		public int				TechLevel		{ get; set; }
		public bool				IsExplored		{ get; set; }
		public Allegiance		Allegiance		{ get; set; }
		public GlobalEventType	CurrentEvent	{ get; set; }
		public int				EventDuration	{ get; set; }
        public DateTime         EventEndDate	{ get; set; }
		public List<Merchandise> LegalGoods		{ get; set; }
		public List<Merchandise> IllegalGoods	{ get; set; }
		public int[]			PriceChanges	{ get; set; }

		#endregion

		#region Constructors
		
		public StarSystemModel()
		{
			Name			= GameConfig.Get_StarSystem_Name();
			IllegalGoods	= new List<Merchandise>();
			LegalGoods		= new List<Merchandise>();
			PriceChanges	= new int[Enums.All_Merchandise.Count];
			TechLevel		= RNG.GetDice();
			IsExplored		= GameConfig.GalaxyIsExplored;
		}

		#endregion


		public static StarSystemModel	CreateRandom(Allegiance allegiance)
		{
			var independentBonus = allegiance == Allegiance.Independent ? GameConfig.IndependentIllegalBonus : 0;
			var model = new StarSystemModel();
			if (RNG.GetDice() + independentBonus <= GameConfig.GoodIsIllegalChance-1)	model.IllegalGoods.Add(Merchandise.Luxury);
			if (RNG.GetDice() + independentBonus <= GameConfig.GoodIsIllegalChance-1)	model.IllegalGoods.Add(Merchandise.Drugs);
			if (RNG.GetDice() + independentBonus <= GameConfig.GoodIsIllegalChance)		model.IllegalGoods.Add(Merchandise.Weapons);
			if (RNG.GetDice() + independentBonus <= GameConfig.GoodIsIllegalChance)		model.IllegalGoods.Add(Merchandise.Slaves);
			model.LegalGoods.AddRange(Enums.All_Merchandise.Where(a => !model.IllegalGoods.Contains(a)));

			model.Allegiance = allegiance;
			return model;
		}
	}
}

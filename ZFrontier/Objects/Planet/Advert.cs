namespace ZFrontier.Objects.Planet
{
	using Galaxy;
	using GameData;
	using Logic;
	using Units;
	using Units.PlayerData;


	public class Advert
	{
		#region Public Properties

		public AdvertType	Type		{ get; set; }
		public string		Caption		{ get; set; }
		public string		OwnerName	{ get; set; }
		public Merchandise	Merchandise	{ get; set; }
		public int			Price		{ get; set; }
		public bool			IsTrap		{ get; set; }
		public string		CallText	{ get; set; }
		public MissionModel	Mission		{ get; set; }

		#endregion


		public static Advert Create(AdvertType advertType, StarSystemModel starSystem, PlayerModel player, bool isMilitary)
		{
			if (advertType == AdvertType.BuyIllegal  &&  starSystem.IllegalGoods.Count == 0)
				advertType = AdvertType.PackageDelivery;

			var model = new Advert
				{
					Type		= advertType,
					Caption		= (isMilitary ? advertType.ToString() : "BBC_Advert" + advertType) + "Caption",
					CallText	= (isMilitary ? advertType.ToString() : "BBC_Advert" + advertType) + "Text",
					OwnerName	= isMilitary ? ZFrontier.Lang["MilitaryHQ_Officer"] : GameConfig.Get_NPC_Name(NPC_Type.Trader),
				};

			switch (advertType)
			{
				case AdvertType.PackageDelivery:	model.Mission = MissionModel.CreateRandom(MissionType.PackageDelivery);	break;
				case AdvertType.GoodsDelivery:		model.Mission = MissionModel.CreateRandom(MissionType.GoodsDelivery);	break;
				case AdvertType.Passenger:			model.Mission = MissionModel.CreateRandom(MissionType.Passenger);		break;
				case AdvertType.Assassination:		model.Mission = MissionModel.CreateRandom(MissionType.Assassination);	break;
				case AdvertType.Military_Delivery:		model.Mission = MissionModel.CreateRandom(MissionType.Military_Delivery,      true, starSystem.Allegiance);	break;
				case AdvertType.Military_Assassination:	model.Mission = MissionModel.CreateRandom(MissionType.Military_Assassination, true, starSystem.Allegiance);	break;
			}

			if (advertType == AdvertType.BuyIllegal)
			{
				var merch = starSystem.IllegalGoods.Get_Random();
				model.Merchandise = merch;
				model.IsTrap = RNG.GetDice() <= GameConfig.AdvertTrapChance;
				model.Price = GameConfig.Get_MerchPrice(starSystem.TechLevel, merch) + starSystem.PriceChanges[(int) (merch)];
				model.Price += RNG.GetNumber(1, GameConfig.AdvertIllegalPriceBonus);
			}
			else
			{
				model.Merchandise = model.Mission.GoodsToDeliver_Type;
				model.Price = model.Mission.RewardAmount;
			}
			
			return model;
		}
	}	
}

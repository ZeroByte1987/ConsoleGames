namespace ZFrontier.Objects.Galaxy
{
	using System;
	using System.Collections.Generic;
	using GameData;
	using Logic;
	using ZConsole;
	using ZLinq;


	public class GalaxyModel
	{
		#region Public Properties

		public StarSystemModel[,]	StarSystems		{ get; set; }
		public DateTime				GameDate		{ get; set; }
		public DateTime				LastEventDate	{ get; set; }
		public List<GlobalEvent>	GlobalEventLog	{ get; set; }
		public int					RandomSeed		{ get; set; }
		public AlienStrength		AlienStrength	{ get; set; }

		#endregion

		#region Constructors
	
		public GalaxyModel()
		{
			RandomSeed		= RNG.GetSeed();
			GameDate		= GameConfig.GameStartDate;
			LastEventDate	= GameConfig.GameStartDate;
			GlobalEventLog	= new List<GlobalEvent>();
		}

		#endregion

		#region Public Properties

		public static GalaxyModel	    Create(int galaxySizeX, int galaxySizeY)
		{
			var maxNumberForAllegiance = galaxySizeX*galaxySizeY*4/10;
			var counts = new int[Enums.All_Allegiances.Count];

			var model = new GalaxyModel { StarSystems = new StarSystemModel[galaxySizeY,galaxySizeX] };
			for (var i = 0; i < galaxySizeY; i++)
				for (var j = 0; j < galaxySizeX; j++)
				{
					int allegianceIndex;
					do
					{
						allegianceIndex = RNG.GetNumber(Enums.All_Allegiances.Count);
					} while (counts[allegianceIndex] >= maxNumberForAllegiance);

					model.StarSystems[i, j] = StarSystemModel.CreateRandom((Allegiance)allegianceIndex);
					model.StarSystems[i, j].Coords = new Coord(j, i);
					counts[allegianceIndex]++;
				}
			return model;
		}

		public StarSystemModel          Get_RandomSystemForEvent(GlobalEventType @event)
	    {
			var allSystemsNormal = Get_AllSystems().Where(a => a.CurrentEvent == GlobalEventType.Normal).ToArray();
			switch (@event)
			{
				case GlobalEventType.AlienInvasion:	return allSystemsNormal.Where(a => a.Name != StarSystems[ZFrontier.Player.PosY, ZFrontier.Player.PosX].Name).GetRandom();
                case GlobalEventType.LevelUp	:	return allSystemsNormal.Where(a => a.TechLevel < GameConfig.TechLevelRange.Max).GetRandom();
				case GlobalEventType.LevelDown	:	return allSystemsNormal.Where(a => a.TechLevel > GameConfig.TechLevelRange.Min).GetRandom();
                case GlobalEventType.IllegalAdd	:	return allSystemsNormal.Where(a => a.IllegalGoods.Count < Enums.All_MerchandiseIllegal.Count).GetRandom();
				case GlobalEventType.IllegalRemove:	return allSystemsNormal.Where(a => a.IllegalGoods.Count > 0).GetRandom();
				default:						return allSystemsNormal.GetRandom();
			}
	    }
		
        public void                     Update_AllSystems()
	    {
	        for (var i = 0; i < GameConfig.CurrentGalaxySizeY; i++)
	            for (var j = 0; j < GameConfig.CurrentGalaxySizeX; j++)
	            {
	                var system = StarSystems[i, j];
	                if (system.CurrentEvent != GlobalEventType.Normal  &&  system.CurrentEvent != GlobalEventType.AlienInvasion  &&  system.EventDuration >= 0)
	                {
	                    system.EventDuration = (system.EventEndDate - GameDate).Days;
	                    if (system.EventDuration < 0)
	                    {
	                        system.CurrentEvent = GlobalEventType.Normal;
	                        system.EventDuration = 0;
	                    }
	                }
	            }
	    }

		public List<StarSystemModel>	Get_AllSystemsWithEvents()
		{
			return Get_AllSystems().Where(a => a.CurrentEvent != GlobalEventType.Normal).ToList();
		}

		public IEnumerable<StarSystemModel> Get_AllSystems()
	    {
	        var result = new List<StarSystemModel>();
	        for (var i = 0; i < GameConfig.CurrentGalaxySizeY; i++)
	            for (var j = 0; j < GameConfig.CurrentGalaxySizeX; j++)
                    result.Add(StarSystems[i,j]);
	        return result;
	    }

		#endregion
	}
}

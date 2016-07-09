namespace ZFrontier.Objects.Units
{
	using GameData;
	using Logic;
	using PlayerData;


	public class PlayerModel : BasicUnitModel
	{
		#region Public Properties
		
		public int				PosX			{ get; set; }
		public int				PosY			{ get; set; }
		
		public int				Credits			{ get; set; }
		public int				FuelLeft		{ get; set; }
		public bool				IsLanded		{ get; set; }

		public EquipmentState	MiningLaser		{ get; set; }
		public EquipmentState	Scanner			{ get; set; }
		public EquipmentState	EscapeBoat		{ get; set; }

		public MissionList		Missions		{ get; set; }
		public LegalRecords		LegalRecords	{ get; set; }
		public MilitaryRecords	MilitaryRanks	{ get; set; }
		public PlayerStatistics	Statistics		{ get; set; }

		public int				CombatRating		{ get; set; }
		public int				ReputationRating	{ get; set; }
		public CombatRating		CombatRank			{ get {	return Enums.Get_Value<CombatRating>(CombatRating);		}}
		public Reputation		Reputation			{ get {	return Enums.Get_Value<Reputation>(ReputationRating);	}}

		#endregion

		#region Constructor
	
		internal PlayerModel()
		{
			CurrentCargo = new CargoStorage();
			Missions	= new MissionList();
			Statistics	= new PlayerStatistics();
			
			LegalRecords = new LegalRecords();
			foreach (var allegiance in Enums.All_Allegiances)
				LegalRecords.Add(allegiance, new LegalRecord());

			MilitaryRanks = new MilitaryRecords { { Allegiance.Alliance, new MilitaryRecord() }, { Allegiance.Empire, new MilitaryRecord() } };
			IsPlayer = true;
		}

		#endregion

		#region Public Methods

		public static PlayerModel	Create()
		{
			var defaultPlayer = GameConfig.DefaultPlayers[GameConfig.CurrentDifficulty];
			return new PlayerModel
			{
				Name			= string.Empty,
				PosX			= RNG.GetNumber(GameConfig.CurrentGalaxySizeX),
				PosY			= RNG.GetNumber(GameConfig.CurrentGalaxySizeY),
				Ship			= defaultPlayer.Ship,
				Credits			= defaultPlayer.Credits,
				Attack			= defaultPlayer.Attack,
				Defense			= defaultPlayer.Defense,
				ECM				= defaultPlayer.ECM,
				Scanner			= defaultPlayer.Scanner,
				MiningLaser		= defaultPlayer.MiningLaser,
				EscapeBoat		= defaultPlayer.EscapeBoat,
				CurrentHP		= defaultPlayer.MaxHP,
				CurrentMissiles = defaultPlayer.MaxMissiles,
				CurrentCargo	= defaultPlayer.CurrentCargo.Copy(),
				LegalRecords	= defaultPlayer.LegalRecords.Copy(),
				MilitaryRanks	= defaultPlayer.MilitaryRanks.Copy(),
				CombatRating	= defaultPlayer.CombatRating,
				ReputationRating	= defaultPlayer.ReputationRating,
				FuelLeft		= GameConfig.FuelMax,
				IsLanded		= true
			};
		}
		
		
		public PlayerModel			Copy()
		{
			return new PlayerModel
				{
					Attack			= Attack,
					Defense			= Defense,
					Credits			= Credits,
					CurrentCargo	= CurrentCargo.Copy(),
					CurrentHP		= CurrentHP,
					CurrentMissiles = CurrentMissiles,
					FuelLeft		= FuelLeft,
					ECM				= ECM,
					EscapeBoat		= EscapeBoat,
					MiningLaser		= MiningLaser,
					Scanner			= Scanner,
					IsLanded		= IsLanded,
					IsPlayer		= IsPlayer,
					LegalRecords	= LegalRecords.Copy(),
					MilitaryRanks	= MilitaryRanks.Copy(),
					Name			= Name,
					PosX			= PosX,
					PosY			= PosY,
					Ship			= Ship,
					CombatRating = CombatRating,
					ReputationRating = ReputationRating
				};
		}

		#endregion
	}
}

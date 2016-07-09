namespace ZFrontier.Objects.Units
{
	using GameData;
	using PlayerData;


	public class BasicUnitModel
	{
		public ShipModel		Ship			{ get; set; }
		public string			Name			{ get; set; }

		public int				Attack			{ get; set; }
		public int				Defense			{ get; set; }

		public int				MaxHP			{ get { return Ship.MaxHP; }}
		public int				CurrentHP		{ get; set; }

		public int				MaxMissiles		{ get { return Ship.MaxMissiles; }}
		public int				CurrentMissiles	{ get; set; }

		public EquipmentState	ECM				{ get; set; }

		public int				MaxCargoLoad	{ get { return Ship.MaxCargoLoad; }}
		public CargoStorage		CurrentCargo	{ get; set; }

		public bool				IsImmuneToCrits	{ get; set; }
		public bool				IsPlayer		{ get; set; }
		public bool				IsDead			{ get { return CurrentHP <= 0; }}
	}
}

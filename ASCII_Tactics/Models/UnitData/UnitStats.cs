namespace ASCII_Tactics.Models.UnitData
{
	public sealed class UnitStats
	{
		public int			MaxHP			{ get; set; }
		public int			CurrentHP		{ get; set; }
	
		public int			TU				{ get; set; }
		public int			MaxTU			{ get { return TU/2 + (TU/2)*(CurrentHP/MaxHP); }}
		public int			CurrentTU		{ get; set; }

		public int			Accuracy		{ get; set; }
		public int			CurrentAccuracy	{ get { return Accuracy/2 + (Accuracy/2)*(CurrentHP/MaxHP); }}
		
		public int			Strength		{ get; set; }
		public int			CurrentStrength	{ get { return Strength/2 + (Strength/2)*(CurrentHP/MaxHP); }}
		public int			MaxWeight		{ get { return CurrentStrength * 2; }}
	}
}
namespace ASCII_Tactics.Models.UnitData
{
	public sealed class ViewStats
	{
		public int	Distance	{ get; set; }
		public int	Angle		{ get; set; }
		public int	Direction	{ get; set; }


		public ViewStats(int distance, int angle, int direction)
		{
			Distance	= distance;
			Angle		= angle;
			Direction	= direction;
		}
	}
}
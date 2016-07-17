namespace ASCII_Tactics.Models.UnitData
{
	using ZConsole;


	public class Position : Coord
	{
		public int		LevelId		{ get; set; }
		public bool		IsSitting	{ get; set; }


		public Position(int levelId, int xPosition, int yPosition) : base(xPosition, yPosition)
		{
			LevelId = levelId;
		}
	}
}
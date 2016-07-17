namespace ASCII_Tactics.Models.Map
{
	using ZConsole;
	

	public class ActiveObject
	{
		public int		LevelId		{ get; set; }
		public Coord	Coord		{ get; set; }

		public ActiveObject(int levelId, Coord coord)
		{
			LevelId = levelId;
			Coord	= coord;
		}
	}
}
namespace ASCII_Tactics.Models.Map
{
	using ZConsole;


	public class Door : ActiveObject
	{
		public bool		IsOpened	{ get; set; }
		public bool		IsKeyNeeded	{ get; set; }
		public Color	KeyColor	{ get; set; }

		public Door(int levelId, Coord coord, bool isKeyNeeded = false, Color keyColor = Color.Black) : base(levelId, coord)
		{
			IsKeyNeeded	= isKeyNeeded;
			KeyColor	= keyColor;
		}
	}
}
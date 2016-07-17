namespace ASCII_Tactics.Models.Map
{
	using System.Collections.Generic;
	using ZConsole;


	public class Locker : ActiveObject
	{
		public bool			IsOpened	{ get; set; }
		public bool			IsKeyNeeded	{ get; set; }
		public Color		KeyColor	{ get; set; }
		public List<Item>	Items		{ get; set; }

		public Locker(int levelId, Coord coord, bool isKeyNeeded = false, Color keyColor = Color.Black) : base(levelId, coord)
		{
			IsKeyNeeded	= isKeyNeeded;
			KeyColor	= keyColor;
		}
	}
}
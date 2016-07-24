namespace ASCII_Tactics.Models.Map
{
	using System.Collections.Generic;
	using CommonEnums;
	using Tiles;
	using ZConsole;


	public class Level
	{
		public int			Id		{ get; set; }
		public Size			Size	{ get; set; }
		public Tile[,]		Map		{ get; set; }
		public List<Room>	Rooms	{ get; set; }
		public Corner		StairsDownLocation { get; set; }


		public Level(int id, Size size)
		{
			Id		= id;
			Size	= size;
			Map		= new Tile[size.Height, size.Width];
			Rooms	= new List<Room>();
			StairsDownLocation = Corner.None;
		}
	}
}
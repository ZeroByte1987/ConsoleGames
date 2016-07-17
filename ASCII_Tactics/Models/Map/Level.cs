namespace ASCII_Tactics.Models.Map
{
	using Tiles;
	using ZConsole;


	public class Level
	{
		public int			Id		{ get; set; }
		public Size			Size	{ get; set; }
		public Tile[,]		Map		{ get; set; }

		public Level(int id, Size size)
		{
			Id		= id;
			Size	= size;
			Map		= new Tile[size.Height, size.Width];			
		}
	}
}
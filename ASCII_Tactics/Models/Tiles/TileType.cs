namespace ASCII_Tactics.Models.Tiles
{
	using CommonEnums;
	using ZConsole;


	public class TileType
	{
		public int			Id				{ get; set; }
		public string		Name			{ get; set; }
		public char			Character		{ get; set; }
		public Color		ForeColor		{ get; set; }
		public Color		BackColor		{ get; set; }
		
		public TileRole		Role			{ get; set; }
		public ObjectHeight	Height			{ get; set; }
		public ObjectSize	Size			{ get; set; }
		public bool			IsPassable		{ get; set; }
		public bool			IsDestructible	{ get; set; }
		public int			Durability		{ get; set; }


		public TileType(int id, string name, TileRole role, 
			char character, Color foreColor, Color backColor,
			ObjectSize size, ObjectHeight height,
			bool isPassable, bool isDestructible = false, int durability = 0)
		{
			Id				= id;
			Name			= name;
			Character		= character;
			ForeColor		= foreColor;
			BackColor		= backColor;
			Role			= role;
			Size			= size;
			Height			= height;
			IsPassable		= isPassable;
			IsDestructible	= isDestructible;
			Durability		= durability;
		}
	}
}
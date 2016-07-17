namespace ASCII_Tactics.Models.Tiles
{
	using Config;
	using Logic.Extensions;


	public class Tile
	{
		public TileType	Type		{ get; set; }
		public int		CurrentHP	{ get; set; }


		public Tile(TileType type, bool isCustomHP = false, int currentHP = 0)
		{
			Type = type;
			CurrentHP = isCustomHP ? currentHP : type.Durability;
		}

		public Tile(string tileName, bool isCustomHP = false, int currentHP = 0)
			: this(MapConfig.TileSet.Get(tileName), isCustomHP, currentHP)
		{
		}
	}
}
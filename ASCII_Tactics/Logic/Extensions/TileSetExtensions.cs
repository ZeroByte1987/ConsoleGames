namespace ASCII_Tactics.Logic.Extensions
{
	using System.Collections.Generic;
	using Models.Tiles;
	using ZLinq;


	public static class TileSetExtensions
	{
		public static TileType Get(this List<TileType> source, string name)
		{ 
			return source.Single(w => w.Name == name); 
		}
	}
}
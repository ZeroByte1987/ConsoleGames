namespace ASCII_Tactics.Config
{
	using System.Collections.Generic;
	using Models.CommonEnums;
	using Models.Tiles;
	using ZConsole;


	public static class MapConfig
	{
		public static Size	LevelSize			= new Size(92, 62);

		public static Range	LevelCount			= new Range(8, 12);
		public static Range	RoomCount			= new Range(6, 9);
		public static Range	RoomSize			= new Range(10, 25);
		public static Range	FurnitureCount		= new Range(0, 6);
		public static Range	ItemsPerLevelCount	= new Range(2, 6);
		public static Range	DoorCountPerRoom	= new Range(1, 2);


		public static List<TileType> TileSet = new List<TileType>
			                                       {
			new TileType(0, "Empty",		TileRole.None, '.',			Color.DarkGray, Color.Black, ObjectSize.Small,		ObjectHeight.None, true),
			new TileType(1, "Wall",			TileRole.None, (char)219,	Color.DarkCyan, Color.Black, ObjectSize.FullTile,	ObjectHeight.Full, false),
			new TileType(2, "ClosedDoor",	TileRole.Door, (char)219,	Color.Cyan,		Color.Black, ObjectSize.FullTile,	ObjectHeight.Full, false),
			new TileType(2, "OpenedDoor",	TileRole.Door, '/',			Color.Cyan,		Color.Black, ObjectSize.FullTile,	ObjectHeight.None, true),
			new TileType(2, "Table",		TileRole.None, (char)177,	Color.DarkCyan,	Color.Black, ObjectSize.FullTile,	ObjectHeight.Half, false),
		};
	}
}
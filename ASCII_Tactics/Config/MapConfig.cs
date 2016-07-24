namespace ASCII_Tactics.Config
{
	using System.Collections.Generic;
	using Models.CommonEnums;
	using Models.Tiles;
	using ZConsole;


	public static class MapConfig
	{
		public static Size	LevelSize			= new Size(92, 62);

		public static Range	LevelCount			= new Range(2, 2);
		public static Range	RoomCount			= new Range(7, 10);
		public static Range	RoomSize			= new Range(12, 25);
		public static Range	FurnitureCount		= new Range(0, 15);
		public static Range	ItemsPerLevelCount	= new Range(2, 6);
		public static Range	DoorCountPerRoom	= new Range(1, 2);


		public static List<TileType> TileSet = new List<TileType>
		{
			new TileType(0, "Empty",		TileRole.None,		'.',		Color.DarkGray, Color.Black,	ObjectSize.Small,		ObjectHeight.None, true),
			new TileType(1, "Wall",			TileRole.None,		(char)219,	Color.DarkCyan, Color.Black,	ObjectSize.FullTile,	ObjectHeight.Full, false),
			new TileType(2, "ClosedDoor",	TileRole.Door,		(char)219,	Color.Cyan,		Color.Black,	ObjectSize.FullTile,	ObjectHeight.Full, false),
			new TileType(3, "OpenedDoor",	TileRole.Door,		'/',		Color.Cyan,		Color.Black,	ObjectSize.FullTile,	ObjectHeight.None, true),
			new TileType(4, "Table",		TileRole.None,		(char)178,	Color.DarkCyan,	Color.Black,	ObjectSize.FullTile,	ObjectHeight.Half, false),
			new TileType(5, "StairsUp",		TileRole.Stairs,	(char)24,	Color.Green,	Color.Black,	ObjectSize.FullTile,	ObjectHeight.None, true),
			new TileType(6, "StairsDown",	TileRole.Stairs,	(char)25,	Color.Green,	Color.Black,	ObjectSize.FullTile,	ObjectHeight.None, true),
			new TileType(6, "FinalTarget",	TileRole.Target,	'!',		Color.Red,		Color.Blue,		ObjectSize.FullTile,	ObjectHeight.Full, false)
		};
	}
}
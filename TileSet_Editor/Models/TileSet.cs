namespace TileSet_Editor.Models
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using ZConsole;
	using ZFC;
	using ZFC.Maths;
	using ZLinq;


	public class TileSet
	{
		#region Private Fields and Constructors

		private string Signature	= "ZTSF";
		private string Version		= "1.00";

		internal List<Tile> _tiles;

		public TileSet(string tileSetName)
		{
			Name = tileSetName;
			ColorDepth = BitsPerColor.Bits_4;
			_tiles = new List<Tile>();
		}

		#endregion

		#region Public Properties

		public List<Tile>	Tiles			{	get { return _tiles; }}

		public string		Name			{ get; set; }

		public BitsPerColor	ColorDepth		{ get; set; }

		/// <summary>
		/// Specifies whether extended (32 bits) options are used.
		/// </summary>
		public bool			ExtendedOptions	{ get; set; }

		#endregion

		#region Public Methods

		#region Add/Remove

		public bool		Contains(int index)
		{
			return _tiles.Any(a => a.Index == index);
		}

		public void		Add(int index, char charValue, Color foreColor, Color backColor, TileOptions options)
		{
			if (!Contains(index))
				Tiles.Add(new Tile(index, charValue, foreColor, backColor, options));
		}

		public void		Add(Tile tile)
		{
			if (!Contains(tile.Index))
				Tiles.Add(tile);
		}

		public void		Add(IEnumerable<Tile> tiles)
		{
			foreach (var tile in tiles)
			{
				if (!Contains(tile.Index))
					_tiles.Add(tile);
			}
		}

		public void		Remove(int tileIndex)
		{
			_tiles.RemoveAll(a => a.Index == tileIndex);
		}

		public void		Remove(char charValue)
		{
			_tiles.RemoveAll(a => a.CharValue == charValue);
		}

		public void		Remove(Predicate<Tile> match)
		{
			_tiles.RemoveAll(match);
		}

		#endregion

		#region Read/Write

		public int				WriteToFile(string fileName)
		{
			var stream = new MemoryStream();
			var writer = new BinaryWriter(stream);
			
			writer.Write(Signature.ToCharArray());			//	0
			writer.Write(Version.ToCharArray());			//	4
			writer.Write((byte)ColorDepth);					//	8
			writer.Write(ZMath.GetByte(ExtendedOptions));	//	9
			writer.Write((byte)8);							//	10, Size of Tile class/structure
			writer.Write((byte)Name.Length);				//	11, Size of Tile class/structure
			writer.Write(_tiles.Count);						//	12
			writer.Write(Name.ToCharArray(), 0, 64);		//	16

			foreach (var tile in _tiles)
				tile.Write(writer);

			var result = ZFile.WriteFile(fileName, stream.ToArray());
			writer.Close();
			return result;
		}

		public static TileSet	ReadFromFile(string fileName)
		{
			var data = ZFile.ReadFile(fileName);
			if (data == null)
				return null;

			var reader = new BinaryReader(new MemoryStream(data));
			var tileSet = new TileSet(null);

			tileSet.Signature		= new string(reader.ReadChars(4));
			tileSet.Version			= new string(reader.ReadChars(4));
			tileSet.ColorDepth		= (BitsPerColor)reader.ReadByte();
			tileSet.ExtendedOptions = ZMath.GetBool(reader.ReadByte());
			var tileDataSize		= reader.ReadByte();		// maybe can be check for some conditions?
			var nameLength			= reader.ReadByte();
			var tilesCount			= reader.ReadInt32();
			tileSet.Name			= new string(reader.ReadChars(64), 0, nameLength);

			for (int i = 0; i < tilesCount; i++)
			{
				try		{	tileSet.Add(new Tile(reader.ReadUInt64()));	}
				catch	{	return null;	}
			}

			return tileSet;
		}

		#endregion

		#endregion
	}
}

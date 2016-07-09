namespace TileSet_Editor.Models
{
	using System;
	using System.IO;
	using ZConsole;
	using ZFC.Data;
	using ZFC.Maths;


	public class Tile
	{
		public short		Index		{ get; set; }
		public char			CharValue	{ get; set; }
		public Color		ForeColor	{ get; set; }
		public Color		BackColor	{ get; set; }
		public TileOptions	Options		{ get; set; }

		public byte[]		ToByteArray()
		{
			var result = new byte[8];
			ZArray.Write_ToArray(ref result, 0, Index);
			ZArray.Write_ToArray(ref result, 2, CharValue);
			ZArray.Write_ToArray(ref result, 4, (byte)ForeColor);
			ZArray.Write_ToArray(ref result, 5, (byte)BackColor);
			ZArray.Write_ToArray(ref result, 6, Options.IntValue);
			return result;
		}

		public void			Write(BinaryWriter writer)
		{
			writer.Write(Index);
			writer.Write(CharValue);
			writer.Write((byte)ForeColor);
			writer.Write((byte)BackColor);
			writer.Write(Options.IntValue);
		}

		public Tile(byte[] data)
		{
			Index		= BitConverter.ToInt16(data, 0);
			CharValue	= BitConverter.ToChar (data, 2);
			ForeColor	= (Color)ZMath.GetBound(data[4], 0, 15);
			BackColor	= (Color)ZMath.GetBound(data[5], 0, 15);
			Options		= new TileOptions(BitConverter.ToInt16(data, 6));
		}

		public Tile(int index, char charValue, Color foreColor, Color backColor, TileOptions options)
		{
			Index		= (short)index;
			CharValue	= charValue;
			ForeColor	= foreColor;
			BackColor	= backColor;
			Options		= options;
		}

		public Tile(ulong ulongValue) : this(BitConverter.GetBytes(ulongValue))
		{}
	}

	public class TileOptions
	{
		private ZBitArray bitArray;

		public TileOptions(int optionsValue)
		{
			bitArray = new ZBitArray(optionsValue);
		}

		public ushort	IntValue		{	get {	return bitArray.ToUShort(); }}

		public bool		IsPassable		{	get {	return bitArray[0]; }	set {	bitArray[0] = value;	}}
		public bool		IsTransparent	{	get {	return bitArray[1]; }	set {	bitArray[1] = value;	}}	
		public bool		IsDestructible	{	get {	return bitArray[2]; }	set {	bitArray[2] = value;	}}	
	}
}

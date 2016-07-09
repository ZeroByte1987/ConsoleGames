using System;
using System.Text;
using System.IO;
using System.IO.Compression;



namespace ZFC
{
	/// <summary>
	/// This class defines the methods for compression/decompression of data.
	/// </summary>
	public class ZCompress
	{
		//	Compress data methods
		#region
		/// <summary>
		/// Compresses binary data with Deflate method.
		/// </summary>
		/// <param name="Data">Byte array with data to compress.</param>
		/// <returns>Returns the result byte array with compressed data if successful, otherwise returns NULL.</returns>
		public static byte[]	CompressData(byte[] Data)
		{
			try
			{
				var MS = new MemoryStream();
				var CS = new DeflateStream(MS, CompressionMode.Compress, true);
				CS.Write(Data, 0, Data.Length);
				CS.Flush();
				CS.Close();
				return MS.ToArray();
			}
			catch	{	return null;	}
		}

		/// <summary>
		/// Decompresses binary data compressed with Deflate method.
		/// </summary>
		/// <param name="Data">Byte array with data to decompress.</param>
		/// <param name="MaxSize">Maximal estimated size of decompressed data (if you're sure about it). This can help to use less memory.</param>
		/// <returns>Byte array with decompressed data if successful, null if failed.</returns>
		public static byte[]	DecompressData(byte[] Data, int MaxSize)
		{
			try
			{
				var DS = new DeflateStream(new MemoryStream(Data), CompressionMode.Decompress);
				var TA = new byte[MaxSize];
				int count = DS.Read(TA, 0, MaxSize);
				var Result = new byte[count];
				Array.Copy(TA, 0, Result, 0, count);
				return Result;
			}
			catch	{	return null;	}
		}

		/// <summary>
		/// Decompresses binary data compressed with Deflate method.
		/// </summary>
		/// <param name="Data">Byte array with data to decompress.</param>
		/// <returns>Byte array with decompressed data if successful, null if failed.</returns>
		public static byte[]	DecompressData(byte[] Data)
		{	
			return DecompressData(Data, Data.Length*20);
		}

		/// <summary>
		/// Compresses text data with Deflate method.
		/// </summary>
		/// <param name="Data">String with text to compress.</param>
		/// <param name="Encoding">Text encoding.</param>
		/// <returns>Byte array with compressed text if successful, null if failed.</returns>
		public static byte[]	CompressString(string Data, Encoding Encoding)
		{	
			return CompressData(Encoding.GetBytes(Data));
			
		}

		/// <summary>
		/// Decompresses text data with Deflate method.
		/// </summary>
		/// <param name="Data">Byte array with data to decompress into string.</param>
		/// /// <param name="Encoding">Text encoding.</param>
		/// <returns>String with decompressed text if successful, null if failed.</returns>
		public static string	DecompressString(byte[] Data, Encoding Encoding)
		{
			try		{	return Encoding.GetString(DecompressData(Data));	}
			catch	{	return null;	}
		}
		#endregion


		//	Read/Write compressed files
		#region
		/// <summary>
		/// Compresses data and writes it into .zzf file (Zero Zipped File)
		/// </summary>
		/// <param name="FileName">Name of result file.</param>
		/// <param name="Data">Byte array with data to compress and write.</param>
		/// <returns>Size of result file if successful, -1 if failed.</returns>
		public static int		WriteCompressedFile(string FileName, byte[] Data)
		{
			try
			{
				var F	= new FileStream(FileName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
				var rw	= new BinaryWriter(F);
				var CD	= CompressData(Data);
				rw.Write((int)0x31465A5A);
				rw.Write((int)Data.Length);
				rw.Write((int)CD.Length);
				rw.Write(CD, 0, CD.Length);
				rw.Close();
				return 12 + CD.Length;
			}
			catch	{	return -1;	}
		}
		
		/// <summary>
		/// Compresses text string and writes it into .zzf file (Zero Zipped File)
		/// </summary>
		/// <param name="FileName">Name of result file.</param>
		/// <param name="Text">String with text data to compress and write.</param>
		/// <param name="Encoding">Encoding of text content.</param>
		/// <returns>Size of result file if successful, -1 if failed.</returns>
		public static int		WriteCompressedTextFile(string FileName, string Text, Encoding Encoding)
		{
			return WriteCompressedFile(FileName, Encoding.GetBytes(Text));
		}


		/// <summary>
		///	Reads and decompresses the compressed data from .zzf file (Zero Zipped File).
		/// </summary>
		/// <param name="FileName">Name of the file to read.</param>
		/// <returns>Byte array with decompressed data if successful, null if failed.</returns>
		public static byte[]	ReadCompressedFile(string FileName)
		{
			try
			{
				var F	= new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
				var rd	= new BinaryReader(F);	
				if (rd.ReadInt32() != 0x31465A5A)	return null;
				int MaxSize = rd.ReadInt32();
				int CDataSize = rd.ReadInt32();
				var Data = DecompressData(rd.ReadBytes(CDataSize), MaxSize);
				rd.Close();
				return Data;
			}
			catch	{	return null;	}
		}

		/// <summary>
		/// Reads and decompresses the compressed text data from .zzf file (Zero Zipped File)
		/// </summary>
		/// <param name="FileName">Name of the file to read.</param>
		/// <param name="Encoding">Encoding of text content.</param>
		/// <returns>String with decompressed text data if successful, null if failed.</returns>
		public static string	ReadCompressedTextFile(string FileName, Encoding Encoding)
		{
			try		{	return Encoding.GetString(ReadCompressedFile(FileName));	}
			catch	{	return null;	}
		}
		#endregion
	}
}

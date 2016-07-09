using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;



namespace ZFC
{
	/// <summary>
	/// This class defines the static methods for work with different data objects.
	/// </summary>
	public class ZData
	{
		internal static MD5		MD5;


		/// <summary>
		/// Swap bytes.
		/// </summary>
		/// <param name="N">Integer value where bytes should be swapped.</param>
		/// <returns>Returns resulting integer value with swapped bytes.</returns>
		public static uint		Swap_Bytes(uint N)
		{
			var aBytes = new Byte[4];
			aBytes[3] = (byte) (N & 0x000000FF);
			aBytes[2] = (byte)((N & 0x0000FF00) >> 8);
			aBytes[1] = (byte)((N & 0x00FF0000) >> 16);
			aBytes[0] = (byte)((N & 0xFF000000) >> 24);
			return BitConverter.ToUInt32(aBytes, 0);
		}
		/// <summary>
		/// Swap bytes.
		/// </summary>
		/// <param name="N">Source byte array</param>
		/// <returns>Returns resulting byte array with swapped bytes.</returns>
		public static byte[]	Swap_Bytes(byte[] N)
		{
			var BA = new byte[N.Length];
			for (int i = 0; i < N.Length; i++)
				BA[i] = N[N.Length-1-i];
			return BA;
		}
		/// <summary>
		/// Swaps the values of two integers.
		/// </summary>
		/// <param name="A">Integer A.</param>
		/// <param name="B">Integer B.</param>
		public static void		Swap_Ints(ref int A, ref int B)
		{
			int C = A;
			A = B;
			B = C;
		}



		/// <summary>
		/// Gets the address of the next page (512 byte).
		/// </summary>
		/// <param name="Address">Current address.</param>
		/// <returns>Returns the address of the next following page.</returns>
		public static int		Get_NextPageAdress(long Address)
		{
			return Get_NextPageAdress(Address, 512);
		}
		/// <summary>
		/// Gets the address of the next page.
		/// </summary>
		/// <param name="Address">Current address.</param>
		/// <param name="PageSize">Sets the page size</param>
		/// <returns>Returns the address of the next following page.</returns>
		public static int		Get_NextPageAdress(long Address, int PageSize)
		{
			if (Address % PageSize == 0)	return (int)Address;
			return (int)((Address / PageSize + 1) * PageSize);
		}
		/// <summary>
		/// Gets the address of the next page relative to current position in specified BinaryWriter.
		/// </summary>
		/// <param name="rw">BinaryWriter with source stream.</param>
		/// <returns>Returns the address of the next following page.</returns>
		public static int		Get_NextPageAdress(BinaryWriter rw)
		{	return Get_NextPageAdress(rw.BaseStream.Position);	}



		/// <summary>
		/// Compare two memory streams.
		/// </summary>
		/// <param name="Stream1">First memory stream to compare.</param>
		/// <param name="Stream2">Second memory stream to compare.</param>
		/// <returns>Returns true if two specified streams are identical, otherwise returns false.</returns>
		public static bool		Compare_Streams(MemoryStream Stream1, MemoryStream Stream2)
		{
			if (Stream1.Length != Stream2.Length)	return false;
			return Compare_Streams(Stream1, Stream2, 0, Stream1.Length);
		}
		/// <summary>
		/// Compare two memory streams.
		/// </summary>
		/// <param name="Stream1">First memory stream to compare.</param>
		/// <param name="Stream2">Second memory stream to compare.</param>
		/// <param name="Index">Index of position at which the comparing should start.</param>
		/// <param name="Count">Count of bytes to compare.</param>
		/// <returns>Returns true if two specified streams are identical, otherwise returns false.</returns>
		public static bool		Compare_Streams(MemoryStream Stream1, MemoryStream Stream2, uint Index, long Count)
		{
			bool a = true;
			uint a1, a2;
			var rd1 = new BinaryReader(Stream1);
			var rd2 = new BinaryReader(Stream2);
			Stream1.Position = Index;
			Stream2.Position = Index;
			for (uint i = 0; i < Count / 4; i++)
			{
				a1 = rd1.ReadUInt32();
				a2 = rd2.ReadUInt32();
				if (a1 != a2) a = false;
			}
			return a;
		}


		/// <summary>
		/// Get MD5 hash code as a string.
		/// </summary>
		/// <param name="Data">Byte array with data to calculate hash for.</param>
		/// <returns>Returns a 8-char long Unicode string.</returns>
		public static string	Get_MD5_String(byte[] Data)
		{
			if (Data == null)	return null;
			if (MD5 == null)	MD5 = MD5.Create();
			return Encoding.Unicode.GetString(MD5.ComputeHash(Data, 0, Data.Length));
		}
	}
}

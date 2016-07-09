using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;



namespace ZFC
{
	/// <summary>
	/// This class defines methods for working with files.
	/// </summary>
	public class ZFile
	{
		#region Read/Write binary/text data from file

		public static bool			IsValidFilename(string testName)
		{
			var containsABadCharacter = new Regex("[" + Regex.Escape(new string(Path.GetInvalidPathChars())) + "]");
			return !containsABadCharacter.IsMatch(testName);
		}


		/// <summary>
		/// Reads all text from the specified text file.
		/// </summary>
		/// <param name="fileName">Name of the file to read from.</param>
		/// <returns>A string with all text of the file if read was successful, null otherwise.</returns>
		public static string		ReadTextFile(string fileName)
		{
			try   { return File.ReadAllText(fileName); }
			catch { return null; }
		}
		/// <summary>
		/// Reads all text with given encoding from a text file.
		/// </summary>
		/// <param name="fileName">Name of the file to read from.</param>
		/// <param name="encoding">Encoding of text content.</param>
		/// <returns>A string with all text of the file if successful, null if failed.</returns>
		public static string		ReadTextFile(string fileName, Encoding encoding)
		{
			try   { return File.ReadAllText(fileName, encoding); }
			catch { return null; }
		}

		/// <summary>
		/// Writes given text string into a file.
		/// </summary>
		/// <param name="fileName">Name of the file to write.</param>
		/// <param name="textContent">Name of result file.</param>
		/// <returns>0 if successful, -1 if failed.</returns>
		public static int			WriteTextFile(string fileName, string textContent)
		{
			try   { File.WriteAllText(fileName, textContent); return 0; }
			catch { return -1; }
		}
		/// <summary>
		/// Writes given text string with given encoding into file.
		/// </summary>
		/// <param name="fileName">Name of the file to write.</param>
		/// <param name="textContent">Name of result file.</param>
		/// <param name="encoding">Encoding of text content.</param>
		/// <returns>0 if successful, -1 if failed.</returns>
		public static int			WriteTextFile(string fileName, string textContent, Encoding encoding)
		{
			try   { File.WriteAllText(fileName, textContent, encoding); return 0; }
			catch { return -1; }
		}


		/// <summary>
		/// Reads all bytes from a binary file.
		/// </summary>
		/// <param name="fileName">Name of the file to read from.</param>
		/// <returns>A byte array with all content of the file if successful, null if failed.</returns>
		public static byte[]		ReadFile(string fileName)
		{
			try   { return File.ReadAllBytes(fileName); }
			catch { return null; }
		}

		/// <summary>
		/// Writes the specified byte array into a file.
		/// </summary>
		/// <param name="fileName">Name of result file.</param>
		/// <param name="content">Byte array with data to write.</param>
		/// <returns>0 if successful, -1 if failed.</returns>
		public static int			WriteFile(string fileName, byte[] content)
		{
			try   { File.WriteAllBytes(fileName, content); return 0; }
			catch { return -1; }
		}

		#endregion


		/// <summary>
		/// Read a list of string from file.
		/// </summary>
		/// <param name="fileName">Name of the file to read.</param>
		/// <returns>Returns result StringList.</returns>
		public static StringList	Get_ListOfStrings(string fileName)
		{
			try   { return Get_ListOfStrings(new FileStream(fileName, FileMode.Open)); }
			catch { return null; }
		}
		/// <summary>
		/// Read a list of string from stream.
		/// </summary>
		/// <param name="stream">Stream instance to read from.</param>
		/// <returns>Returns result StringList.</returns>
		public static StringList	Get_ListOfStrings(Stream stream)
		{
			var delimiters = new string[] { "\r\n" };
			var streamReader = new StreamReader(stream);
			var streamContent = streamReader.ReadToEnd().Split(delimiters, StringSplitOptions.None);
			streamReader.Close();
			return new StringList(streamContent);
		}



		#region Binary Reader  &  BitConverter

		#region Variables

		private static BinaryReader		binaryReader;
		private static BinaryWriter		binaryWriter;
		private static byte[]			sourceArray;
		private static readonly byte[]	fillArray = new byte[1000];
		private static int				currentIndex;

		#endregion

		#region BitConverter

		/// <summary>
		/// Initialize the reading of file with BitConverter methods.
		/// </summary>
		/// <param name="fileName">Name of the file to read from.</param>
		public static void		BC_BeginRead(string fileName)
		{
			sourceArray = File.ReadAllBytes(fileName);
			currentIndex = 0;
		}
		/// <summary>
		/// Initialize the reading of byte array with BitConverter methods.
		/// </summary>
		/// <param name="newSourceArray">Source byte array.</param>
		public static void		BC_BeginRead(byte[] newSourceArray)
		{
			sourceArray = newSourceArray;
			currentIndex = 0;
		}

		/// <summary>
		/// Reads 32-bit integer.
		/// </summary>
		/// <returns>Returns 32-bit integer.</returns>
		public static int		BC_Read32()
		{
			currentIndex += 4; 
			return BitConverter.ToInt32(sourceArray, currentIndex - 4);
		}
		/// <summary>
		/// Reads 16-bit integer.
		/// </summary>
		/// <returns>Returns 16-bit integer.</returns>
		public static short		BC_Read16()
		{
			currentIndex += 2; 
			return BitConverter.ToInt16(sourceArray, currentIndex - 2);
		}
		/// <summary>
		/// Reads 8-bit integer.
		/// </summary>
		/// <returns>Returns 8-bit integer.</returns>
		public static byte		BC_Read8()
		{
			return sourceArray[currentIndex++];
		}
		/// <summary>
		/// Skips the specified count of bytes.
		/// </summary>
		/// <param name="countOfBytesToSkip">Count of bytes to skip.</param>
		public static void		BC_Skip(int countOfBytesToSkip)
		{
			currentIndex += countOfBytesToSkip;
		}
		#endregion

		#region BinaryReader

		/// <summary>
		/// Initialize the reading of file with BinaryReader methods.
		/// </summary>
		/// <param name="fileName">Name of the file to read from.</param>
		public static void		BR_BeginRead(string fileName)
		{
			binaryReader = new BinaryReader(new MemoryStream(File.ReadAllBytes(fileName)));
		}
		/// <summary>
		/// Reads 32-bit integer.
		/// </summary>
		/// <returns>Returns 32-bit integer.</returns>
		public static int		BR_Read32()
		{
			return binaryReader.ReadInt32();
		}
		/// <summary>
		/// Reads 16-bit integer.
		/// </summary>
		/// <returns>Returns 16-bit integer.</returns>
		public static short		BR_Read16()
		{
			return binaryReader.ReadInt16();
		}
		/// <summary>
		/// Reads 8-bit integer.
		/// </summary>
		/// <returns>Returns 8-bit integer.</returns>
		public static byte		BR_Read8()
		{
			return binaryReader.ReadByte();
		}
		/// <summary>
		/// Skips the specified count of bytes.
		/// </summary>
		/// <param name="countOfBytesToSkip">Count of bytes to skip.</param>
		public static void		BR_Skip(int countOfBytesToSkip)
		{
			binaryReader.BaseStream.Seek(countOfBytesToSkip, SeekOrigin.Current);
		}

		/// <summary>
		/// Initialize the reading of file with BinaryReader methods.
		/// </summary>
		/// <param name="fileName">Name of the file to read from.</param>
		public static void		BR_BeginWrite(string fileName)
		{
			binaryWriter = new BinaryWriter(new MemoryStream());
		}
		/// <summary>
		/// Writes 32-bit integer.
		/// </summary>
		public static void		BR_Write(int value)
		{
			binaryWriter.Write(value);
		}
		/// <summary>
		/// Writes 16-bit integer.
		/// </summary>
		public static void		BR_Write(short value)
		{
			binaryWriter.Write(value);
		}
		/// <summary>
		/// Writes 8-bit integer.
		/// </summary>
		public static void		BR_Write(byte value)
		{
			binaryWriter.Write(value);
		}
		/// <summary>
		/// Writes the specified count of zeroed bytes.
		/// </summary>
		/// <param name="countOfBytesToSkip">Count of bytes to write.</param>
		public static void		BR_Fill(int countOfBytesToSkip)
		{
			binaryWriter.Write(fillArray, 0, countOfBytesToSkip);
		}

		#endregion

		#endregion
	}
}

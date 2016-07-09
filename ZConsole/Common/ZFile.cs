namespace ZConsole.Common
{
	using System.IO;
	using System.Text;
	using System.Text.RegularExpressions;

	
	/// <summary>
	/// This class defines methods for working with files.
	/// </summary>
	public class ZFile
	{
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
	}
}
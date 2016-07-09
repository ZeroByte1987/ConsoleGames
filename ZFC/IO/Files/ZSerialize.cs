namespace ZFC
{
	using System;
	using System.IO;
	using System.Runtime.Serialization;
	using System.Runtime.Serialization.Formatters.Binary;


	/// <summary>
	/// This class defines the static methods for serialization purposes.
	/// </summary>
	public class ZSerialize
	{
		/// <summary>
		/// Write serialized object into file
		/// </summary>
		/// <typeparam name="T">Type of the object.</typeparam>
		/// <param name="fileName">Name of the output file.</param>
		/// <param name="sourceObject">Object to write into file.</param>
		public static void			Write_ObjToFile<T>(string fileName, T sourceObject)
		{
			var fileStream = new FileStream(fileName, FileMode.Create);
			var binaryWriter = new BinaryWriter(fileStream);
			binaryWriter.Write(Serialize(sourceObject));
			fileStream.Close();
		}


		/// <summary>
		/// Read serialized object from file
		/// </summary>
		/// <typeparam name="T">Type of object</typeparam>
		/// <param name="fileName">Name of input file</param>
		/// <param name="resultObject">Reference to object that will be read from file</param>
		public static void			Read_ObjFromFile<T>(string fileName, ref T resultObject)
		{
			var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
			Deserialize(fileStream, ref resultObject);
			fileStream.Close();
		}


		/// <summary>
		/// Clone object or instance of class
		/// </summary>
		/// <typeparam name="T">Type of the object</typeparam>
		/// <param name="sourceObject">Object to clone</param>
		/// <returns>A deep copy of the same object</returns>	
		public static T				Clone<T>(T sourceObject)
		{
			if (typeof(T).IsSerializable == false)	
				throw new ArgumentException("The type must be serializable.", "sourceObject");
			if (Object.ReferenceEquals(sourceObject, null))
				return default(T);
			IFormatter formatter = new BinaryFormatter();
			Stream memoryStream = new MemoryStream();
			using (memoryStream)
			{
				formatter.Serialize(memoryStream, sourceObject);
				memoryStream.Seek(0, SeekOrigin.Begin);
				return (T)formatter.Deserialize(memoryStream);
			}
		}


		/// <summary>
		/// Serialize object to byte array
		/// </summary>
		/// <typeparam name="T">Type of object</typeparam>
		/// <param name="sourceObject">Object to serialize</param>
		/// <returns>Byte array containing object</returns>
		public static byte[]		Serialize<T>(T sourceObject)
		{
			if (typeof(T).IsSerializable == false)	
				throw new ArgumentException("The type must be serializable.", "sourceObject");
			if (Object.ReferenceEquals(sourceObject, null))	
				return null;
			IFormatter formatter = new BinaryFormatter();
			Stream memoryStream = new MemoryStream();
			formatter.Serialize(memoryStream, sourceObject);
			memoryStream.Seek(0, SeekOrigin.Begin);
			var resultByteArray = new byte[memoryStream.Length];
			memoryStream.Read(resultByteArray, 0, (int)memoryStream.Length);
			return resultByteArray;
		}


		/// <summary>
		/// Deserialize stream to object
		/// </summary>
		/// <typeparam name="T">Type of object</typeparam>
		/// <param name="sourceStream">Stream containing serialized object</param>
		/// <param name="resultObject">Reference to object that will be deserialized from stream</param>
		public static void			Deserialize<T>(Stream sourceStream, ref T resultObject)
		{
			IFormatter formatter = new BinaryFormatter();
			resultObject = (T)formatter.Deserialize(sourceStream);
		}


		/// <summary>
		/// Compare objects
		/// </summary>
		/// <typeparam name="T">Type of object</typeparam>
		/// <param name="sourceObject">First object to compare</param>
		/// <param name="destinationObject">Second object to compare</param>
		/// <returns>True if object are identical, otherwise false.</returns>
		public static bool			Compare_Objects<T>(T sourceObject, T destinationObject)
		{
			var sourceByteArray		 = Serialize(sourceObject);
			var destinationByteArray = Serialize(destinationObject);
			if (sourceByteArray.Length != destinationByteArray.Length)
				return false;
			for (int i = 0; i < sourceByteArray.Length; i++)	
				if (sourceByteArray[i] != destinationByteArray[i]) 
					return false;
			return true;
		}


		/// <summary>
		/// Recreates object if its value is null (only for object with public constructors).
		/// </summary>
		/// <typeparam name="T">Any non-abstract class which has public constructor without parameters.</typeparam>
		/// <param name="sourceObject">The object to recreate.</param>
		public static void			Recreate<T>(ref T sourceObject)
		{
			if (sourceObject == null)
			{
				var objectType = typeof(T);
				var typeConstructor = objectType.GetConstructor(Type.EmptyTypes);
				sourceObject = typeConstructor != null 
					? (T) typeConstructor.Invoke(new object[] {}) 
					: default(T);
			}
		}
	}
}

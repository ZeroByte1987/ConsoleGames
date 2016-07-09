namespace ZFC
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Text;


	/// <summary>
	/// This class used for internal purposes.
	/// </summary>
	public class PropertyList : Dictionary<string, List<string>>
	{
		#region Add Properties

		/// <summary>
		/// Add a property with boolean value.
		/// </summary>
		/// <param name="propertyName">Name of the property to add.</param>
		/// <param name="value">Boolean value of property.</param>
		public void			AddProp(string propertyName, bool value)
		{
			AddProp(propertyName, value ? "1" : "0");
		}

		/// <summary>
		/// Add a property with integer value.
		/// </summary>
		/// <param name="propertyName">Name of the property to add.</param>
		/// <param name="value">Integer value of property.</param>
		public void			AddProp(string propertyName, int value)
		{
			AddProp(propertyName, value.ToString());
		}
		/// <summary>
		/// Add a property with integer value.
		/// </summary>
		/// <param name="propertyName">Name of the property to add.</param>
		/// <param name="value">Integer value of property.</param>
		public void			AddProp(string propertyName, decimal value)
		{
			AddProp(propertyName, value.ToString());
		}
		/// <summary>
		/// Add a property with float value.
		/// </summary>
		/// <param name="propertyName">Name of the property to add.</param>
		/// <param name="value">Float value of property.</param>
		public void			AddProp(string propertyName, float value)
		{
			AddProp(propertyName, value.ToString());
		}
		/// <summary>
		/// Add a property with string value.
		/// </summary>
		/// <param name="propertyName">Name of the property to add.</param>
		/// <param name="value">String value of property.</param>
		public void			AddProp(string propertyName, string value)
		{
			AddProp(propertyName, new List<string>() { value });
		}
		/// <summary>
		/// Add a property with list of strings.
		/// </summary>
		/// <param name="propertyName">Name of the property to add.</param>
		/// <param name="value">List of strings.</param>
		public void			AddProp(string propertyName, List<string> value)
		{
			Add(propertyName, value);
		}

		#endregion


		#region Get Values

		/// <summary>
		/// Get boolean value from this list of properties by category name.
		/// </summary>
		/// <param name="propertyName">Name of the property to look for.</param>
		/// <returns>Boolean value of property if it exists, or false otherwise.</returns>
		public bool			GetBool(string propertyName)
		{
			if (!this.ContainsKey(propertyName))	
				return false;
			string boolString = this[propertyName][0].ToLower();
			return (boolString == "1"  ||  boolString == "true"  ||  boolString == "on"  ||  boolString == "yes");
		}

		/// <summary>
		/// Get string value from this list of properties by category name.
		/// </summary>
		/// <param name="propertyName">Name of the property to look for.</param>
		/// <returns>String value of property if it exists, or empty string otherwise.</returns>
		public string		GetString(string propertyName)
		{
			return !this.ContainsKey(propertyName) ? "" : this[propertyName][0];
		}

		/// <summary>
		/// Get integer value from this list of properties by category name.
		/// </summary>
		/// <param name="propertyName">Name of the property to look for.</param>
		/// <returns>Boolean value of property if it exists, or false otherwise.</returns>
		public List<string>	GetStringList(string propertyName)
		{
			return !this.ContainsKey(propertyName) ? new List<string>() : this[propertyName];
		}

		/// <summary>
		/// Get integer value from this list of properties by category name.
		/// </summary>
		/// <param name="propertyName">Name of the property to look for.</param>
		/// <returns>Integer value of property if it exists, or -1 otherwise.</returns>
		public int			GetInt(string propertyName)
		{
			return GetInt(propertyName, -1);
		}
		/// <summary>
		/// Get integer value from this list of properties by category name.
		/// </summary>
		/// <param name="propertyName">Name of the property to look for.</param>
		/// <param name="defaultValue">Default value in case the given property can't be found.</param>
		/// <returns>Integer value of property if it exists, or DefaultValue otherwise.</returns>
		public  int			GetInt(string propertyName, int defaultValue)
		{
			if (!this.ContainsKey(propertyName))	
				return -1;
			string intString = this[propertyName][0].ToLower();
			try		{	return int.Parse(intString);	}
			catch	{	return -1;	}
		}

		#endregion
	}



	/// <summary>
	/// This class defines a set of methods to read and properly parse the configuration files.
	/// </summary>
	public class ZConfig
	{
		#region Read Config File

		/// <summary>
		/// Read a property set from file or string.
		/// </summary>
		/// <param name="source">The name of the configuration file to read or a source string with properties.</param>
		/// <returns>Returns resulting dictionary with pairs of categories and their appropriate values.</returns>
		public static PropertyList	ReadConfig(string source)
		{
			return ReadConfig(source, "[", "]", "", true, false, "\r\n");
		}

		/// <summary>
		/// Read a property set from file or string.
		/// </summary>
		/// <param name="source">The name of the configuration file to read or a source string with properties.</param>
		/// <param name="openingChars">Characters which open category name, e.g "[" in [Category].</param>
		/// <param name="closingChars">Characters which close category name, e.g "]" in [Category].</param>
		/// <returns>Returns resulting dictionary with pairs of categories and their appropriate values.</returns>
		public static PropertyList	ReadConfig(string source, string openingChars, string closingChars)
		{
			return ReadConfig(source, openingChars, closingChars, "", true, false, "\r\n");
		}

		/// <summary>
		/// Read a property set from file or string.
		/// </summary>
		/// <param name="source">The name of the configuration file to read or a source string with properties.</param>
		/// <param name="openingChars">Characters which open category name, e.g "[" in [Category].</param>
		/// <param name="closingChars">Characters which close category name, e.g "]" in [Category].</param>
		/// <param name="fillerChars">Filler characters which should be ignored when getting category names, e.g. "." in [...Category...]</param>
		/// <returns>Returns resulting dictionary with pairs of categories and their appropriate values.</returns>
		public static PropertyList	ReadConfig(string source, string openingChars, string closingChars, string fillerChars)
		{
			return ReadConfig(source, openingChars, closingChars, fillerChars, true, false, "\r\n");
		}

		/// <summary>
		/// Read a property set from file or string.
		/// </summary>
		/// <param name="source">The name of the configuration file to read or a source string with properties.</param>
		/// <param name="openingChars">Characters which open category name, e.g "[" in [Category].</param>
		/// <param name="closingChars">Characters which close category name, e.g "]" in [Category].</param>
		/// <param name="fillerChars">Filler characters which should be ignored when getting category names, e.g. "." in [...Category...]</param>
		/// <param name="removeEmpty">Remove empty entries or not.</param>
		/// <returns>Returns resulting dictionary with pairs of categories and their appropriate values.</returns>
		public static PropertyList	ReadConfig(string source, string openingChars, string closingChars, string fillerChars, bool removeEmpty)
		{
			return ReadConfig(source, openingChars, closingChars, fillerChars, removeEmpty, false, "\r\n");
		}

		/// <summary>
		/// Read a property set from file or string.
		/// </summary>
		/// <param name="source">The name of the configuration file to read or a source string with properties.</param>
		/// <param name="openingChars">Characters which open category name, e.g "[" in [Category].</param>
		/// <param name="closingChars">Characters which close category name, e.g "]" in [Category].</param>
		/// <param name="fillerChars">Filler characters which should be ignored when getting category names, e.g. "." in [...Category...]</param>
		/// <param name="removeEmpty">Remove empty entries or not.</param>
		/// <param name="isLowerCase">Transform category names to lower case or not.</param>
		/// <returns>Returns resulting dictionary with pairs of categories and their appropriate values.</returns>
		public static PropertyList	ReadConfig(string source, string openingChars, string closingChars, string fillerChars, bool removeEmpty, bool isLowerCase)
		{
			return ReadConfig(source, openingChars, closingChars, fillerChars, removeEmpty, isLowerCase, "\r\n");
		}

		/// <summary>
		/// Read a property set from file or string.
		/// </summary>
		/// <param name="source">The name of the configuration file to read or a source string with properties.</param>
		/// <param name="openingChars">Characters which open category name, e.g "[" in [Category].</param>
		/// <param name="closingChars">Characters which close category name, e.g "]" in [Category].</param>
		/// <param name="fillerChars">Filler characters which should be ignored when getting category names, e.g. "." in [...Category...]</param>
		/// <param name="removeEmpty">Remove empty entries or not.</param>
		/// <param name="isLowerCase">Transform category names to lower case or not.</param>
		/// <param name="newLineChars">Characters which used as default newlines, e.g. "\r\n".</param>
		/// <returns>Returns resulting dictionary with pairs of categories and their appropriate values.</returns>
		public static PropertyList	ReadConfig(string source, string openingChars, string closingChars, string fillerChars, bool removeEmpty, bool isLowerCase, string newLineChars)
		{
			var resultProperptyList = new PropertyList();
			string sourceText = (File.Exists(source)) ? ZFile.ReadTextFile(source) : source;
			var textLines = sourceText.Split(new [] { newLineChars, "\r", "\n" }, removeEmpty ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None);

			var currentList = new List<string>();
			for (int i = 0; i < textLines.Length; i++)
			{
				string line = textLines[i];
				if (line.StartsWith(openingChars))
				{
					int k = line.IndexOf(closingChars);
					line = line.Substring(openingChars.Length, (k != -1) ? k - openingChars.Length : line.Length - openingChars.Length - closingChars.Length);

					if (!string.IsNullOrEmpty(fillerChars))	
						line = line.Replace(fillerChars, "");
					if (isLowerCase)	
						line = line.ToLower();

					resultProperptyList.Add(line, new List<string>());
					currentList = resultProperptyList[line];
				}
				else	currentList.Add(line);
			}
			return resultProperptyList.Count == 0 ? null : resultProperptyList;
		}

		#endregion



		#region Write Config File

		/// <summary>
		/// Write a set of properties into output configuration file.
		/// </summary>
		/// <param name="propertyList">Source list of properties.</param>
		/// <param name="openingChars">Characters which open category name, e.g "[" in [Category].</param>
		/// <param name="closingChars">Characters which close category name, e.g "]" in [Category].</param>
		/// <param name="newLineChar">Characters which used as default newlines, e.g. "\r\n".</param>
		/// <param name="insertEmpty">Insert empty lines after every category or not.</param>
		/// <returns>Returns resulting string.</returns>
		public static string	WriteConfig(PropertyList propertyList, string openingChars, string closingChars, string newLineChar, bool insertEmpty)
		{	
			var stringBuilder = new StringBuilder();
			foreach (var property in propertyList)
			{
				stringBuilder.Append(openingChars).Append(property.Key).Append(closingChars).Append(newLineChar);
				for (int i = 0; i < property.Value.Count; i++)
					stringBuilder.Append(property.Value[i]).Append(newLineChar);
				if (insertEmpty)	
					stringBuilder.Append(newLineChar);
			}
			return stringBuilder.ToString();
		}

		/// <summary>
		/// Write a set of properties into output configuration file.
		/// </summary>
		/// <param name="fileName">Name of the file to write to.</param>
		/// <param name="propertyList">Source list of properties.</param>
		/// <returns>Return "true" if writing was successful.</returns>
		public static bool		WriteConfig(string fileName, PropertyList propertyList)
		{
			return WriteConfig(fileName, propertyList, "[", "]", "\r\n", false);
		}
		/// <summary>
		/// Write a set of properties into output configuration file.
		/// </summary>
		/// <param name="fileName">Name of the file to write to.</param>
		/// <param name="propertyList">Source list of properties.</param>
		/// <param name="openingChars">Characters which open category name, e.g "[" in [Category].</param>
		/// <param name="closingChars">Characters which close category name, e.g "]" in [Category].</param>
		/// <returns>Return "true" if writing was successful.</returns>
		public static bool		WriteConfig(string fileName, PropertyList propertyList, string openingChars, string closingChars)
		{
			return WriteConfig(fileName, propertyList, openingChars, closingChars, "\r\n", false);
		}
		/// <summary>
		/// Write a set of properties into output configuration file.
		/// </summary>
		/// <param name="fileName">Name of the file to write to.</param>
		/// <param name="propertyList">Source list of properties.</param>
		/// <param name="openingChars">Characters which open category name, e.g "[" in [Category].</param>
		/// <param name="closingChars">Characters which close category name, e.g "]" in [Category].</param>
		/// <param name="newLineChar">Characters which used as default newlines, e.g. "\r\n".</param>
		/// <returns>Return "true" if writing was successful.</returns>
		/// 
		public static bool		WriteConfig(string fileName, PropertyList propertyList, string openingChars, string closingChars, string newLineChar)
		{
			return WriteConfig(fileName, propertyList, openingChars, closingChars, newLineChar, false);
		}
		/// <summary>
		/// Write a set of properties into output configuration file.
		/// </summary>
		/// <param name="fileName">Name of the file to write to.</param>
		/// <param name="propertyList">Source list of properties.</param>
		/// <param name="openingChars">Characters which open category name, e.g "[" in [Category].</param>
		/// <param name="closingChars">Characters which close category name, e.g "]" in [Category].</param>
		/// <param name="newLineChar">Characters which used as default newlines, e.g. "\r\n".</param>
		/// <param name="insertEmpty">Insert empty lines after every category or not.</param>
		/// <returns>Return "true" if writing was successful.</returns>
		public static bool		WriteConfig(string fileName, PropertyList propertyList, string openingChars, string closingChars, string newLineChar, bool insertEmpty)
		{
			try	
			{	
				File.WriteAllText(fileName, WriteConfig(propertyList, openingChars, closingChars, newLineChar, insertEmpty));
				return true;
			}
			catch	{	return false;	}
		}

		#endregion
	}
}

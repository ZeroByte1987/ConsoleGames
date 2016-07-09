namespace ZFC
{
	using System;
	using System.IO;
	using System.Collections.Generic;


	/// <summary>
	/// This class defines the static methods for text formatting.
	/// </summary>
	public class ZTextFormat
	{
		/// <summary>
		/// Insert/Remove newlines in specified text string according to rules specifed by config string.
		/// </summary>
		/// <param name="sourceString">Source text string.</param>
		/// <param name="configContent">String with configuration.</param>
		/// <returns>Returns resulting string.</returns>
		public static string	NewLine(string sourceString, string configContent)
		{
			var text = (File.Exists(sourceString)) ? File.ReadAllText(sourceString) : sourceString;
			ReadConfig_NewLine(configContent);

			for (int i = 0; i < TaskList.Count; i++)
			{
				var task = TaskList[i];
				string NL = "",  CNL = "";
				for (int j = 0; j < task.CondCount; j++)	CNL += "\r\n";
				if (task.CondCount != 0)	task.Count = task.CondCount - task.Count;
				for (int j = 0; j < task.Count; j++)		NL	+= "\r\n";

				for (int j = 0; j < task.Entries.Count; j++)
				{
					#region Processing

					string S = task.Entries[j];
					string OldVal = S,  NewVal = S;
					if (task.TaskType == "Ins")
					switch (task.Situation)
					{
						case "Bef"		:	NewVal = NL + S;	break;
						case "Aft"		:	NewVal = S + NL;	break;
						case "If-Bef"	:	;	break;
						case "If-Aft"	:	;	break;
						case "Betw"		:
							#region
							int k = S.IndexOf("\t");
							int n = int.Parse(S.Substring(0, k));
							OldVal = S.Substring(k+1, S.Length-k-1);
							NewVal = OldVal.Substring(0, n) + NL + OldVal.Substring(n, OldVal.Length-n);
							break;
							#endregion
					}
					if (task.TaskType == "Del")
					switch (task.Situation)
					{
						case "Bef"		:	OldVal = NL + S;	break;
						case "Aft"		:	OldVal = S + NL;	break;
						case "If-Bef"	:	OldVal = CNL + S;	NewVal = NL + S;	break;
						case "If-Aft"	:	OldVal = S + CNL;	NewVal = S + NL;	break;
					}
					text = text.Replace(OldVal, NewVal);
					#endregion
				}				
			}

			return text;
		}


		/// <summary>
		///	Makes multiple replaces in source text string, according to rules specifed by config string.
		/// </summary>
		/// <param name="sourceString">Source text string.</param>
		/// <param name="config">Array of strings with configuration.</param>
		/// <returns>Returns resulting string.</returns>
		public static string	TextReplace(string sourceString, string[] config)
		{
			var text = (File.Exists(sourceString)) ? File.ReadAllText(sourceString) : sourceString;
			ReadConfig_TextReplace(config);
			for (int i = 0; i < OldValues.Count; i++)
				text = text.Replace(OldValues[i], NewValues[i]);
			return text;
		}

		/// <summary>
		/// Insert/remove paddings in specified text string according to rules specifed by config string.
		/// </summary>
		/// <param name="sourceString">Source text string.</param>
		/// <param name="Config">String with configuration.</param>
		/// <returns>Returns resulting string.</returns>
		public static string	CreatePad(string sourceString, string configString)
		{
			string Pad = string.Empty;
			var text = (File.Exists(sourceString)) ? File.ReadAllText(sourceString) : sourceString;
			var configProperties = ZConfig.ReadConfig(configString, "[", "]", ".", true, true);
			string paddingConfig = configProperties.GetString("padding");
			int N = paddingConfig.IndexOf("=");
			N = int.Parse(paddingConfig.Substring(N+1, paddingConfig.Length-N-1));
			if (paddingConfig.StartsWith("Spaces"))	Pad = Pad.PadRight(N, ' ');
			if (paddingConfig.StartsWith("Tabs"))	Pad = Pad.PadRight(N, '\t');
				
			return text;
		}



		#region Service variables

		private static List<Task>	TaskList;

		private static List<string> OldValues	= new List<string>();
		private static List<string> NewValues	= new List<string>();
		
		#endregion


		#region Service routines

		private static void		ReadConfig_NewLine(string configSource)
		{
			var configList	= ZConfig.ReadConfig(configSource, "[", "]", ".", true);
			TaskList = new List<Task>();
			foreach (var property in configList)
			{
				var tokens = property.Key.Split('-');
				if (tokens.Length < 3)	continue;
				var T = new Task();
				if (tokens[0] == "Insert")		T.TaskType = "Ins";
				if (tokens[0] == "Delete")		T.TaskType = "Del";
				T.Count	= int.Parse(tokens[1]);
				if (tokens[2] == "Before")		T.Situation = "Bef";
				if (tokens[2] == "After")		T.Situation = "Aft";
				if (tokens[2] == "IfBefore")	T.Situation = "If-Bef";
				if (tokens[2] == "IfAfter")		T.Situation = "If-Aft";
				if (tokens[2] == "InBetween")	T.Situation = "Betw";
				if (tokens.Length > 3)
					T.CondCount	= int.Parse(tokens[3]);
				T.Entries.AddRange(property.Value);
				TaskList.Add(T);
			}
			if (configList == null)	
				Environment.Exit(0);
		}

		private static void		ReadConfig_TextReplace(string[] tokens)
		{
			if (tokens.Length < 2  ||  tokens.Length % 3 != 0  &&  tokens.Length % 3 != 2)	return;
			for (int i = 0; i < tokens.Length; i++)
			{
				OldValues.Add(GetLine(tokens[i++]));
				NewValues.Add(GetLine(tokens[i++]));
			}
		}
		private static string	GetLine(string line)
		{
			return line.Replace("\\r", "\r").Replace("\\n", "\n").Replace("\\t", "\t").Replace("\\v", "\v");
		}

		#endregion


		#region Service classes
		 
		private class Task
		{
			internal string		TaskType;
			internal string		Situation;
			internal int		Count;
			internal int		CondCount;
			internal readonly List<string>	Entries;

			internal new string		ToString()
			{
				string resultString = TaskType + " " + Count + " " + Situation;
				return (CondCount != 0)	 ? resultString + " " + CondCount : resultString;
			}

			internal Task()
			{
				Entries		= new List<string>();
				TaskType	= string.Empty;
				Situation	= string.Empty;
			}
		}

		#endregion
	}
}

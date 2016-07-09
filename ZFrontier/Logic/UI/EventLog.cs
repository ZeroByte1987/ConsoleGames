namespace ZFrontier.Logic.UI
{
	using Common;
	using Objects.GameData;
	using ZConsole;


	public class EventLog : AreaUI_Basic
	{
		#region Private Fields  and  Constructor

		private static string			eventLogDivider;
		private const string			LogFileName = "ZCommander_Log.txt";


		public EventLog(Rect eventLogArea)
			: base(eventLogArea)
		{
			AreaType		= AreaUI.EventLog;
			ZMessageLog.Initialize(2, 2, eventLogArea.Width-2, eventLogArea.Height-2);
			eventLogDivider = "^-".PadRight(eventLogArea.Width - 6, '-') + "^";
		    ZMessageLog.YesText = Lang["Common_Yes"];
            ZMessageLog.NoText  = Lang["Common_No"];
            ZUI.YesText = Lang["Common_Yes"];
            ZUI.NoText  = Lang["Common_No"];
		}

		#endregion


		public void			Clear()
		{
			ClearArea();
			ZMessageLog.Clear();
		}


		public void			PrintPlain(string text, bool useSpacing = true)
		{
			ZMessageLog.Draw_Message(text, useSpacing);
		}

		public void			PrintPlainWithoutLog(string text, bool useSpacing = true)
		{
			ZMessageLog.Draw_Message(text, useSpacing, false);
		}

		public void			Print(string text, bool useSpacing = true)
		{
			ZMessageLog.Draw_Message(Lang[text], useSpacing);
		}

		public void			Print(string text, int value, bool useSpacing = true)
		{
			ZMessageLog.Draw_Message(string.Format(Lang[text], value), useSpacing);
		}

		public void			Print(string text, int value1, int value2, bool useSpacing = true)
		{
			ZMessageLog.Draw_Message(string.Format(Lang[text], value1, value2), useSpacing);
		}

		public void			Print(string text, string value, bool useSpacing = true)
		{
			ZMessageLog.Draw_Message(string.Format(Lang[text], value), useSpacing);
		}

		public void			Print(string text, string value1, string value2, bool useSpacing = true)
		{
			ZMessageLog.Draw_Message(string.Format(Lang[text], value1, value2), useSpacing);
		}


		public void			PrintDivider()
		{
			ZMessageLog.Draw_Message(eventLogDivider);
		}

		public void			PrintLine()
		{
			ZMessageLog.Draw_Message("", false);
		}

		public void			ShadowOldMessages()
		{
			ZMessageLog.ShadowOldMessages();
		}


		public bool			Get_YesNo(string text, bool buttonsOnSameLine = false, bool isNoDefault = false)
		{
			return ZMessageLog.Draw_Message_YesNo(Lang[text], buttonsOnSameLine, isNoDefault);
		}

		public bool			Get_YesNo(string text, int value, bool buttonsOnSameLine = false, bool isNoDefault = false)
		{
			return ZMessageLog.Draw_Message_YesNo(string.Format(Lang[text], value), buttonsOnSameLine, isNoDefault);
		}

		public bool			Get_YesNo(string text, string value, bool buttonsOnSameLine = false, bool isNoDefault = false)
		{
			return ZMessageLog.Draw_Message_YesNo(string.Format(Lang[text], value), buttonsOnSameLine, isNoDefault);
		}

		public bool			Get_YesNo(string text, string value1, string value2, bool buttonsOnSameLine = false, bool isNoDefault = false)
		{
			return ZMessageLog.Draw_Message_YesNo(string.Format(Lang[text], value1, value2), buttonsOnSameLine, isNoDefault);
		}

		public bool			Get_YesNo(string text, string value1, string value2, string value3, bool buttonsOnSameLine = false, bool isNoDefault = false)
		{
			return ZMessageLog.Draw_Message_YesNo(string.Format(Lang[text], value1, value2, value3), buttonsOnSameLine, isNoDefault);
		}


		public bool			WriteLogToFile()
		{
			return ZMessageLog.FlushLogToFile(LogFileName);
		}
	}
}

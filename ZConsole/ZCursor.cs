namespace ZConsole
{
	using System;
	using LowLevel;


	public static class ZCursor
	{
		public static IntPtr		hConsoleOutput;
		public static IntPtr		hConsoleInput;

		public static Size			WindowSize  = new Size(80, 30);

		private static ZCharInfo	_charUnderCursor;
		private static ZCharInfo	_currentCursor;
		private static Coord		_cursorPosition;
		private static Rect			_cursorValidArea;


		public static void		SetPosition(int x, int y)
		{
			Console.SetCursorPosition(x, y);
			_cursorPosition = new Coord(x, y);
		}

		public static void		SetCursorVisibility(bool isVisible)
		{
			Console.CursorVisible = isVisible;
		}

		public static void		SetCursorChar(char cursorChar, Color foreColor, Color backColor)
		{
			_currentCursor = new ZCharInfo(cursorChar, new ZCharAttribute(foreColor, backColor));
		}

		public static void		SetCursorValidArea(int left, int top, int right, int bottom)
		{
			_cursorValidArea = new Rect(left, top, right, bottom);
		}

		public static void		ShowCursor()
		{
			_charUnderCursor = GetCharInfo(_cursorPosition.X, _cursorPosition.Y);
			ZOutput.Print(_cursorPosition.X, _cursorPosition.Y, _currentCursor.UnicodeChar, _currentCursor.ForeColor, _currentCursor.BackColor);
		}

		public static void		HideCursor()
		{
			ZOutput.Print(_cursorPosition.X, _cursorPosition.Y, _charUnderCursor.UnicodeChar, _charUnderCursor.Attribute);
		}

		public static void		MoveCursor(int xMove, int yMove)
		{
			if (_cursorPosition.X + xMove >= _cursorValidArea.Left  &&  _cursorPosition.X + xMove <= _cursorValidArea.Right)
				_cursorPosition.X += xMove;

			if (_cursorPosition.Y + yMove >= _cursorValidArea.Top   &&  _cursorPosition.Y + yMove <= _cursorValidArea.Bottom)
				_cursorPosition.Y += yMove;
		}


		#region Get chars and attributes

		public static char				GetChar(int x, int y)
		{
			var chars = new char[2];
			var countRead = 0;
			WinCon.ReadConsoleOutputCharacterW(hConsoleOutput, chars, 2, new CoordInternal(x, y), ref countRead);
			return Tools.Get_UnicodeChar_From_TwoChars(chars);
		}

		public static char[]			GetChar(int x, int y, int count)
		{
			var chars = new char[count*2];
			var countRead = 0;
			WinCon.ReadConsoleOutputCharacterW(hConsoleOutput, chars, count*2, new CoordInternal(x, y), ref countRead);
			return Tools.Get_UnicodeChars_From_TwoCharArray(chars);
		}

		public static ZCharAttribute	GetCharAttributes(int x, int y)
		{
			var attrs = new ZCharAttribute[1];
			var countRead = 0;
			WinCon.ReadConsoleOutputAttribute(hConsoleOutput, attrs, 1, new CoordInternal(x, y), ref countRead);
			return attrs[0];
		}

		public static ZCharAttribute[]	GetCharAttributes(int x, int y, int count)
		{
			var attrs = new ZCharAttribute[count];
			var countRead = 0;
			WinCon.ReadConsoleOutputAttribute(hConsoleOutput, attrs, count, new CoordInternal(x, y), ref countRead);
			return attrs;
		}

		public static ZCharInfo			GetCharInfo(int x, int y)
		{
			var charInfo = new ZCharInfo(GetChar(x, y), GetCharAttributes(x, y));
			return charInfo;
		}

		#endregion
	}
}
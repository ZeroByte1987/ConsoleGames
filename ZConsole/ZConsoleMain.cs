namespace ZConsole
{
	using System;
	using System.Diagnostics;
	using System.Runtime.InteropServices;
	using System.Text;
	using LowLevel;


	public static class ZConsoleMain
	{
		public static Size			WindowSize  = new Size(80, 30);

		private static Encoding		oldInputEncoding = Encoding.Default;
		private static Encoding		oldOutputEncoding = Encoding.Default;


		public static void		Initialize()
		{
			InitializeConsoleOutput();
			InitializeConsoleInput();
			
			var cHnd = Process.GetCurrentProcess().MainWindowHandle;
			WinCon.SetWindowLong(cHnd, -16, 0x00CA0000);
			WinCon.SetWindowPos(cHnd, new IntPtr(-2), 100, 50, 1000, 660, 0x0040);

			ZColors.StoreColors();
		}

		public static void		InitializeConsoleOutput()
		{
			oldOutputEncoding = Console.OutputEncoding;
			Console.OutputEncoding = Encoding.GetEncoding(866);
			ZOutput.hConsoleOutput = WinCon.GetStdHandle(WinCon.STD_OUTPUT_HANDLE);
		}

		public static void		InitializeConsoleInput()
		{
			oldInputEncoding  = Console.InputEncoding;
			Console.InputEncoding  = Encoding.GetEncoding(866);
			ZInput.hConsoleInput  = WinCon.GetStdHandle(WinCon.STD_INPUT_HANDLE);

			var mode = 0;
            if (!(WinCon.GetConsoleMode(ZInput.hConsoleInput, ref mode)))
	            throw new Exception();

            mode |= (int)ConsoleInputModeFlags.MouseInput;
            mode &= ~(int)ConsoleInputModeFlags.QuickEditMode;
            mode |= (int)ConsoleInputModeFlags.ExtendedFlags;

            if (!(WinCon.SetConsoleMode(ZInput.hConsoleInput, mode)))
	            throw new Exception();
		}


		public static void		Initialize(Size consoleSize)
		{
			Initialize(consoleSize.Width, consoleSize.Height);
		}

		public static void		Initialize(int xConsoleSize, int yConsoleSize)
		{
			Initialize();
			SetWindowSize(xConsoleSize, yConsoleSize);
			WindowSize = new Size(xConsoleSize, yConsoleSize);
		}

		public static void		RestoreMode()
		{
			Console.OutputEncoding = oldOutputEncoding;
			Console.InputEncoding  = oldInputEncoding;
			Console.BackgroundColor = ConsoleColor.Black;

			ZColors.RestoreColors();
		}



		public static void		ClearScreen()
		{
			ZColors.SetBackColor(Color.Black);
			Console.Clear();
		}

		public static void		SetWindowSize(int width, int height)
		{
			Console.SetWindowSize(width, height);
			Console.BufferWidth = width;
			Console.BufferHeight = height;
		}

		public static void		ChangeConsoleCaption(string caption)
		{
			Console.Title = caption;
		}

		public static unsafe void	SetConsoleFont(string fontName, int xSize, int ySize)
		{
			var hnd = WinCon.GetStdHandle(WinCon.STD_OUTPUT_HANDLE);
			var newInfo = new CONSOLE_FONT_INFO_EX { cbSize = 84, FontFamily = 48 };
			var ptr = new IntPtr(newInfo.FaceName);
			Marshal.Copy(fontName.ToCharArray(), 0, ptr, fontName.Length);
			newInfo.dwFontSize = new CoordInternal(xSize, ySize);
			newInfo.FontWeight = 400;
			WinCon.SetCurrentConsoleFontEx(hnd, false, ref newInfo);
		}
	}
}
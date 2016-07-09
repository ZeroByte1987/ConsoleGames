namespace ZConsole.LowLevel
{
	using System;
	using System.Text;
	using System.Runtime.InteropServices;


	[StructLayout(LayoutKind.Sequential)]
	public class		SecurityAttributes
	{
		public int		nLength;
		public IntPtr	lpSecurityDescriptor;
		public bool		bInheritHandle;
	}


	/// <summary>
    /// Control event handler delegate.
    /// </summary>
    /// <param name="CtrlType">Control event type.</param>
    /// <returns>Return true to cancel the control event.  A return value of false
    /// will terminate the application and send the event to the next control handler.</returns>
    public delegate bool ConsoleCtrlHandlerDelegate(ConsoleControlEventType CtrlType);


	/// <summary>
    /// Windows Console API definitions.
    /// </summary>
	public static class WinCon
	{
		// Attributes flags:
		public const int FOREGROUND_BLUE			= 0x0001; // text color contains blue.
		public const int FOREGROUND_GREEN			= 0x0002; // text color contains green.
		public const int FOREGROUND_RED				= 0x0004; // text color contains red.
		public const int FOREGROUND_INTENSITY		= 0x0008; // text color is intensified.
		public const int BACKGROUND_BLUE			= 0x0010; // background color contains blue.
		public const int BACKGROUND_GREEN			= 0x0020; // background color contains green.
		public const int BACKGROUND_RED				= 0x0040; // background color contains red.
		public const int BACKGROUND_INTENSITY		= 0x0080; // background color is intensified.
		public const int COMMON_LVB_LEADING_BYTE    = 0x0100; // Leading Byte of DBCS
		public const int COMMON_LVB_TRAILING_BYTE   = 0x0200; // Trailing Byte of DBCS
		public const int COMMON_LVB_GRID_HORIZONTAL = 0x0400; // DBCS: Grid attribute: top horizontal.
		public const int COMMON_LVB_GRID_LVERTICAL  = 0x0800; // DBCS: Grid attribute: left vertical.
		public const int COMMON_LVB_GRID_RVERTICAL  = 0x1000; // DBCS: Grid attribute: right vertical.
		public const int COMMON_LVB_REVERSE_VIDEO   = 0x4000; // DBCS: Reverse fore/back ground attribute.
		public const int COMMON_LVB_UNDERSCORE      = 0x8000; // DBCS: Underscore.
		public const int COMMON_LVB_SBCSDBCS        = 0x0300; // SBCS or DBCS flag.

		public const long WS_OVERLAPPEDWINDOW		= 0x0000;

		public const int ATTACH_PARENT_PROCESS		= -1;

		public const int CONSOLE_TEXTMODE_BUFFER	= 1;

		public const int STD_INPUT_HANDLE			= -10;
		public const int STD_OUTPUT_HANDLE			= -11;
		public const int STD_ERROR_HANDLE			= -12;

		// *******************************************************
		// * API Definitions
		// *******************************************************

        [DllImport("kernel32.dll", SetLastError=true)]
		public static extern bool		AddConsoleAlias(string Source, string Target, string ExeName);

		[DllImport("kernel32.dll", SetLastError=true)]
		public static extern bool		AllocConsole();

		[DllImport("kernel32.dll", SetLastError=true)]
		public static extern bool		AttachConsole(int dwProcessId);

		[DllImport("kernel32.dll", SetLastError=true)]
		public static extern IntPtr		CreateConsoleScreenBuffer(
			int dwDesiredAccess,
			int dwShareMode,
			[In, Out][MarshalAs(UnmanagedType.LPStruct)] SecurityAttributes lpSecurityAttributes,
			int dwFlags,
			IntPtr lpScreenBufferData);

		[DllImport("kernel32.dll", SetLastError=true)]
		public static extern bool		FillConsoleOutputAttribute(
			IntPtr hConsoleOutput,
			ZCharAttribute wAttribute,
			int nLength,
			CoordInternal dwWriteCoord,
			ref int lpNumberOfAttrsWritten);

		[DllImport("kernel32.dll", SetLastError=true)]
		public static extern bool		FillConsoleOutputCharacter(
			IntPtr hConsoleOutput,
			char cCharacter,
			int nLength,
			CoordInternal dwWriteCoord,
			ref int lpNumberOfCharsWritten);

		[DllImport("kernel32.dll", SetLastError=true)]
		public static extern bool		FlushConsoleInputBuffer(IntPtr hConsoleInput);

		[DllImport("kernel32.dll", SetLastError=true)]
		public static extern bool		FreeConsole();

		[DllImport("kernel32.dll", SetLastError=true)]
		public static extern bool		GenerateConsoleCtrlEvent(int ctrlEvent, int dwProcessGroupId);

		[DllImport("kernel32.dll", SetLastError=true)]
		public static extern int		GetConsoleAlias(
			string lpSource,
			[Out][MarshalAs(UnmanagedType.LPArray, SizeParamIndex=1)] char[] lpTargetBuffer,
			int TargetBufferLength,
			string lpExeName);

		[DllImport("kernel32.dll", SetLastError=true)]
		public static extern int		GetConsoleAliases(
			[Out][MarshalAs(UnmanagedType.LPArray, SizeParamIndex=1)] char[] lpAliasBuffer,
			int AliasBufferLength,
			string lpExeName);

		[DllImport("kernel32.dll", SetLastError=true)]
		public static extern int		GetConsoleAliasesLength(string lpExeName);

		[DllImport("kernel32.dll", SetLastError=true)]
		public static extern int		GetConsoleAliasExes(
			[In, Out][MarshalAs(UnmanagedType.LPArray, SizeParamIndex=1)] char[] lpExeNameBuffer,
			int ExeNameBufferLength);

		[DllImport("kernel32.dll", SetLastError=true)]
		public static extern int		GetConsoleAliasExesLength();

		[DllImport("kernel32.dll", SetLastError=true)]
		public static extern int		GetConsoleCP();

		[DllImport("kernel32.dll", SetLastError=true)]
		public static extern bool		GetConsoleCursorInfo(
			IntPtr hConsoleOutput,
			[Out][MarshalAs(UnmanagedType.LPStruct)] ZCursorInfo lpConsoleCursorInfo);

		[DllImport("kernel32.dll", SetLastError=true)]
		public static extern bool		GetConsoleDisplayMode(ref int lpModeFlags);

		[DllImport("kernel32.dll", SetLastError=true)]
		public static extern CoordInternal		GetConsoleFontSize(IntPtr hConsoleOutput, int nFont);

		[DllImport("kernel32.dll", SetLastError=true)]
		public static extern bool		GetConsoleMode(IntPtr hConsoleHandle, ref int lpMode);

		[DllImport("kernel32.dll", SetLastError=true)]
		public static extern int		GetConsoleOutputCP();

		[DllImport("kernel32.dll", SetLastError=true)]
		public static extern int		GetConsoleProcessList(
			[In,Out][MarshalAs(UnmanagedType.LPArray, SizeParamIndex=1)]int[] lpdwProcessList,
			int dwProcessCount);

		[DllImport("kernel32.dll", SetLastError=true)]
		public static extern bool		GetConsoleScreenBufferInfo(
			IntPtr hConsoleOutput,
			[In,Out][MarshalAs(UnmanagedType.LPStruct)]ConsoleScreenBufferInfo lpConsoleScreenBufferInfo);

		[DllImport("kernel32.dll", SetLastError=true)]
		public static extern bool		GetConsoleSelectionInfo(
			[In,Out][MarshalAs(UnmanagedType.LPStruct)]ConsoleSelectionInfo lpConsoleSelectionInfo);

		[DllImport("kernel32.dll", SetLastError=true)]
		public static extern int		GetConsoleTitle(
			[In,Out][MarshalAs(UnmanagedType.LPStr, SizeParamIndex=1)]StringBuilder lpConsoleTitle,
			int nSize);

		[DllImport("kernel32.dll", SetLastError=true)]
		public static extern IntPtr		GetConsoleWindow();

		[DllImport("kernel32.dll", SetLastError=true)]
		public static extern bool		GetCurrentConsoleFont(
			IntPtr hConsoleOutput,
			bool bMaximumWindow,
			[Out][MarshalAs(UnmanagedType.LPStruct)]ConsoleFontInfo lpConsoleCurrentFont);

		[DllImport("kernel32.dll", SetLastError=true)]
		public static extern CoordInternal		GetLargestConsoleWindowSize(IntPtr hConsoleOutput);

		[DllImport("kernel32.dll", SetLastError=true)]
		public static extern bool		GetNumberOfConsoleInputEvents(IntPtr hConsoleInput, ref int lpcNumberOfEvents);

		[DllImport("kernel32.dll", SetLastError=true)]
		public static extern bool		GetNumberOfConsoleMouseButtons(ref int lpNumberOfMouseButtons);

		[DllImport("kernel32.dll", SetLastError=true)]
		public static extern IntPtr		GetStdHandle(int nStdHandle);

		[DllImport("kernel32.dll", SetLastError=true)]
		public static extern bool		PeekConsoleInput(
			IntPtr hConsoleInput,
			[Out][MarshalAs(UnmanagedType.LPArray, SizeParamIndex=2)]ConsoleInput[] lpBuffer,
			int nLength,
			ref int lpNumberOfEventsRead);

		[DllImport("kernel32.dll", SetLastError=true)]
		public static extern bool		ReadConsole(
			IntPtr hConsoleInput,
			[Out][MarshalAs(UnmanagedType.LPArray, SizeParamIndex=2)]char[] lpBuffer,
			int nNumberOfCharsToRead,
			ref int lpNumberOfCharsRead,
			IntPtr lpReserved);  // must be null (IntPtr.Zero)

		[DllImport("kernel32.dll", SetLastError=true)]
		public static extern bool		ReadConsoleInput(
			IntPtr hConsoleInput,
			[Out][MarshalAs(UnmanagedType.LPArray, SizeParamIndex=2)]ConsoleInput[] lpBuffer,
			int nLength,
			ref int lpNumberOfEventsRead);

		[DllImport("kernel32.dll", SetLastError=true)]
		public static extern bool		ReadConsoleOutput(
			IntPtr hConsoleOutput,
			[Out][MarshalAs(UnmanagedType.LPArray, SizeParamIndex=2)]ZCharInfo[,] lpBuffer,
			CoordInternal dwBufferSize,
			CoordInternal dwBufferCoord,
			[In,Out][MarshalAs(UnmanagedType.LPStruct)]RectInternal lpReadRegion);

		[DllImport("kernel32.dll", SetLastError=true)]
		public static extern bool		ReadConsoleOutputAttribute(
			IntPtr hConsoleOutput,
			[Out][MarshalAs(UnmanagedType.LPArray, SizeParamIndex=2)]ZCharAttribute[] lpAttribute,
			int nLength,
			CoordInternal dwReadCoord,
			ref int lpNumberOfAttrsRead);

		[DllImport("kernel32.dll", SetLastError=true)]
		public static extern bool		ReadConsoleOutputCharacter(
			IntPtr hConsoleOutput,
			[Out][MarshalAs(UnmanagedType.LPArray, SizeParamIndex=2)]char[] lpCharacter,
			int nLength,
			CoordInternal dwReadCoord,
			ref int lpNumberOfCharsRead);

		[DllImport("kernel32.dll", SetLastError=true)]
		public static extern bool		ReadConsoleOutputCharacterW(
			IntPtr hConsoleOutput,
			[Out][MarshalAs(UnmanagedType.LPArray, SizeParamIndex=2)]char[] lpCharacter,
			int nLength,
			CoordInternal dwReadCoord,
			ref int lpNumberOfCharsRead);

		[DllImport("kernel32.dll", SetLastError=true)]
		public static extern bool		ScrollConsoleScreenBuffer(
			IntPtr hConsoleOutput,
			[In][MarshalAs(UnmanagedType.LPStruct)]RectInternal lpScrollRectangle,
			[In][MarshalAs(UnmanagedType.LPStruct)]RectInternal lpClipRectangle,
			CoordInternal dwDestinationOrigin,
			ref ZCharInfo lpFill);

		[DllImport("kernel32.dll", SetLastError=true)]
		public static extern bool		SetConsoleActiveScreenBuffer(IntPtr hConsoleOutput);

		[DllImport("kernel32.dll", SetLastError=true)]
		public static extern bool		SetConsoleCP(int wCodePageID);

        [DllImport("kernel32.dll", SetLastError=true)]
		public static extern bool		SetConsoleCtrlHandler(ConsoleCtrlHandlerDelegate HandlerRoutine, bool Add);

		[DllImport("kernel32.dll", SetLastError=true)]
		public static extern bool		SetConsoleCursorInfo(
			IntPtr hConsoleOutput,
			[In][MarshalAs(UnmanagedType.LPStruct)]ZCursorInfo lpConsoleCursorInfo);

		[DllImport("kernel32.dll", SetLastError=true)]
		public static extern bool		SetConsoleCursorPosition(IntPtr hConsoleOutput, CoordInternal dwCursorPosition);

		[DllImport("kernel32.dll", SetLastError=true)]
		public static extern bool		SetConsoleDisplayMode(IntPtr hConsoleOutput, int dwFlags, ref CoordInternal lpNewScreenBufferDimensions);

		[DllImport("kernel32.dll", SetLastError=true)]
		public static extern bool		SetConsoleMode(IntPtr hConsoleHandle, int ioMode);

		[DllImport("kernel32.dll", SetLastError=true)]
		public static extern bool		SetConsoleOutputCP(int wCodePageID);

		[DllImport("kernel32.dll", SetLastError=true)]
		public static extern bool		SetConsoleScreenBufferSize(IntPtr hConsoleOutput, CoordInternal dwSize);

		[DllImport("kernel32.dll", SetLastError=true)]
		public static extern bool		SetConsoleTextAttribute(IntPtr hConsoleOutput, ZCharAttribute attr);
		
		[DllImport("kernel32.dll", SetLastError=true)]
		public static extern bool		SetConsoleTitle(string lpConsoleTitle);

		[DllImport("kernel32.dll", SetLastError=true)]
		public static extern bool		SetConsoleWindowInfo(
			IntPtr hConsoleOutput,
			bool bAbsolute,
			[In][MarshalAs(UnmanagedType.LPStruct)]RectInternal lpConsoleWindow);

		[DllImport("kernel32.dll", SetLastError=true)]
		public static extern bool		SetStdHandle(int nStdHandle, IntPtr hHandle);

		[DllImport("user32.dll")]
		public static extern bool		SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

		[DllImport("user32.dll")]
		public static extern bool		SetWindowLong(IntPtr hWnd, int nIndex, long newParams);

		[DllImport("kernel32.dll", SetLastError=true)]
		public static extern bool		WriteConsole(
			IntPtr hConsoleOutput,
			[In][MarshalAs(UnmanagedType.LPArray, SizeParamIndex=2)]char[] lpBuffer,
			int NumberOfCharsToWrite,
			ref int NumberOfCharsWritten,
			IntPtr reserved);

		[DllImport("kernel32.dll", SetLastError=true)]
		public static extern bool		WriteConsoleInput(
			IntPtr hConsoleInput,
			[In][MarshalAs(UnmanagedType.LPArray, SizeParamIndex=2)]ConsoleInput[] lpBuffer,
			int nLength,
			ref int lpNumberOfEventsWritten);

		[DllImport("kernel32.dll", SetLastError=true)]
		public static extern bool		WriteConsoleOutput(
			IntPtr hConsoleOutput,
			[In][MarshalAs(UnmanagedType.LPArray, SizeParamIndex=2)]ZCharInfo[,] lpBuffer,
			CoordInternal dwBufferSize,
			CoordInternal dwBufferCoord,
			[In,Out][MarshalAs(UnmanagedType.LPStruct)]RectInternal lpWriteRegion);

		[DllImport("kernel32.dll", SetLastError=true)]
		public static extern bool		WriteConsoleOutputAttribute(
			IntPtr hConsoleOutput,
			[In][MarshalAs(UnmanagedType.LPArray, SizeParamIndex=2)]ZCharAttribute[] lpAttribute,
			int nLength,
			CoordInternal dwWriteCoord,
			ref int lpNumberOfAttrsWritten);

		[DllImport("kernel32.dll", SetLastError=true)]
		public static extern bool		WriteConsoleOutputCharacter(
			IntPtr hConsoleOutput,
			[In][MarshalAs(UnmanagedType.LPArray, SizeParamIndex=2)]char[] lpCharacter,
			int nLength,
			CoordInternal dwWriteCoord,
			ref int lpNumberOfCharsWritten);
	}
}
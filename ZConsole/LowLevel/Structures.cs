namespace ZConsole.LowLevel
{
	using System;
	using System.Runtime.InteropServices;

	/// <summary>
    /// Reports keyboard events in a console input record.
    /// </summary>
 	[StructLayout(LayoutKind.Explicit)]
	public struct		ConsoleKeyEventInfo
	{ 
		[FieldOffset(0)] private	int		bKeyDown;
		[FieldOffset(4)] private	short	wRepeatCount;
		[FieldOffset(6)] private	short	wVirtualKeyCode;
		[FieldOffset(8)] private	short	wVirtualScanCode;
		[FieldOffset(10)]private	char	cUnicodeChar;
		[FieldOffset(10)]private	short	wUnicodeChar;
		[FieldOffset(10)]private	byte	bAsciiChar;
		[FieldOffset(12)]private	int		dwControlKeyState;

        /// <summary>
        /// Gets or sets a value indicating whether this is a key down or key up event.
        /// </summary>
		public bool			IsKeyDown
		{
			get { return bKeyDown != 0; }
			set { bKeyDown = (value ? 1 : 0); }
		}

        /// <summary>
        /// Gets or sets a value indicating that a key is being held down.
        /// </summary>
		public short		RepeatCount
		{
			get { return wRepeatCount; }
			set { wRepeatCount = value; }
		}

        /// <summary>
        /// Gets or sets a value that identifies the given key in a device-independent manner.
        /// </summary>
		public ConsoleKey	VirtualKeyCode
		{
			get { return (ConsoleKey)wVirtualKeyCode; }
			set { wVirtualKeyCode = (short)value; }
		}

        /// <summary>
        /// Gets or sets the hardware-dependent virtual scan code.
        /// </summary>
		public short		VirtualScanCode
		{
			get { return wVirtualScanCode; }
			set { wVirtualScanCode = value; }
		}

        /// <summary>
        /// Gets or sets the Unicode character for this key event.
        /// </summary>
		public char			UnicodeChar
		{
			get { return cUnicodeChar; }
			set { cUnicodeChar = value; }
		}

        /// <summary>
        /// Gets or sets the ASCII key for this key event.
        /// </summary>
		public byte			AsciiChar
		{
			get { return bAsciiChar; }
			set { bAsciiChar = value; }
		}

        /// <summary>
        /// Gets or sets a value specifying the control key state for this key event.
        /// </summary>
		public ConsoleControlKeyState ControlKeyState
		{
			get { return (ConsoleControlKeyState)dwControlKeyState; }
			set { dwControlKeyState = (int)value; }
		}
    }

    /// <summary>
    /// Reports a mouse event in a console input record.
    /// </summary>
	[StructLayout(LayoutKind.Explicit)]
	public struct		ConsoleMouseEventInfo
	{
		[FieldOffset(0)] private CoordInternal	dwMousePosition;
		[FieldOffset(4)] private int	dwButtonState;
		[FieldOffset(8)] private int	dwControlKeyState;
		[FieldOffset(12)]private int	dwEventFlags;

        /// <summary>
        /// Gets or sets a value indicating the current mouse position.
        /// </summary>
		public CoordInternal					MousePosition
		{
			get { return dwMousePosition; }
			set { dwMousePosition = value; }
		}

        /// <summary>
        /// Gets or sets a value indicating the state of the mouse buttons.
        /// </summary>
		public ConsoleMouseButtonState	ButtonState
		{
			get { return (ConsoleMouseButtonState)dwButtonState; }
			set { dwButtonState = (int)value; }
		}

        /// <summary>
        /// Gets or sets a value indicating the state of the keyboard control keys.
        /// </summary>
		public ConsoleControlKeyState	ControlKeyState
		{
			get { return (ConsoleControlKeyState)dwControlKeyState; }
			set { dwControlKeyState = (int)value; }
		}

        /// <summary>
        /// Gets or sets a value indicating the type of mouse event.
        /// </summary>
		public ConsoleMouseEventType	EventFlags
		{
			get { return (ConsoleMouseEventType)dwEventFlags; }
			set { dwEventFlags = (int)value; }
		}
    }

    /// <summary>
    /// Reports window buffer sizing events in a console input record.
    /// </summary>
	[StructLayout(LayoutKind.Explicit)]
	public struct		ConsoleWindowBufferSizeEventInfo
	{
		[FieldOffset(0)]
        private CoordInternal	dwSize;

        /// <summary>
        /// Gets or sets a value indicating the size of the screen buffer,
        /// in character cell columns and rows.
        /// </summary>
		public CoordInternal	Size
		{
			get { return dwSize; }
			set { dwSize = value; }
		}
    }

    /// <summary>
    /// Reports menu events in a console input record.
    /// Use of this event type is not documented.
    /// </summary>
	[StructLayout(LayoutKind.Explicit)]
	public struct		ConsoleMenuEventInfo
	{
        [FieldOffset(0)]
		private int		dwCommandId;

        /// <summary>
        /// The id of the menu command.  Possible values are undocumented.
        /// </summary>
		public int		CommandId
		{
			get { return dwCommandId; }
			set { dwCommandId = value; }
		}
    }

    /// <summary>
    /// Reports focus events in a console input record.
    /// Use of this event type is not documented.
    /// </summary>
	[StructLayout(LayoutKind.Explicit)]
	public struct		ConsoleFocusEventInfo
	{
        [FieldOffset(0)]
		private uint	bSetFocus;

        /// <summary>
        /// Gets or sets a value indicating whether focus is gained or lost.
        /// This value is not documented.
        /// </summary>
		public bool		SetFocus
		{
			get { return (bSetFocus != 0); }
			set { bSetFocus = Convert.ToUInt32(value); }
		}
    }

    /// <summary>
    /// Used to report events in the console input buffer.
    /// </summary>
	[StructLayout(LayoutKind.Explicit)]
	public struct		ConsoleInput
	{
		[FieldOffset(0)]private short	evType;
        // NOTE:  Careful here.  Structure packing aligns the
        // Event union (see WinCon.h) on a 4-byte boundary.

        /// <summary>
        /// Key event information if this is a keyboard event.
        /// </summary>
		[FieldOffset(4)]public ConsoleKeyEventInfo		KeyEvent;
        /// <summary>
        /// Mouse event information if this is a mouse event.
        /// </summary>
		[FieldOffset(4)]public ConsoleMouseEventInfo	MouseEvent;
        /// <summary>
        /// Window buffer size information if this is a window buffer size event.
        /// </summary>
		[FieldOffset(4)]public ConsoleWindowBufferSizeEventInfo WindowBufferSizeEvent;
        /// <summary>
        /// Menu event information if this is a menu event.
        /// </summary>
		[FieldOffset(4)]public ConsoleMenuEventInfo		MenuEvent;
        /// <summary>
        /// Focus event information if this is a focus event.
        /// </summary>
		[FieldOffset(4)]public ConsoleFocusEventInfo	FocusEvent;

        /// <summary>
        /// Gets or sets a value that specifies the type of event.
        /// </summary>
		public ConsoleInputEventType EventType
		{
			get { return (ConsoleInputEventType)evType; }
			set { evType = (short)value; }
		}
    }

	 /// <summary>
    /// Contains information about a console screen buffer.
    /// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public class		ConsoleScreenBufferInfo
	{
		public CoordInternal dwSize;
		public CoordInternal dwCursorPosition;
		public short wAttributes;
		[MarshalAs(UnmanagedType.Struct)]public RectInternal srWindow;
		public CoordInternal dwMaximumWindowSize;
    }

    /// <summary>
    /// Contains information for a console selection.
    /// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public class		ConsoleSelectionInfo
	{
		public int		dwFlags;
		public CoordInternal	dwSelectionAnchor;
		[MarshalAs(UnmanagedType.Struct)]public RectInternal srSelection;
    }

    /// <summary>
    /// Contains information about a console font.
    /// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public class		ConsoleFontInfo
	{
		public int		nFont;
		public CoordInternal	dwFontSize;
    }


	/// <summary>
    /// Defines the coordinates of a character cell in a console window.
    /// </summary>
	[StructLayout(LayoutKind.Explicit)]
	public struct		CoordInternal
	{
		[FieldOffset(0)]	private short x;
		[FieldOffset(2)]	private short y;

		public CoordInternal(int xPosition, int yPosition)
		{
			x = (short)xPosition;
			y = (short)yPosition;
		}
		
		public int	X	{	get { return x; }	set { x = (short)value; } }
		public int	Y	{	get { return y; }	set { y = (short)value; } }

		public CoordInternal	Move(int dx, int dy)
		{
			return new CoordInternal(X + dx, Y + dy);
		}

	}

	/// <summary>
    /// Defines the rectangle.
    /// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public class		RectInternal
	{
		private short left;
		private short top;
		private short right;
		private short bottom;

		public RectInternal(int left, int top, int right, int bottom)
		{
			this.left	= (short)left;
			this.top	= (short)top;
			this.right  = (short)right;
			this.bottom = (short)bottom;
		}

		public int		Left	{	get { return left;	}	set { left = (short)value;	}	}
		public int		Top		{	get { return top;	}	set { top = (short)value;	}	}
		public int		Right	{	get { return right; }	set { right = (short)value;	}	}
		public int		Bottom	{	get { return bottom; }	set { bottom = (short)value; }	}
    }

	/// <summary>
	/// Thic class contains information about the console cursor.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public class		ZCursorInfo
	{
		#region Private Fields and Constructors

		private int  cursorSize;

		/// <summary>
        /// Creates a new instance of the ConsoleCursorInfo class.
        /// </summary>
        /// <param name="visible">Visible flag. Set to true to make the cursor visible.</param>
        /// <param name="size">Percentage (from 1 to 100) of the character cell that is 
        /// filled by the cursor.</param>
		public ZCursorInfo(bool visible, int size)
		{
			cursorSize = size;
			Visible = visible;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets or sets a value indicating whether the cursor is visible.
		/// </summary>
		public bool		Visible		{ get; set; }

		/// <summary>
        /// Gets or sets a value that indicates the percentabel (from 1 to 100) of the character cell that is filled by the cursor.
        /// </summary>
        public int		Size		{ get { return cursorSize; } set { cursorSize = Tools.SetIntoRange(value, 0, 100); }}

		#endregion		
    }
}
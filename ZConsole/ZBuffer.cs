namespace ZConsole
{
	using System;
	using System.Collections.Generic;
	using LowLevel;


	public static class ZBuffer
	{
		private static readonly Dictionary<string, ZCharInfo[,]> buffers = new Dictionary<string, ZCharInfo[,]>();


		public static void		CheckBuffer(string bufferName)
		{
			if (!buffers.ContainsKey(bufferName))
				throw new Exception("Cannot find buffer with specified name: " + bufferName + ".");
		}

		public static void		ReadBuffer(string bufferName, Rect rect)
		{
			ReadBuffer(bufferName, 0, 0, rect.Left, rect.Top, rect.Right, rect.Bottom);
		}

		/// <summary>
        /// Reads a rectangular block of character and attribute information from the screen buffer into the passed array.
        /// </summary>
        /// <param name="bufferName">Name of the buffer to read to.</param>
        /// <param name="left">Column position of the top-left corner of the screen buffer area from which characters are to be read.</param>
        /// <param name="top">Row position of the top-left corner of the screen buffer area from which characters are to be read.</param>
        /// <param name="width">Width of the screen buffer area from which characters are to be read.</param>
        /// <param name="height">Height of the screen buffer area from which characters are to be read.</param>
		public static void		ReadBuffer(string bufferName, int left, int top, int width, int height)
		{
			ReadBuffer(bufferName, 0, 0, left, top, left+width-1, top+height-1);
		}

		/// <summary>
        /// Reads a rectangular block of character and attribute information from the screen buffer into the passed array.
        /// </summary>
        /// <param name="bufferName">Name of the buffer to read to.</param>
        /// <param name="bufferX">The column position in the array where the first character is to be placed.</param>
        /// <param name="bufferY">The row position in the array where the first character is to be placed.</param>
        /// <param name="left">Column position of the top-left corner of the screen buffer area from which characters are to be read.</param>
        /// <param name="top">Row position of the top-left corner of the screen buffer area from which characters are to be read.</param>
        /// <param name="right">Column position of the bottom-right corner of the screen buffer area from which characters are to be read.</param>
        /// <param name="bottom">Row position of the bottom-right corner of the screen buffer area from which characters are to be read.</param>
		public static void		ReadBuffer(string bufferName, int bufferX, int bufferY, int left, int top, int right, int bottom)
		{
			var buffer = new ZCharInfo[bottom-top+1,right-left+1];
			var bufferSize = new CoordInternal(buffer.GetLength(1), buffer.GetLength(0));
            var bufferPos  = new CoordInternal(bufferX, bufferY);
            var readRegion = new RectInternal(left, top, right, bottom);
			WinCon.ReadConsoleOutput(ZOutput.hConsoleOutput, buffer, bufferSize, bufferPos, readRegion);

			if (buffers.ContainsKey(bufferName))
			{
				buffers[bufferName] = buffer;
			}
			else
			{
				buffers.Add(bufferName, buffer);
			}
		}

		/// <summary>
        /// Writes character and attribute information to a rectangular portion of the screen buffer.
        /// </summary>
        /// <param name="bufferName">Name of the buffer to write from.</param>
        /// <param name="left">Column position of the top-left corner of the screen buffer area where characters are to be written.</param>
        /// <param name="top">Row position of the top-left corner of the screen buffer area where characters are to be written.</param>
		public static void		WriteBuffer(string bufferName, int left, int top)
		{
			CheckBuffer(bufferName);
			var buffer = buffers[bufferName];
			WriteBuffer(bufferName, 0, 0, left, top, left + buffer.GetLength(1)-1, top + buffer.GetLength(0)-1);
		}

		/// <summary>
        /// Writes character and attribute information to a rectangular portion of the screen buffer.
        /// </summary>
        /// <param name="bufferName">Name of the buffer to write from.</param>
        /// <param name="bufferX">Column position of the first character to be written from the array.</param>
        /// <param name="bufferY">Row position of the first character to be written from the array.</param>
        /// <param name="left">Column position of the top-left corner of the screen buffer area where characters are to be written.</param>
        /// <param name="top">Row position of the top-left corner of the screen buffer area where characters are to be written.</param>
        /// <param name="right">Column position of the bottom-right corner of the screen buffer area where characters are to be written.</param>
        /// <param name="bottom">Row position of the bottom-right corner of the screen buffer area where characters are to be written.</param>
		public static void		WriteBuffer(string bufferName, int bufferX, int bufferY, int left, int top, int right, int bottom)
		{
			CheckBuffer(bufferName);
			var buffer = buffers[bufferName];
			var bufferSize = new CoordInternal (buffer.GetLength(1), buffer.GetLength(0));
            var bufferPos = new CoordInternal(bufferX, bufferY);
            var writeRegion = new RectInternal(left, top, right, bottom);
			WinCon.WriteConsoleOutput(ZOutput.hConsoleOutput, buffer, bufferSize, bufferPos, writeRegion);
		}

		public static void		SaveBuffer(string bufferName, ZCharInfo[,] buffer)
		{
			if (buffers.ContainsKey(bufferName))
			{
				buffers[bufferName] = buffer;
			}
			else
			{
				buffers.Add(bufferName, buffer);
			}
		}

		public static ZCharInfo[,] BackupBuffer(string bufferName)
		{
			CheckBuffer(bufferName);

			var buffer = buffers[bufferName];
			var sizeX = buffer.GetLength(1);
			var sizeY = buffer.GetLength(0);

			var resultBuffer = new ZCharInfo[sizeY,sizeX];
			for (var i = 0; i < sizeY; i++)
				for (var j = 0; j < sizeX; j++)
					resultBuffer[i, j] = buffer[i, j];

			return resultBuffer;
		}


		public static void		CreateBuffer(string bufferName, int width, int height)
		{
			var newBuffer = new ZCharInfo[height,width];
			SaveBuffer(bufferName, newBuffer);
		}

		public static void		PrintToBuffer(string bufferName, int x, int y, char charToWrite, Color foreColor, Color backColor = Color.Black)
		{
			CheckBuffer(bufferName);
			buffers[bufferName][y,x] = new ZCharInfo(charToWrite, new ZCharAttribute(foreColor, backColor));
		}

		public static void		PrintToBuffer(string bufferName, int x, int y, string text, Color foreColor, Color backColor = Color.Black)
		{
			CheckBuffer(bufferName);
			for (var i = 0; i < text.Length; i++)
			{
				buffers[bufferName][y,x+i] = new ZCharInfo(text[i], new ZCharAttribute(foreColor, backColor));
			}
		}

		public static void		PrintToBuffer(int x, int y, char charToWrite, Color foreColor, Color backColor = Color.Black)
		{
			PrintToBuffer("defaultBuffer", x, y, charToWrite, foreColor, backColor);
		}

		public static void		PrintToBuffer(int x, int y, string text, Color foreColor, Color backColor = Color.Black)
		{
			PrintToBuffer("defaultBuffer", x, y, text, foreColor, backColor);
		}
	}
}
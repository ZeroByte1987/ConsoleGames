namespace ZConsole
{
	using System;
	using LowLevel;


	public static class ZInput
	{
		public static IntPtr	hConsoleInput;


		public static string			ReadLine(int x, int y, int maxLength, Color foreColor = Color.Gray, Color backColor = Color.Black,
			bool allowEmpty = true, string defaultText = "")
		{
			ZCursor.SetPosition(x, y);
			ZColors.SetColor(foreColor);
			ZColors.SetBackColor(backColor);

			var result = defaultText;
			ZOutput.Print(result);
			x += result.Length;

			while (true)
			{
				var key = ReadKeyInfo();
				switch (key.Key)
				{
					case ConsoleKey.Enter	:
						if (result != string.Empty  ||  allowEmpty)
							return result;
						break;

					case ConsoleKey.Escape	:
						if (allowEmpty)
							return string.Empty;
						break;
					
					case ConsoleKey.Backspace:
						if (result.Length > 0)
						{
							result = result.Substring(0, result.Length-1);
							x--;
							ZOutput.Print(x, y, ' ');
							ZCursor.SetPosition(x, y);
						}
						break;

					default:
						if (result.Length < maxLength  &&  !char.IsControl(key.KeyChar))
						{
							result += key.KeyChar;
							ZOutput.Print(key.KeyChar);
							x++;
							ZCursor.SetPosition(x, y);
						}
						break;
				}
			}
		}


		public static ConsoleInput		WaitForInput()
		{
			while (true)
			{
				var input = ReadInput();
				if (input.EventType == ConsoleInputEventType.KeyEvent  ||  input.EventType == ConsoleInputEventType.MouseEvent)
				{
					return input;
				}
			}
		}

		public static ConsoleInput		ReadInput()
		{
			var result = new ConsoleInput[1];
			var resultCount = 0;
			WinCon.ReadConsoleInput(hConsoleInput, result, 1, ref resultCount);
			return result[0];
		}


		public static ConsoleKey		ReadKey()
		{
			return Console.ReadKey(true).Key;
		}

		public static void				ReadKeyIf(bool statement)
		{
			if (statement)
			{
				Console.ReadKey(true);
			}
		}

		public static char				ReadChar()
		{
			return Console.ReadKey(true).KeyChar;
		}

		public static ConsoleKeyInfo	ReadKeyInfo()
		{
			return Console.ReadKey(true);
		}
	}
}
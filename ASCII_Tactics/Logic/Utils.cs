﻿namespace ASCII_Tactics.Logic
{
	public static class Utils
	{
		public  static void Swap<T>(ref T value1, ref T value2)
		{
			var temp = value1; 
			value1 = value2; 
			value2 = temp;
		}
	}
}
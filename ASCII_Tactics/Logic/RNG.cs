namespace ASCII_Tactics.Logic
{
	using System;
	using ZConsole;
	using ZLinq;


	public static class RNG
	{
		private static Random	backupRandomGenerator;
		private static Random	randomGenerator;

		
		public static void		Initialize()
		{
			randomGenerator = new Random();
			backupRandomGenerator = randomGenerator;
		}

		public static void		SwitchMode(bool useSeed, int seed = 0)
		{
			if (useSeed)
			{
				backupRandomGenerator = randomGenerator;
				randomGenerator = new Random(seed);
			}
			else
			{
				randomGenerator = backupRandomGenerator;
			}
		}

		public static int		GetNumber(int maxValue)
		{
			return randomGenerator.Next(maxValue);
		}
		public static int		GetNumber(Range range, params int[] valuesToExclude)
		{
			return GetNumber(range.Min, range.Max, valuesToExclude);
		}
		public static int		GetNumber(int minValue, int maxValue, params int[] valuesToExclude)
		{
			for (var i = 0; i < 100; i++)
			{
				var value = randomGenerator.Next(minValue, maxValue+1);
				if (valuesToExclude.All(w => w != value))
				{
					return value;
				}
			}
			return -1;
		}
		

		public static int		GetSeed()
		{
			return (int)DateTime.Now.Ticks;
		}
	}
}
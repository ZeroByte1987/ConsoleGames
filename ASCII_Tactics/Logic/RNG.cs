namespace ASCII_Tactics.Logic
{
	using System;
	using ZConsole;


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
			return randomGenerator.Next(0, maxValue);
		}
		public static int		GetNumber(int minValue, int maxValue)
		{
			return randomGenerator.Next(minValue, maxValue);
		}
		public static int		GetNumber(Range range)
		{
			return GetNumber(range.Min, range.Max+1);
		}

		public static int		GetSeed()
		{
			return (int)DateTime.Now.Ticks;
		}
	}
}
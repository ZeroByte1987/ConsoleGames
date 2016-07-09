namespace ZFrontier.Logic
{
	using System;
	using ZConsole;


	public static class RNG
	{
		#region Fields & Properties

		private static Random	backupRandomGenerator;
		private static Random	randomGenerator;

		public static int		DiceSize = 6;

		#endregion


		#region Initialization and SwitchMode

		public static void		Initialize()
		{
			Initialize(DiceSize);
		}

		public static void		Initialize(int diceSize)
		{
			DiceSize = diceSize;
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

		#endregion


		#region Main Methods

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
			return GetNumber(range.Min, range.Max);
		}

		public static int		GetDice()
		{
			return randomGenerator.Next(DiceSize)+1;
		}
		public static int		GetDiceZero()
		{
			return randomGenerator.Next(DiceSize);
		}

		public static int		GetDiceDiv2()
		{
			return randomGenerator.Next(DiceSize) / 2 + 1;
		}
		public static int		GetDiceDiv2Zero()
		{
			return randomGenerator.Next(DiceSize) / 2;
		}

		public static int		GetDice2dX(int diceCount)
		{
			return (randomGenerator.Next(DiceSize*diceCount)+1) + (randomGenerator.Next(6*diceCount)+1);
		}
		public static int		GetDiceX(int diceCount)
		{
			return (randomGenerator.Next(DiceSize*diceCount)+1);
		}
		
		public static int		GetSeed()
		{
			return (int)DateTime.Now.Ticks;
		}

		#endregion
	}
}

namespace ASCII_Tactics.Config
{
	public static class ActionCostConfig
	{
		public static double[] DirectionsCostFactor = new [] { 1.5, 1, 1.5, 2, 2.5, 2, 2.5, 2 };

		public static int	BasicMove		= 2;
		public static int	Turn			= 1;
		public static int	SitDown			= 4;
		public static int	StandUp			= 6;

		public static float	DiagonalFactor	= 1.5f;
		public static int	CrouchFactor	= 2;

		public static int	OpenCloseDoor	= 6;
		public static int	UseStairs		= 10;
	}
}
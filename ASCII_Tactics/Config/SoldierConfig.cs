namespace ASCII_Tactics.Config
{
	using System.IO;
	using System.Text;
	using ZConsole;
	using ZLinq;


	public static class SoldierConfig
	{
		public static string[] Names;


		public static Range	HitPointsRange		= new Range(40, 60);
		public static Range	TimeUnitsRange		= new Range(450, 650);
		public static Range	AccuracyRange		= new Range(30, 70);
		public static Range	StrengthRange		= new Range(35, 65);

		public static int	PointsPerSoldier	= 200;


		public static void Initialize()
		{
			const string fileName = @"Data\Names.txt";
			var codePage = int.Parse(File.ReadAllLines(fileName)[0]);
			var encoding = Encoding.GetEncoding(codePage);
			Names = File.ReadAllLines(fileName, encoding).Where(a => !string.IsNullOrEmpty(a) && a.Any(c => !char.IsDigit(c))).ToArray();
		}
	}
}
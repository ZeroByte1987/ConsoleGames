namespace ZDataBase.Logic
{
	using System;

	public static class Extensions
	{
		public static bool	EqualsIC(this string source, string stringToCompare)
		{
			return source.Equals(stringToCompare, StringComparison.InvariantCultureIgnoreCase);
		}
	}
}
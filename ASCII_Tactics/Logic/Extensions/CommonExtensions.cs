namespace ASCII_Tactics.Logic.Extensions
{
	using System.Collections.Generic;


	public static class CommonExtensions
	{
		public static T		GetRandom<T>(this List<T> list)
		{
			return list[RNG.GetNumber(list.Count)];
		}
	}
}
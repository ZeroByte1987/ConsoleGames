namespace ZFrontier.Objects.GameData
{
	using System.Collections.Generic;
	using Galaxy;
	using Logic;
	using ZConsole;
	using ZLinq;


	public static class		Helpers
	{
		public static StarSystemModel	GetRandom(this IEnumerable<StarSystemModel> allStars)
		{
			var starArray = allStars.ToArray();
			return starArray.ToArray()[RNG.GetNumber(starArray.Length)];
		}

		public static T					Last<T>(this List<T> list)
		{
			return list[list.Count - 1];
		}
	}


	#region Stats config

	public class NPC_StatsConfig
	{
		public Range	Bounty			{ get; set; }
		public Range	ShipsUsage		{ get; set; }
		public int		Bonus_Attack	{ get; set; }
		public int		Bonus_Defense	{ get; set; }
		public int		Bonus_ECM		{ get; set; }
		public int		EscapeChance	{ get; set; }
		public int		FineForAttack	{ get; set; }
	}

	#endregion

	#region Translation / Localization

	public class TranslationSet : Dictionary<string, string>
	{
	}

	public class LanguageSet : Dictionary<string, TranslationSet>
	{
	}

	#endregion

	#region Prices

	public class GoodsPriceSet : Dictionary<Merchandise, int>
	{
	}

	public class EquipPriceSet : Dictionary<Equipment, int>
	{
	}

	#endregion

	#region Cargo

	public class CargoDrop : List<KeyValuePair<Merchandise, int>>
	{
	}

	#endregion

	#region Legal Record classes

	public class LegalRecord
	{
		public int			FineAmount		{ get; set; }
		public LegalStatus	Status			{ get { return Enums.Get_Value<LegalStatus>(FineAmount); }}
	}

	public class LegalRecords : Dictionary<Allegiance, LegalRecord>
	{
		public LegalRecords Copy()
		{
			var result = new LegalRecords();
			foreach (var record in this)
				result.Add(record.Key, new LegalRecord { FineAmount = record.Value.FineAmount });
			return result;
		}
	}

	#endregion

	#region Legal Record classes

	public class MilitaryRecord
	{
		public int				Rating	{ get; set; }
		public MilitaryRank		Rank	{ get { return Enums.Get_Value<MilitaryRank>(Rating); }}
	}

	public class MilitaryRecords : Dictionary<Allegiance, MilitaryRecord>
	{
		public MilitaryRecords Copy()
		{
			var result = new MilitaryRecords();
			foreach (var record in this)
				result.Add(record.Key, new MilitaryRecord { Rating = record.Value.Rating });
			return result;
		}
	}

	#endregion
}

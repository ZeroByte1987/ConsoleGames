namespace ASCII_Tactics.Models
{
	using System.Collections.Generic;
	using ZLinq;

	public class Team
	{
		private const int MAX_TEAM_SIZE = 100;


		public string		Name		{ get; set; }
		public List<Unit>	Units		{ get; set; }

		public bool			IsPlayable	{ get; set; }



		public void			Add(Unit unit)
		{
			for (var i = 0; i < MAX_TEAM_SIZE; i++)
				if (Units.All(a => a.Id != i))
				{
					unit.Id = i;
					break;
				}

			var stats = unit.Stats;

			if (stats.TU == 0)	stats.TU = 50;
			if (stats.Accuracy == 0)	stats.Accuracy = 50;
			if (stats.Strength == 0)	stats.Strength = 50;
			if (stats.MaxHP == 0)			stats.MaxHP = 50;

			stats.CurrentHP = stats.MaxHP;
			stats.CurrentTU = stats.MaxTU;
			unit.Team = this;

			Units.Add(unit);
		}


		public Team(string teamName, bool isPlayable)
		{
			Name = teamName;
			IsPlayable = isPlayable;
			Units = new List<Unit>();
		}
	}
}

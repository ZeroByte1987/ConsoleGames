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
		
			if (unit.InitialMaxTU == 0) unit.InitialMaxTU = 50;
			if (unit.Accuracy == 0)		unit.InitialAccuracy = 50;
			if (unit.Strength == 0)		unit.Strength = 50;
			if (unit.MaxHP == 0)		unit.MaxHP = 50;

			unit.CurrentHP = unit.MaxHP;
			unit.CurrentTU = unit.MaxTU;
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

namespace ASCII_Tactics.Models
{
	using System.Collections.Generic;


	public class Team
	{
		public string		Name		{ get; set; }
		public List<Unit>	Units		{ get; set; }
		public bool			IsPlayable	{ get; set; }


		public Team(string teamName, bool isPlayable)
		{
			Name = teamName;
			IsPlayable = isPlayable;
			Units = new List<Unit>();
		}


		public void		Add(Unit unit)
		{
			unit.Team = this;
			Units.Add(unit);
		}
	}
}
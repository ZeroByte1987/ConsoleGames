namespace ASCII_Tactics.Models.Map
{
	using System.Collections.Generic;


	public class SpaceStation
	{
		public List<Level>			Levels		{ get; set; }
		public List<ItemOnFloor>	Items		{ get; set; }

		public SpaceStation()
		{
			Levels = new List<Level>();
			Items  = new List<ItemOnFloor>();
		}
	}
}
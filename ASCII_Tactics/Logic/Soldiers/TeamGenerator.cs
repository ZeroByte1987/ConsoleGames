namespace ASCII_Tactics.Logic.Soldiers
{
	using System.Collections.Generic;
	using Config;
	using Models;
	using Models.CommonEnums;
	using Models.Items;
	using Models.Map;
	using Models.Tiles;
	using Models.UnitData;
	using ZLinq;


	public static class TeamGenerator
	{
		private static readonly List<string> _usedNames = new List<string>();


		public static Team		CreateTeam(string teamName, bool isPlayable)
		{
			var team = new Team(teamName, isPlayable);

			for (var i = 0; i < GameConfig.TeamSize; i++)
			{
				var freeNames = SoldierConfig.Names.Where(w => !_usedNames.Contains(w)).ToArray();
				var name = freeNames[RNG.GetNumber(freeNames.Length)];
				_usedNames.Add(name);

				var unit = new Unit(name)
				{
					Stats = new UnitStats
						{
							TU		 = RNG.GetNumber(SoldierConfig.TimeUnitsRange),
							MaxHP	=  RNG.GetNumber(SoldierConfig.HitPointsRange),
							Accuracy = RNG.GetNumber(SoldierConfig.AccuracyRange),
							Strength = RNG.GetNumber(SoldierConfig.StrengthRange)
						},

					View = ViewLogic.Initialize(GameConfig.DefaultViewWidth, GameConfig.DefaultViewDistance, RNG.GetNumber(8)),
					Inventory = new Inventory(ItemConfig.DefaultInventories[i].Select(w => new Item(w)))
				};

				unit.Stats.CurrentHP = unit.Stats.MaxHP;
				unit.Inventory.ActiveItem = unit.Inventory.FirstOrDefault();
				team.Add(unit);
			}

			return team;
		}

		
		public static void		LocateTeamInStation(Team team, SpaceStation station, int levelId)
		{
			var level = station.Levels[levelId];
			var corner = (Corner)RNG.GetNumber(0, 3, (int)level.StairsDownLocation);

			var coordX = corner == Corner.TopLeft  ||  corner == Corner.BottomLeft ? 3 : MapConfig.LevelSize.Width - 6;
			var coordY = corner == Corner.TopLeft  ||  corner == Corner.TopRight ? 3 : MapConfig.LevelSize.Height - 6;

			for (var i = 0; i < 2; i++)
				for (var j = 0; j < 2; j++)
				{
					var unit = team.Units[i*2 + j];
					unit.Position = new Position(levelId, coordX + j*2, coordY + i*2);
					unit.View.Direction = corner == Corner.TopLeft ? 4 : corner == Corner.TopRight ? 6 : corner == Corner.BottomLeft ? 2 : 0;
					level.Map[coordY + i*2, coordX + j*2] = new Tile("Empty");
				}
		}
	}
}
namespace ZFrontier.Objects.Galaxy
{
	using System;
	using GameData;
	using Logic;
	using ZLinq;


	public class GlobalEvent
	{
		#region Public Properties

		public int				StarSystemIndex		{ get; set; }
		public GlobalEventType	Event				{ get; set; }
		public DateTime			EventDate			{ get; set; }
		public int				EventValue			{ get; set; }

		public StarSystemModel	StarSystem			{ get { return ZFrontier.Galaxy.StarSystems[StarSystemIndex / GameConfig.CurrentGalaxySizeX, StarSystemIndex % GameConfig.CurrentGalaxySizeX]; }}

		#endregion


		#region Conbstructors

		public GlobalEvent(StarSystemModel starSystem, GlobalEventType @event, int eventValue, DateTime date)
		{
			StarSystemIndex = starSystem.Coords.Y * GameConfig.CurrentGalaxySizeX + starSystem.Coords.X;
			Event = @event;
			EventValue = eventValue;
			EventDate = date;
		}

		public GlobalEvent(int starSystemIndex, GlobalEventType @event, int eventValue, DateTime date)
		{
			StarSystemIndex = starSystemIndex;
			Event = @event;
			EventValue = eventValue;
			EventDate = date;
		}

		#endregion


		#region Public Methods

		public void		Print()
		{
			var textEventValue = (Event == GlobalEventType.IllegalAdd || Event == GlobalEventType.IllegalRemove)
				                     ? Enums.Get_Name((Merchandise) EventValue)
				                     : EventValue.ToString();
			ZFrontier.EventLog.Print("GlobalEvent_" + Event, StarSystem.Name, textEventValue);
		}

		#endregion


		public static void     Create(GalaxyModel galaxy, StarSystemModel currentSystem)
	    {
			galaxy.LastEventDate = galaxy.GameDate;
			var globalEvent = Enums.All_GlobalEvents.Get_Random();
			if (globalEvent == GlobalEventType.Normal)
				globalEvent = GlobalEventType.AlienInvasion;

			#region If it's time for Great Alien Attack

			if (galaxy.AlienStrength != AlienStrength.FatherIsKilled  &&
			    (galaxy.GameDate > GameConfig.AlienAttackDate
			 ||  galaxy.AlienStrength == AlienStrength.FatherIsHere
			 || (globalEvent == GlobalEventType.AlienInvasion  &&  galaxy.Get_AllSystems().Count(a => a.CurrentEvent == GlobalEventType.AlienInvasion) > GameConfig.AlienAttackTriggerCount)))
			{
				galaxy.AlienStrength = AlienStrength.FatherIsHere;
				globalEvent = GlobalEventType.AlienInvasion;
			}

			#endregion

			#region Get star system and event duration

			var system = galaxy.Get_RandomSystemForEvent(globalEvent);
		    
            if (globalEvent == GlobalEventType.Epidemy  ||  globalEvent == GlobalEventType.Starvation  ||  globalEvent == GlobalEventType.CivilWar)
            {
                system.CurrentEvent = globalEvent;
                system.EventDuration = GameConfig.BaseGlobalEventDuration + RNG.GetDice();
	            system.EventEndDate = galaxy.GameDate.AddDays(system.EventDuration);
            }
            if (globalEvent == GlobalEventType.AlienInvasion)
                system.CurrentEvent = globalEvent;

		    var eventValue = system.EventDuration;

			#endregion

			#region Process the event impact

			switch (globalEvent)
	        {
                case GlobalEventType.AlienInvasion:	break;
                case GlobalEventType.LevelUp	:	system.TechLevel++;		eventValue = system.TechLevel;	break;
				case GlobalEventType.LevelDown	:	system.TechLevel--;		eventValue = system.TechLevel;	break;
                case GlobalEventType.IllegalAdd	:	
					var merchToAdd = Enums.All_MerchandiseIllegal.Where(a => !system.IllegalGoods.Contains(a)).ToList().Get_Random();
					system.IllegalGoods.Add(merchToAdd);
					eventValue = (int) merchToAdd;
					break;
				case GlobalEventType.IllegalRemove:	
					var merchToRemove = system.IllegalGoods.Get_Random();
					system.IllegalGoods.Remove(merchToRemove);
					eventValue = (int) merchToRemove;
					break;
	        }

			#endregion

			#region Add the event to EventLog and print it if needed

			var newGlobalEvent = new GlobalEvent(system, globalEvent, eventValue, galaxy.GameDate);
			galaxy.GlobalEventLog.Add(newGlobalEvent);
			if (globalEvent == GlobalEventType.AlienInvasion  ||  (Enums.All_GlobalEventsWithDuration.Contains(globalEvent)  &&  system.Allegiance == currentSystem.Allegiance))
			{
				newGlobalEvent.Print();
			}

			#endregion
	    }
	}
}

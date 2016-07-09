namespace ZFrontier.Logic.UI
{
	using System;
	using System.Collections.Generic;
	using Common;
	using Objects.Galaxy;
	using Objects.GameData;
	using Objects.Planet;
	using Objects.Units;
	using ZConsole;
	using ZLinq;


	public class ActionPanel : AreaUI_Basic
	{
		#region Menu Items

		public static string Common_Quit;

		public static string RepairShop_RepairAll;
		public static string RepairShop_RepairSingle;

		public static string Tavern_LatestNews;
		public static string Tavern_Rumors;
		public static string Tavern_SpecialServices;

		public static string MilitaryHQ_About;

		public static string FinesPayment_PayAllFines;

		#endregion

		#region Private Fields

		private static PlayerModel		Player				{ get { return ZFrontier.Player;			}}
		private static GalaxyModel		Galaxy				{ get { return ZFrontier.Galaxy;			}}
		private static StarSystemModel	CurrentSystem		{ get { return ZFrontier.CurrentStarSystem; }}
		private static int				TechLevel			{ get { return CurrentSystem.TechLevel;		}}
		private static GoodsPriceSet	GoodsPrices			{ get { return GameConfig.GoodsPrices[TechLevel];	}}
		private static LegalRecord		CurrentLegalRecord	{ get {	return Player.LegalRecords[CurrentSystem.Allegiance];	}}
		private static PlayerStats		PlayerStats			{ get { return ZFrontier.PlayerStats;		}}
		private static EventLog			EventLog			{ get { return ZFrontier.EventLog;			}}
		private static GalaxyMap		GalaxyMap			{ get { return ZFrontier.GalaxyMap;			}}
		private readonly ZMenu.Options	DefaultMenuOptions;

		private readonly int xMenu, yMenu;

		#endregion

		#region Constructors
	
		public ActionPanel(Rect actionPanelArea) : base(actionPanelArea)
		{
			AreaType = AreaUI.ActionPanel;
			xMenu = AreaRect.Left + 20;
			yMenu = AreaRect.Top  + 1;

			DefaultMenuOptions = new ZMenu.Options
			{
				Mode = ZMenu.MenuMode.ShortkeysAndArrows,
				FrameSpacingHorizontal = 2,
				UseSelectedBackColor = true,
				ColorScheme = new ZMenu.ColorScheme{ FrameForeColor = Color.Green, CaptionBackColor = Color.DarkGreen, CaptionForeColor = Color.Yellow },
				FrameOptions = new ZFrame.Options
					{
						FrameType = FrameType.Double,
						Width = 10, 
						Height = 4,
					},						
			};
		}

		#endregion


		#region Main Methods

		public void			PlanetMainMenu()
		{
			PlayerStats.Draw_PlayerStats();
			
			ClearArea();
			var currentPosition = 0;
			var exitFlag = false;
			while (!exitFlag)
			{
				var menuResult =  ZMenu.GetMenuResult(xMenu, yMenu,
					new ZMenu.MenuItem(Lang["Common_YourAction"])
						{
							Options = DefaultMenuOptions,
							ChildMenuItems = new ZMenu.MenuItemList(currentPosition)
							{
								new ZMenu.MenuItem(Lang["Merchandise_Caption"]),
								new ZMenu.MenuItem(Lang["Equipment_Caption"]),
								new ZMenu.MenuItem(Lang["Repair_Caption"])			{	IsActive = Player.CurrentHP < Player.MaxHP		},
								new ZMenu.MenuItem(Lang["BBC_Caption"]),
								new ZMenu.MenuItem(Lang["Tavern_Caption"]),
								new ZMenu.MenuItem(Lang["Shipyard_Caption"]),
								new ZMenu.MenuItem(Lang["FinesPayment_Caption"])	{	IsActive = CurrentLegalRecord.FineAmount > 0	},
								new ZMenu.MenuItem(Lang["MilitaryHQ_Caption"])		{	IsActive = GameConfig.FractionsWithNavy.Contains(CurrentSystem.Allegiance)	},
								new ZMenu.MenuItem(Lang["Common_Quit"])
							}
						},
					ZFrontier.HotKeys);

				EventLog.ShadowOldMessages();

				currentPosition = menuResult.Index;
				var actions = new Dictionary<string, Action>
				{
					{	Lang["Merchandise_Caption"],	GoodsStore		},
					{	Lang["Equipment_Caption"],		EquipmentShop	},
					{	Lang["Repair_Caption"],			RepairShop		},
					{	Lang["BBC_Caption"],			BBC_Board		},
					{	Lang["Tavern_Caption"],			Tavern			},
					{	Lang["Shipyard_Caption"],		Shipyard		},
					{	Lang["FinesPayment_Caption"],	FinesPayment	},
					{	Lang["MilitaryHQ_Caption"],		MilitaryHQ		},
					{	Lang["Common_Quit"],			() => {	ClearArea();	exitFlag = true;	}},
					{	string.Empty,					() => {	ClearArea();	exitFlag = true;	}}
				};

				if (actions.ContainsKey(menuResult.Text))
					actions[menuResult.Text]();
			}

			EventLog.Print("Galaxy_ChooseSystem");
		}


		private void		GoodsStore()
		{
			#region Create list of shop items

			var shopItems	= Enums.All_Merchandise.Select(a => new ShopItem(Enums.Get_Name(a),  Tools.SetIntoRange(GoodsPrices[a] + CurrentSystem.PriceChanges[(int)(a)], 1, 100),  IsLegal(a), a)).ToList();

			if (Enums.All_GlobalEventsWithDuration.Contains(CurrentSystem.CurrentEvent))
			{
				var eventItem = shopItems.Single(a => (Merchandise)a.ItemObject == CurrentSystem.CurrentEvent.Get_MerchandiseForEvent());
		        eventItem.Price += GameConfig.GlobalEventPriceBonus;
		        eventItem.IsActive = true;
			}

			#endregion

			var shop = new Shop(Lang["Merchandise_Caption"], shopItems);
			ShopLogic.DoShopping(xMenu, yMenu, shop, GoodsStore_Buy, GoodsStore_Sell);
		}
		

		private void		EquipmentShop()
		{
			#region Create list of shop items

			var techLevelPrices = GameConfig.EquipmentPrices[TechLevel];
			var shopItems	= new List<ShopItem>
				{
					new ShopItem(Lang["Equipment_Missile"],		techLevelPrices[Equipment.Missile],		true,			Equipment.Missile),
					new ShopItem(Lang["Equipment_Laser"],		techLevelPrices[Equipment.LaserUnit],	true,			Equipment.LaserUnit),
					new ShopItem(Lang["Equipment_Shield"],		techLevelPrices[Equipment.Shield],		true,			Equipment.Shield),
					new ShopItem(Lang["Equipment_ECM"],			techLevelPrices[Equipment.ECM],			TechLevel > 2,	Equipment.ECM),
					new ShopItem(Lang["Equipment_Scanner"],		techLevelPrices[Equipment.Scanner],		TechLevel > 1,	Equipment.Scanner),
					new ShopItem(Lang["Equipment_MiningLaser"],	techLevelPrices[Equipment.MiningLaser],	TechLevel > 1,	Equipment.MiningLaser),
					new ShopItem(Lang["Equipment_EscapeBoat"],	techLevelPrices[Equipment.EscapeBoat],	TechLevel > 3,	Equipment.EscapeBoat),
				};

			#endregion

			var shop = new Shop(Lang["Equipment_Caption"], shopItems);
			ShopLogic.DoShopping(xMenu, yMenu, shop, EquipmentShop_Buy, EquipmentShop_Sell);
		}


		private void		RepairShop()
		{
			ClearArea();
			var exitFlag = false;
			var currentPosition = 0;

			while (!exitFlag)
			{
				#region If Player CANNOT get any repairs

				if (Player.Credits <= 0)
				{
					EventLog.Print("Repair_NoMoney");
					currentPosition = 2;
				}

				#endregion

				#region Get Menu Result

				var menuResult =  ZMenu.GetMenuResult(xMenu, yMenu,
					new ZMenu.MenuItem(Lang["Repair_Caption"])
						{
							Options = DefaultMenuOptions,
							ChildMenuItems = new ZMenu.MenuItemList(currentPosition)
							{
								new ZMenu.MenuItem(RepairShop_RepairAll)	{ IsActive = Player.Credits > 0  &&  Player.CurrentHP < Player.MaxHP },
								new ZMenu.MenuItem(RepairShop_RepairSingle) { IsActive = Player.Credits > 0  &&  Player.CurrentHP < Player.MaxHP },
								new ZMenu.MenuItem(Common_Quit),
							}
						},
					ZFrontier.HotKeys);

				#endregion

				#region Actions

				if (menuResult.Text == RepairShop_RepairAll)
				{
					var amountToRepair = Math.Min(Player.Credits, Player.MaxHP - Player.CurrentHP);
					Player.Credits	 -= amountToRepair;
					Player.CurrentHP += amountToRepair;
					EventLog.Print("Repair_RepairedMany", amountToRepair);
					currentPosition = 2;
				}
				else if (menuResult.Text == RepairShop_RepairSingle)
				{
					Player.Credits--;
					Player.CurrentHP++;
					EventLog.Print("Repair_RepairedOne");
					currentPosition = 1;
				}
				else
				{
					exitFlag = true;
				}

				PlayerStats.Draw_PlayerStats();

				#endregion
			}
		}


		private void		BBC_Board()
		{
			var shop = new Shop(Lang["BBC_Caption"], GetItemsForBBC(Enums.All_BBCAdvertTypes, false), ShopType.BBC, Lang["BBC_Header"], Lang["BBC_Footer"], hasQuit:true);
			ShopLogic.DoShopping(xMenu, yMenu, shop, BBC_Buy, null);
		}	


		private void		Tavern()
		{
			#region Initialization works

			var lastEvent = Galaxy.GlobalEventLog[Galaxy.GlobalEventLog.Count-1];
			RNG.SwitchMode(true, ZFrontier.Galaxy.RandomSeed);
			var eventForRumors = Galaxy.GlobalEventLog.Get_Random();

			var unexploredSystemsCount = Galaxy.Get_AllSystems().Count(a => !a.IsExplored);
			var isBuyMapAvailable = 
				RNG.GetDice() <= GameConfig.BuyMap_OptionChance 
				&&  Player.Credits >= GameConfig.BuyMap_Price  
				&&  unexploredSystemsCount > 0;
			RNG.SwitchMode(false);

			ClearArea();
			var exitFlag = false;
			var currentPosition = 0;

			#endregion

			var isNewsTold = false;
			var isRumorsTold = false;
			while (!exitFlag)
			{
				#region Get Menu Result
				
				var menuResult =  ZMenu.GetMenuResult(xMenu, yMenu,
					new ZMenu.MenuItem(Lang["Tavern_Caption"])
						{
							Options = DefaultMenuOptions,
							ChildMenuItems = new ZMenu.MenuItemList(currentPosition)
							{
								new ZMenu.MenuItem(Tavern_LatestNews),
								new ZMenu.MenuItem(Tavern_Rumors),
								new ZMenu.MenuItem(Tavern_SpecialServices) { IsActive = isBuyMapAvailable  &&  unexploredSystemsCount > 0 },
								new ZMenu.MenuItem(Common_Quit),
							}
						},
					ZFrontier.HotKeys);

				#endregion

				#region Actions

				#region Latest News

				if (menuResult.Text == Tavern_LatestNews)
				{
					if (!isNewsTold)
					{
						lastEvent.Print();
					}
					else
					{
						EventLog.Print("Tavern_NoNewNews");
					}
					isNewsTold = true;
					currentPosition = 0;
				}

				#endregion

				#region Rumors

				else if (menuResult.Text == Tavern_Rumors)
				{
					if (!isRumorsTold)
					{
						var @event = eventForRumors.Event;
						if (Enums.All_GlobalEventsWithDuration.Contains(@event))
						{
							EventLog.PrintPlain(
								string.Format(Lang["Tavern_Rumors_EventWithDuration"],
								eventForRumors.StarSystem.Name,
								GameConfig.Get_NPC_Name(NPC_Type.Trader),
								Enums.Get_Name(@event).ToLower(),
								Enums.Get_Name(@event.Get_MerchandiseForEvent()).ToLower()));
						}
						else
						{
							var resourceName = "Tavern_Rumors_" + @event;
							EventLog.Print(resourceName, eventForRumors.StarSystem.Name, Enums.Get_Name((Merchandise)eventForRumors.EventValue));
						}
					}
					else
					{
						EventLog.Print("Tavern_NoNewRumors");
					}

					isRumorsTold = true;
					currentPosition = 1;
				}

				#endregion

				#region Buy Map

				else if (menuResult.Text == Tavern_SpecialServices)
				{
					var buyMap = EventLog.Get_YesNo("Tavern_BuyMap", GameConfig.BuyMap_Price);
					if (buyMap)
					{
						Player.Credits -= GameConfig.BuyMap_Price;
						var systemNames = string.Empty;

						var countOfSystemToExplore = Math.Min(unexploredSystemsCount, GameConfig.BuyMap_SystemCount);
						for (var i = 0; i < countOfSystemToExplore; i++)
						{
							var system = Galaxy.Get_AllSystems().Where(a => !a.IsExplored).GetRandom();
							system.IsExplored = true;
							systemNames += "<" + system.Name + ">, ";
						}
						EventLog.Print("Tavern_BuyMapResult", systemNames.Substring(0, systemNames.Length-2));
						GalaxyMap.Draw_GalaxyMap();
					}
					unexploredSystemsCount = Galaxy.Get_AllSystems().Count(a => !a.IsExplored);
					currentPosition = unexploredSystemsCount > 0 ? 2 : 3;
				}

				#endregion

				else
				{
					exitFlag = true;
				}

				#endregion				
			}
		}


		private void		Shipyard()
		{
			#region Create list of shop items

			var baseItems = GameConfig.ShipModels
				.Select(ship => new ShopItem(
				string.Format("{0}  {1}   {2}    {3}", 
					ship.ModelName.PadRight(8, ' '), 
					ship.MaxHP.ToString().PadLeft(2, ' '), 
					ship.MaxCargoLoad.ToString().PadLeft(2, ' '), 
					ship.MaxMissiles), 
				ship.Price - Player.Ship.Price*Player.CurrentHP/Player.MaxHP, ship.ModelName != Player.Ship.ModelName, ship)).ToList();

			RNG.SwitchMode(true, ZFrontier.Galaxy.RandomSeed);
			var shopItems = baseItems.Where(shopItem => RNG.GetDice() <= GameConfig.ShipInShopChance).ToList();
			if (shopItems.Count == 0)
				shopItems.Add(baseItems[RNG.GetNumber(baseItems.Count)]);
			RNG.SwitchMode(false);
			
			#endregion

			var shop = new Shop(Lang["Shipyard_Caption"], shopItems, ShopType.Shipyard, Lang["Shipyard_Header"], Lang["Shipyard_Footer"], true);
			ShopLogic.DoShopping(xMenu, yMenu, shop, Shipyard_Buy, null);
		}


		private void		FinesPayment()
		{
			ClearArea();
			var exitFlag = false;
			var currentPosition = 0;

			while (!exitFlag)
			{
				#region If Player CANNOT pay fines

				if (Player.Credits <= 0  &&  CurrentLegalRecord.FineAmount > 0)
				{
					EventLog.Print("FinesPayment_NoMoney");
					currentPosition = 1;
				}
				else if (CurrentLegalRecord.Status == LegalStatus.Terrorist)
				{
					EventLog.Print("FinesPayment_CannotRepay", Enums.Get_Name(CurrentLegalRecord.Status));
					currentPosition = 1;
				}

				#endregion

				#region Get Menu Result
				
				var menuResult =  ZMenu.GetMenuResult(xMenu, yMenu,
					new ZMenu.MenuItem(Lang["FinesPayment_Caption"])
						{
							Options = DefaultMenuOptions,
							ChildMenuItems = new ZMenu.MenuItemList(currentPosition)
							{
								new ZMenu.MenuItem { Caption = FinesPayment_PayAllFines, IsActive = Player.Credits > 0  &&  CurrentLegalRecord.FineAmount > 0 },
								new ZMenu.MenuItem { Caption = Common_Quit	},
							}
						},
					ZFrontier.HotKeys);

				#endregion

				#region Actions

				if (menuResult.Text == FinesPayment_PayAllFines)
				{
					var amountToPay = Math.Min(Player.Credits, CurrentLegalRecord.FineAmount);
					Player.Statistics.FinesPaid += amountToPay;
					Player.Credits -= amountToPay;
					CurrentLegalRecord.FineAmount -= amountToPay;
					EventLog.Print("FinesPayment_Paid", amountToPay.ToString(), Enums.Get_Name(CurrentLegalRecord.Status));
					currentPosition = 1;
					PlayerStats.Draw_PlayerStats();
					GalaxyMap.Draw_CurrentSystemInfo();
				}
				else
				{
					exitFlag = true;
				}

				#endregion				
			}
		}


		private void		MilitaryHQ()
		{
			var shop = new Shop(Lang["MilitaryHQ_Caption"], GetItemsForBBC(Enums.All_MilitaryAdvertTypes, true), ShopType.BBC, Lang["MilitaryHQ_Header"], Lang["MilitaryHQ_Footer"], hasQuit:true,
				descriptionItems: new [] { Lang["MilitaryHQ_About"] + "\t" + "MilitaryHQ_" + CurrentSystem.Allegiance + "_About"});
			ShopLogic.DoShopping(xMenu, yMenu, shop, BBC_Buy, null);
		}


		private IEnumerable<ShopItem> GetItemsForBBC(List<AdvertType> listOfAdvertTypes, bool isMilitary)
		{
			var shopItems = new List<ShopItem>();
			RNG.SwitchMode(true, ZFrontier.Galaxy.RandomSeed);
			for (var i = 0; i < GameConfig.AdvertCount-1 + RNG.GetDiceDiv2Zero(); i++)
			{
				var advert = Advert.Create(listOfAdvertTypes.Get_Random(), CurrentSystem, Player, isMilitary);
				if (isMilitary  &&  advert.Mission.MissionTypeData.MilitaryRatingNeeded > Player.MilitaryRanks[CurrentSystem.Allegiance].Rating)
					continue;

				shopItems.Add(new ShopItem(
					Lang[advert.Caption],
					advert.Price,
					Player.Missions.All(a => advert.Mission == null  ||  a.Id != advert.Mission.Id),
					advert));
			}
			RNG.SwitchMode(false);
			return shopItems;
		}

		#endregion
		

		#region Buy/Sell Methods and Classes for Shops

		private bool		GoodsStore_Buy(ShopItem item)
		{
			if (Player.CurrentCargo.CurrentLoad < Player.MaxCargoLoad)
			{
				Player.Credits -= item.Price;
				Player.CurrentCargo[(Merchandise)item.ItemObject]++;
				EventLog.Print("Merchandise_Bought", item.Name);
			}
			else
			{
				EventLog.Print("Merchandise_CannotBuy");
			}
			return true;
		}
		private bool		GoodsStore_Sell(ShopItem item)
		{
			var enumValue = (Merchandise)item.ItemObject;
			if (Player.CurrentCargo[enumValue] > 0)
			{
				Player.Credits += item.Price;
				Player.CurrentCargo[enumValue]--;
				EventLog.Print("Merchandise_Sold", item.Name);
			}
			else
			{
				EventLog.Print("Merchandise_CannotSell", item.Name);
			}
			return true;
		}

		private bool		EquipmentShop_Buy(ShopItem item)
		{
			var oldCredits = Player.Credits;
			switch ((Equipment)item.ItemObject)
			{
				case Equipment.Missile	:	if (Player.CurrentMissiles<Player.MaxMissiles)	{	Player.CurrentMissiles++;					Player.Credits -= item.Price;	}	else EventLog.Print("Equipment_CannotBuyMore",	Lang["Equipment_Missile_Plural"]);	break;
				case Equipment.LaserUnit:	if (Player.Attack < GameConfig.AttackRange.Max)	{	Player.Attack++;							Player.Credits -= item.Price;	}	else EventLog.Print("Equipment_CannotBuyMore",	Lang["Equipment_Laser_Plural"]);	break;
				case Equipment.Shield	:	if (Player.Defense<GameConfig.DefenseRange.Max)	{	Player.Defense++;							Player.Credits -= item.Price;	}	else EventLog.Print("Equipment_CannotBuyMore",	Lang["Equipment_Shield_Plural"]);	break;
				case Equipment.ECM		:	if (Player.ECM			== EquipmentState.No)	{	Player.ECM			= EquipmentState.Yes;	Player.Credits -= item.Price;	}	else EventLog.Print("Equipment_AlreadyHave",	Lang["Equipment_ECM"]);				break;
				case Equipment.Scanner	:	if (Player.Scanner		== EquipmentState.No)	{	Player.Scanner		= EquipmentState.Yes;	Player.Credits -= item.Price;	}	else EventLog.Print("Equipment_AlreadyHave",	Lang["Equipment_Scanner"]);			break;
				case Equipment.MiningLaser:	if (Player.MiningLaser	== EquipmentState.No)	{	Player.MiningLaser	= EquipmentState.Yes;	Player.Credits -= item.Price;	}	else EventLog.Print("Equipment_AlreadyHave",	Lang["Equipment_MiningLaser"]);		break;
				case Equipment.EscapeBoat:	if (Player.EscapeBoat	== EquipmentState.No)	{	Player.EscapeBoat	= EquipmentState.Yes;	Player.Credits -= item.Price;	}	else EventLog.Print("Equipment_AlreadyHave",	Lang["Equipment_EscapeBoat"]);		break;
			}
			if (oldCredits != Player.Credits)
			{
				EventLog.Print("Equipment_Bought", item.Name);
			}
			return true;
		}
		private bool		EquipmentShop_Sell(ShopItem item)
		{
			var oldCredits = Player.Credits;

			switch ((Equipment)item.ItemObject)
			{
				case Equipment.Missile	:	if (Player.CurrentMissiles > 0)					{	Player.CurrentMissiles--;					Player.Credits += item.Price;	}	else EventLog.Print("Equipment_DontHave",			Lang["Equipment_Missile_Plural"]);	break;
				case Equipment.LaserUnit:	if (Player.Attack > GameConfig.AttackRange.Min)	{	Player.Attack--;							Player.Credits += item.Price;	}	else EventLog.Print("Equipment_CannotSellAnymore",	Lang["Equipment_Laser_Plural"]);	break;
				case Equipment.Shield	:	if (Player.Defense>GameConfig.DefenseRange.Min)	{	Player.Defense--;							Player.Credits += item.Price;	}	else EventLog.Print("Equipment_CannotSellAnymore",	Lang["Equipment_Shield_Plural"]);	break;
				case Equipment.ECM		:	if (Player.ECM			== EquipmentState.Yes)	{	Player.ECM			= EquipmentState.No;	Player.Credits += item.Price;	}	else EventLog.Print("Equipment_DontHave",			Lang["Equipment_ECM"]);				break;
				case Equipment.Scanner	:	if (Player.Scanner		== EquipmentState.Yes)	{	Player.Scanner		= EquipmentState.No;	Player.Credits += item.Price;	}	else EventLog.Print("Equipment_DontHave",			Lang["Equipment_Scanner"]);			break;
				case Equipment.MiningLaser:	if (Player.MiningLaser	== EquipmentState.Yes)	{	Player.MiningLaser	= EquipmentState.No;	Player.Credits += item.Price;	}	else EventLog.Print("Equipment_DontHave",			Lang["Equipment_MiningLaser"]);		break;
				case Equipment.EscapeBoat:	if (Player.EscapeBoat	== EquipmentState.Yes)	{	Player.EscapeBoat	= EquipmentState.No;	Player.Credits += item.Price;	}	else EventLog.Print("Equipment_DontHave",			Lang["Equipment_EscapeBoat"]);		break;
			}
			if (oldCredits != Player.Credits)
			{
				EventLog.Print("Equipment_Sold", item.Name);
			}
			return true;
		}

		private bool		Shipyard_Buy(ShopItem item)
		{
			var newShip = (ShipModel) item.ItemObject;

			if (Player.Credits < item.Price)
			{
				EventLog.Print("Planet_NoMoneyToBuy");
				HighlightArea();
				return false;
			}

			EventLog.HighlightArea();
			var result = false;
			var toBuy = EventLog.Get_YesNo("Shipyard_Confirmation", Player.Ship.ModelName, newShip.ModelName, true);
			if (toBuy)
			{
				var oldShipName = Player.Ship.ModelName;
				Player.Ship = newShip;

				for (var i = 0; i < Enums.All_Merchandise.Count; i++)
				{
					var merch = (Merchandise) i;
					if (IsLegal(merch))
					{
						Player.Credits += (GoodsPrices[merch] + CurrentSystem.PriceChanges[i]) * Player.CurrentCargo[merch];
						Player.CurrentCargo[merch] = 0;
					}
				}

				Player.CurrentCargo.Clear();
				Player.CurrentHP = Player.MaxHP;
				Player.CurrentMissiles = Math.Min(Player.CurrentMissiles, Player.MaxMissiles);
				Player.Credits -= item.Price;
				PlayerStats.Draw_PlayerStats();

				EventLog.Print("Shipyard_Exchanged", oldShipName, newShip.ModelName);
				result = true;
			}
			HighlightArea();
			return result;
		}

		private bool		BBC_Buy(ShopItem item)
		{
			EventLog.ShadowOldMessages();
			var advert = (Advert) item.ItemObject;
			var clientName = advert.OwnerName;

			#region Buy & Illegal Buy

			if (advert.Type == AdvertType.BuyIllegal)
			{
				var merch = advert.Merchandise;
				if (Player.CurrentCargo[merch] == 0)
				{
					EventLog.Print("BBC_NothingToOffer", clientName);
					HighlightArea();
					return false;
				}

				EventLog.HighlightArea();
				var toAgree = EventLog.Get_YesNo(advert.CallText, clientName, Enums.Get_Name(advert.Merchandise), advert.Price.ToString(), false, true);
				if (toAgree)
				{
					if (advert.IsTrap)
					{
						EventLog.Print("BBC_PoliceTrap1", false);
						EventLog.Print("BBC_PoliceTrap2", GameConfig.Fine_IllegalGoods);
						Player.CurrentCargo[merch] = 0;
						CurrentLegalRecord.FineAmount += GameConfig.Fine_IllegalGoods;
						Player.Statistics.IllegalGoodsFound = true;
					}
					else
					{
						var totalAmount = Player.CurrentCargo[merch] * advert.Price;
						Player.CurrentCargo[merch] = 0;
						Player.Credits += totalAmount;
						PlayerStats.Draw_PlayerStats();
						EventLog.Print("BBC_Sold", merch.ToString(), totalAmount.ToString());
					}
					return true;
				}
			}

			#endregion

			#region Missions

			else
			{
				var mission = advert.Mission;
				var system	= mission.TargetStarSystem;
				var npc		= mission.AssassinationTarget;

				#region If player cannot take this mission

				if (Player.CombatRating < mission.MissionTypeData.CombatRatingNeeded)
				{
					EventLog.Print("BBC_NotExperiencedEnough", clientName);
					HighlightArea();
					return false;
				}
				if (Player.ReputationRating < mission.MissionTypeData.ReputationNeeded)
				{
					EventLog.Print("BBC_BadReputation", clientName);
					HighlightArea();
					return false;
				}

				#endregion

				var advertText = Lang[advert.CallText];
				switch (mission.Type)
				{
					case MissionType.PackageDelivery:
					case MissionType.Military_Delivery:	advertText = string.Format(advertText, clientName, system.Name, mission.DaysBeforeDate, mission.RewardAmount);	break;
					case MissionType.GoodsDelivery	:	advertText = string.Format(advertText, clientName, system.Name, mission.DaysBeforeDate, mission.RewardAmount, mission.GoodsToDeliver_Amount, Enums.Get_Name(mission.GoodsToDeliver_Type));	break;
					case MissionType.Passenger		:	advertText = string.Format(advertText, clientName, system.Name, mission.DaysBeforeDate, mission.RewardAmount);	break;
					case MissionType.Assassination	:	advertText = string.Format(advertText, clientName, system.Name, mission.DaysBeforeDate, mission.RewardAmount, Enums.Get_Name(npc.NPC_Type), npc.Name);	break;
					case MissionType.Military_Assassination:advertText = string.Format(advertText, clientName, system.Name, mission.DaysBeforeDate, mission.RewardAmount, npc.Name);	break;
				}

				EventLog.HighlightArea();
				EventLog.PrintPlain(advertText, false);
				var toAgree = EventLog.Get_YesNo("Common_DoYouAgree", true, true);
				if (toAgree)
				{
					Player.Missions.Add(mission);
					EventLog.Print(mission.Type >= MissionType.Military_Delivery ? "Military_MissionConfirmed" : "BBC_AdvertMissionConfirmed", clientName);
					item.IsActive = false;
					return true;
				}
			}

			#endregion

			HighlightArea();
			return true;
		}

		#endregion


		#region Private and Overrided Methods

		private static bool		IsLegal(Merchandise merch)
		{
			return !CurrentSystem.IllegalGoods.Contains(merch);
		}

		public override void	HighlightArea()
		{
			CommonMethods.HighlightArea(AreaType);
			EventLog.WriteLogToFile();
		}

		#endregion
	}
}

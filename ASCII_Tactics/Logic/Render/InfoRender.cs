namespace ASCII_Tactics.Logic.Render
{
	using Config;
	using Models;
	using Models.Items;
	using ZConsole;
	using ZLinq;


	public static class InfoRender
	{
		public static void		DrawUnitInfo(Unit unit)
		{
			ZIOX.OutputType = ZIOX.OutputTypeEnum.Buffer;
			ZIOX.BufferName = UIConfig.Buffer_Info;
			ZBuffer.ClearBuffer(ZIOX.BufferName);
			var area = UIConfig.UnitStatsArea;
			var stats = unit.Stats;
			
			ZIOX.Draw_Stat(area, 0, "Team",			unit.Team.Name);
			ZIOX.Draw_Stat(area, 1, "Name",			unit.Name);
			ZIOX.Draw_Stat(area, 3, "Health", 		ZIOX.Draw_State, stats.CurrentHP, stats.MaxHP, true);
			ZIOX.Draw_Stat(area, 4, "Accuracy",		stats.CurrentAccuracy);
			ZIOX.Draw_Stat(area, 5, "Strength",		stats.CurrentStrength);

			ZIOX.Draw_Stat(area, 7, "Direction",	unit.View.DirectionName);
			ZIOX.Draw_Stat(area, 8, "Crouch",		unit.Position.IsSitting.ToString());

			ZIOX.Draw_Stat(area, 10, "TU", 		ZIOX.Draw_State, stats.CurrentTU, stats.MaxTU, true);

			var activeItem = unit.ActiveItem != null 
				? string.Format("{0} ({1})", unit.ActiveItem.Type.Name, unit.ActiveItem.Value)
				: "nothing";
			ZIOX.Draw_Stat(area, 12, "In hands", activeItem);

			DrawInventory(unit.Inventory);
			
			ZBuffer.WriteBuffer(UIConfig.Buffer_Info, UIConfig.UnitInfoRect.Left, UIConfig.UnitInfoRect.Top);
			ZIOX.OutputType = ZIOX.OutputTypeEnum.Direct;

			DrawUnitPositionInfo(unit);
		}

		public static void		DrawUnitPositionInfo(Unit unit)
		{
			var position = unit.Position;
			var rect = UIConfig.PositionInfoRect;
			ZOutput.PrintBB(rect.Left + 1, rect.Top+1, string.Format("Level:<{0}>  Position:<{1}>,<{2}>", position.LevelId, position.X, position.Y), Color.Magenta);
		}		


		public static void		DrawUnitInfoAsTarget(Unit unit)
		{
			ZIOX.OutputType = ZIOX.OutputTypeEnum.Buffer;
			ZIOX.BufferName = UIConfig.Buffer_TargetInfo;
			var area = UIConfig.UnitStatsArea;
			var stats = unit.Stats;
			
			ZIOX.Draw_Stat(area, 0, "Name",			unit.Name);
			ZIOX.Draw_Stat(area, 1, "Health", 		ZIOX.Draw_State, stats.CurrentHP, stats.MaxHP, true);
			ZIOX.Draw_Stat(area, 2, "TU", 			ZIOX.Draw_State, stats.CurrentTU, stats.MaxTU, true);
			ZIOX.Draw_Stat(area, 3, "Crouch",		unit.Position.IsSitting.ToString());
			ZIOX.Draw_Stat(area, 5, "Direction",	unit.View.DirectionName);
			
			ZIOX.Draw_Stat(area, 7, "In hands",	unit.ActiveItem.Type.Name);
			
			ZBuffer.WriteBuffer(UIConfig.Buffer_TargetInfo, UIConfig.TargetInfoRect.Left, UIConfig.TargetInfoRect.Top);
			ZIOX.OutputType = ZIOX.OutputTypeEnum.Direct;
		}		


		public static void		DrawInventory(Inventory inventory)
		{
			ZIOX.OutputType = ZIOX.OutputTypeEnum.Buffer;
			ZIOX.BufferName = UIConfig.Buffer_Inventory;
			ZBuffer.ClearBuffer(ZIOX.BufferName);

			ZIOX.Draw_Stat(UIConfig.InventoryStatsArea, 0, "Inventory", ZIOX.Draw_Mass, inventory.TotalWeight / 10);

			var backpack = inventory.Where(w => w != inventory.ActiveItem).ToArray();
			for (var i = 0; i < backpack.Length; i++)
			{
				ZIOX.Draw_Item(UIConfig.InventoryStatsArea, 1+i, backpack[i].Type.Name, false);
			}

			ZBuffer.WriteBuffer(UIConfig.Buffer_Inventory, UIConfig.InventoryRect.Left, UIConfig.InventoryRect.Top);
			ZIOX.OutputType = ZIOX.OutputTypeEnum.Direct;
		}
	}
}
namespace ASCII_Tactics.Logic
{
	using System;
	using Config;
	using Models;
	using Render;
	using ZConsole;
	using ZLinq;


	public static class TargetMark
	{
		public static Coord		DoTargetAction(Unit unit)
		{
			var exitFlag = false;

			var target = new Coord(unit.Position.X, unit.Position.Y);
			DrawTargetMark(unit, target, true);

			while (!exitFlag)
			{
				var key = ZInput.ReadKey();
				var oldTarget = new Coord(target.X, target.Y);

				switch (key)
				{
					case ConsoleKey.LeftArrow	:	MoveTarget(target, -1,  0);		break;
					case ConsoleKey.RightArrow	:	MoveTarget(target, +1,  0);		break;
					case ConsoleKey.UpArrow		:	MoveTarget(target,  0, -1);		break;
					case ConsoleKey.DownArrow	:	MoveTarget(target,  0, +1);		break;

					case ConsoleKey.Enter		:	return target;		break;
					case ConsoleKey.Escape		:	exitFlag = true;	break;
				}

				if (!oldTarget.Equals(target))
				{
					DrawTargetMark(unit, oldTarget, false);
					DrawTargetMark(unit, target, true);
				}
			}

			return null;
		}


		private static void	DrawTargetMark(Unit currentUnit, Coord target, bool toShow)
		{
			if (toShow)
			{
				ZBuffer.ReadBuffer("targetBuffer", target.X, target.Y, 3, 3);

				var buffer = ZBuffer.BackupBuffer("defaultBuffer");
				ZIOX.OutputType = ZIOX.OutputTypeEnum.Buffer;
				ZIOX.BufferName = "defaultBuffer";

				var targetColor = IsTargetOnSoldier(currentUnit, target) ? Color.Yellow : Color.Cyan;
				
				ZIOX.Print(target.X-1, target.Y-1, (char)Tools.Get_Ascii_Byte('┌'), targetColor);
				ZIOX.Print(target.X+1, target.Y-1, (char)Tools.Get_Ascii_Byte('┐'), targetColor);
				ZIOX.Print(target.X-1, target.Y+1, (char)Tools.Get_Ascii_Byte('└'), targetColor);
				ZIOX.Print(target.X+1, target.Y+1, (char)Tools.Get_Ascii_Byte('┘'), targetColor);

				MapRender.DrawVisibleUnits(currentUnit);
				ZIOX.OutputType = ZIOX.OutputTypeEnum.Direct;
				
				ZBuffer.WriteBuffer("defaultBuffer", UIConfig.GameAreaRect.Left, UIConfig.GameAreaRect.Top);
				ZBuffer.SaveBuffer("defaultBuffer", buffer);
			}
			else
			{
				ZBuffer.WriteBuffer("targetBuffer", target.X, target.Y);
			}
		}

		private static void	MoveTarget(Coord target, int dx, int dy)
		{
			if (target.X + dx >= UIConfig.GameAreaRect.Left  &&  target.X + dx <= UIConfig.GameAreaRect.Right  &&
				target.Y + dy >= UIConfig.GameAreaRect.Top   &&  target.Y + dy <= UIConfig.GameAreaRect.Bottom)
			{
				target.X += dx;
				target.Y += dy;
			}
		}

		public static bool	IsTargetOnSoldier(Unit currentUnit, Coord target)
		{
			foreach (var team in MainGame.Teams)
				foreach (var unit in team.Units.Where(a => a.Name != currentUnit.Name))
					if (target.Equals(unit.Position))
						return true;
			return false;
		}		
	}
}
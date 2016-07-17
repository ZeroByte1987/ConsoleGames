namespace ASCII_Tactics.Logic
{
	using System;
	using Models.CommonEnums;
	using Models.Map;
	using Models.UnitData;
	using ZConsole;


	public sealed class ViewLogic
	{
		#region Private Variables

		private static readonly Coord[] _viewDirections = new [] { 
			new Coord(-1,-1), new Coord(+0,-1), new Coord(+1,-1), new Coord(+1,+0),
			new Coord(+1,+1), new Coord(+0,+1), new Coord(-1,+1), new Coord(-1,+0)};

		private static readonly string[] _viewDirectionNames = new [] { 
			"\u001b\u0018", "\u0018", "\u0018\u001A", "\u001A", "\u0019\u001A", "\u0019", "\u001b\u0019", "\u001b" };

		private static readonly int[]	_viewAngles = new [] { 225, 270, 315, 0, 45, 90, 135, 180 };
		private static Range[]			_viewRanges;

		private int	_direction;

		private ViewLogic(int viewWidth, int initialViewDirection = 0, int viewDistance = 0)
		{
			ViewDistance = viewDistance;
			Direction = initialViewDirection;

			_viewRanges = new Range[8];
			for (var i = 0; i < 8; i++)
			{
				_viewRanges[i] = new Range(_viewAngles[i] - viewWidth/2, _viewAngles[i] + viewWidth/2);
			}
		}

		#endregion

		public int		Direction
		{
			get { return _direction; }
			set { _direction = Tools.SetIntoRange(value, 0, 7); }
		}

		public string	DirectionName
		{
			get { return _viewDirectionNames[_direction]; }
		}

		public int		ViewDistance	{ get; set; }
		public int		DX				{ get { return _viewDirections[Direction].X;  }}
		public int		DY				{ get { return _viewDirections[Direction].Y;  }}


		public static ViewLogic	Initialize(int viewAngle, int initialViewDirection = 0, int viewDistance = 0)
		{
			return new ViewLogic(viewAngle, initialViewDirection, viewDistance);
		}


		public Visibility		IsPointVisible(Level level, Position unit, Coord tileCoord)
		{
			var isPointInRange = IsPointInRange(level, unit, tileCoord);
			return isPointInRange 
				? IsRayPossible(level, unit, tileCoord) 
				: Visibility.None;
		}

		public Visibility		IsUnitVisible(Level level, Position activeUnit, Position targetUnit)
		{
			var isPointInRange = IsPointInRange(level, activeUnit, targetUnit);
			return isPointInRange 
				? IsRayPossibleForUnit(level, activeUnit, targetUnit) 
				: Visibility.None;
		}


		public bool				IsPointInRange(Level level, Position unit, Coord point)
		{
			if (point.X == unit.X  &&  point.Y == unit.Y)
				return false;

			var dx = point.X - unit.X;
			var dy = point.Y - unit.Y;

			if (ViewDistance > 0  &&  (dx*dx + dy*dy) > (ViewDistance*ViewDistance))
				return false;
			
			var currentRange = _viewRanges[Direction];
			var angle = Math.Atan2(dy, dx) * 180.0 / Math.PI;
			angle = (angle < currentRange.Min) ? 360 + angle : angle;
			return angle >= currentRange.Min  &&  angle <= currentRange.Max;
		}


		public Visibility		IsRayPossible(Level level, Position unit, Coord point)
		{
			var x = point.X;
			var y = point.Y;
			var playerX = unit.X;
			var playerY = unit.Y;

			var unitHeight = unit.IsSitting ? ObjectHeight.Half : ObjectHeight.Full;
			var targetTileHeight = level.Map[y, x].Type.Height;
			var result = Visibility.Full;
			
			var steep = Math.Abs(y - playerY) > Math.Abs(x - playerX);
            if (steep)
            {
	            Utils.Swap(ref playerX, ref playerY); 
				Utils.Swap(ref x, ref y);
            }
			
			var dX = Math.Abs(x - playerX);
			var dY = Math.Abs(y - playerY);
			var fullDistance = dX*dX + dY*dY;
			var xDistancePassed = 0;
			var yDistancePassed = 0;
			var err = (dX/2);
			var xStep = playerX > x ? -1 : 1;
			var yStep = playerY > y ? -1 : 1;
			var cy = playerY;

			for (var cx = playerX; cx != x; cx += xStep)
            {
	            var tile = !steep ? level.Map[cy, cx] : level.Map[cx, cy];
	            var tileHeight = tile.Type.Height;

				if (tileHeight == ObjectHeight.Half)
				{
					var halfHeightDistance = (xDistancePassed*xDistancePassed + yDistancePassed*yDistancePassed);
					if (targetTileHeight == ObjectHeight.None  &&  Math.Sqrt(fullDistance) / Math.Sqrt(halfHeightDistance) < 1.7)
					{
						result = Visibility.Shadow;
					}
				}

				if (tileHeight >= unitHeight  &&  cx != playerX)
					return Visibility.None;

	            xDistancePassed++;
				err = err - dY;
                if (err < 0)
                {
	                cy += yStep;
	                yDistancePassed++;
					err += dX;
                }
            }

			return result;
		}

		public Visibility		IsRayPossibleForUnit(Level level, Position activeUnit, Position targetUnit)
		{
			var x = targetUnit.X;
			var y = targetUnit.Y;
			var playerX = activeUnit.X;
			var playerY = activeUnit.Y;

			var unitHeight = activeUnit.IsSitting ? ObjectHeight.Half : ObjectHeight.Full;
			var targetHeight = targetUnit.IsSitting ? ObjectHeight.Half : ObjectHeight.Full;
			var result = Visibility.Full;

			var steep = Math.Abs(y - playerY) > Math.Abs(x - playerX);
            if (steep)
            {
	            Utils.Swap(ref playerX, ref playerY); 
				Utils.Swap(ref x, ref y);
            }
			
			var dX = Math.Abs(x - playerX);
			var dY = Math.Abs(y - playerY);
			var fullDistance = dX*dX + dY*dY;
			var xDistancePassed = 0;
			var yDistancePassed = 0;
			var err = (dX/2);
			var xStep = playerX > x ? -1 : 1;
			var yStep = playerY > y ? -1 : 1;
			var cy = playerY;

			for (var cx = playerX; cx != x; cx += xStep)
            {
	            var tile = !steep ? level.Map[cy, cx] : level.Map[cx, cy];
	            var tileHeight = tile.Type.Height;

				if (tileHeight == ObjectHeight.Half)
				{
					var halfHeightDistance = (xDistancePassed*xDistancePassed + yDistancePassed*yDistancePassed);
					if (targetHeight == ObjectHeight.Half  &&  Math.Sqrt(fullDistance) - Math.Sqrt(halfHeightDistance) < 1.5)
					{
						result = Visibility.Shadow;
					}
				}

				if (tileHeight >= unitHeight  &&  cx != playerX)
					return Visibility.None;

	            xDistancePassed++;
				err = err - dY;
                if (err < 0)
                {
	                cy += yStep;
	                yDistancePassed++;
					err += dX;
                }
            }

			return result;
		}


		public void				TurnLeft(int times = 1)
		{
			for (var i = 0; i < times; i++)
			{
				Direction = Direction > 0 ? Direction - 1 : 7;
			}
		}
		
		public void				TurnRight(int times = 1)
		{
			for (var i = 0; i < times; i++)
			{
				Direction = Direction < 7 ? Direction + 1 : 0;
			}
		}
	}
}
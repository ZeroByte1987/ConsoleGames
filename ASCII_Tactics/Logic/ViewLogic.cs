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

		public int		ViewDistance	{ get; set; }

		public int		Direction
		{
			get { return _direction; }
			set { _direction = Tools.SetIntoRange(value, 0, 7); }
		}

		public string	DirectionName	{ get { return _viewDirectionNames[_direction]; }}
		public int		DX				{ get { return _viewDirections[Direction].X;  }}
		public int		DY				{ get { return _viewDirections[Direction].Y;  }}
		public bool		IsDiagonal		{ get { return _direction % 2 == 0; }}


		public static ViewLogic	Initialize(int viewAngle, int viewDistance, int initialViewDirection = 0)
		{
			return new ViewLogic(viewAngle, initialViewDirection, viewDistance);
		}


		public Visibility		IsPointVisible(Level level, Position unit, Coord tileCoord)
		{
			var isPointInRange = IsPointInRange(level, unit, tileCoord);
			return isPointInRange 
				? IsRayPossibleForTile(level, unit, tileCoord) 
				: Visibility.None;
		}

		public Visibility		IsUnitVisible(Level level, Position activeUnit, Position targetUnit)
		{
			if (activeUnit.LevelId != targetUnit.LevelId)
				return Visibility.None;
			
			var isPointInRange = IsPointInRange(level, activeUnit, targetUnit);
			return isPointInRange 
				? IsRayPossibleForUnit(level, activeUnit, targetUnit) 
				: Visibility.None;
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


		public int				GetDirectionForOffset(int dx, int dy)
		{
			var coord = new Coord(dx, dy);
			for (var i = 0; i < _viewDirections.Length; i++)
				if (coord.Equals(_viewDirections[i]))
					return i;
			return -1;
		}

		public Coord			GetAbsoluteMoveDirection(int dx, int dy)
		{
			var moveRelativeDirection = GetDirectionForOffset(dx, dy);
			return _viewDirections[(moveRelativeDirection + Direction - 1) % 8];
		}


		private bool			IsPointInRange(Level level, Position unit, Coord point)
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

		private Visibility		IsRayPossibleForTile(Level level, Position unit, Coord point)
		{
			var targetHeight = level.Map[point.Y, point.X].Type.Height;
			return IsRayPossible(level, unit, point.X, point.Y, targetHeight, ObjectHeight.None);
		}

		private Visibility		IsRayPossibleForUnit(Level level, Position activeUnit, Position targetUnit)
		{
			var targetHeight = targetUnit.IsSitting ? ObjectHeight.Half : ObjectHeight.Full;
			return IsRayPossible(level, activeUnit, targetUnit.X, targetUnit.Y, targetHeight, ObjectHeight.Half);
		}

		private Visibility		IsRayPossible(
			Level level, Position unit, int x, int y, ObjectHeight targetHeight, ObjectHeight heightLimit)
		{
			var playerX = unit.X;
			var playerY = unit.Y;
			var unitHeight = unit.IsSitting ? ObjectHeight.Half : ObjectHeight.Full;
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
			var xPassed = 0;
			var yPassed = 0;
			var err = (dX/2);
			var xStep = playerX > x ? -1 : 1;
			var yStep = playerY > y ? -1 : 1;
			var cy = playerY;

			for (var cx = playerX; cx != x; cx += xStep)
            {
	            var tile = !steep ? level.Map[cy, cx] : level.Map[cx, cy];
	            var tileHeight = tile.Type.Height;

				if (tileHeight == ObjectHeight.Half  &&  IsTileShadowedAfterObstacle(targetHeight, heightLimit, fullDistance, xPassed, yPassed))
				{
					result = Visibility.Shadow;
				}

				if (tileHeight >= unitHeight  &&  cx != playerX)
					return Visibility.None;

	            xPassed++;
				err = err - dY;
                if (err < 0)
                {
	                cy += yStep;
	                yPassed++;
					err += dX;
                }
            }

			return result;
		}

		private bool			IsTileShadowedAfterObstacle(
			ObjectHeight targetHeight, ObjectHeight heightLimit, int fullDistance, int xPassed, int yPassed)
		{
			var halfHeightDistance = (xPassed*xPassed + yPassed*yPassed);
			var factor = Math.Sqrt(fullDistance) / Math.Sqrt(halfHeightDistance);
			return targetHeight == heightLimit  &&  (factor < 1.7
			                                         ||  (fullDistance == 4  &&  halfHeightDistance == 1)
			                                         ||  (fullDistance == 8  &&  halfHeightDistance == 2));
		}
	}
}
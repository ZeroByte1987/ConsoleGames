namespace ASCII_Tactics.Logic
{
	using System;
	using Models.CommonEnums;
	using Models.Map;
	using ZConsole;


	public sealed class ViewLogic
	{
		#region Private Variables

		private static readonly Coord[] _viewDirections = new [] { 
			new Coord(-1,-1), new Coord(+0,-1), new Coord(+1,-1), new Coord(+1,+0),
			new Coord(+1,+1), new Coord(+0,+1), new Coord(-1,+1), new Coord(-1,+0)};

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

		public static bool DrawRay = false;

		public int	Direction
		{
			get { return _direction; }
			set { _direction = Tools.SetIntoRange(value, 0, 7); }
		}

		public int	ViewDistance	{ get; set; }
		public int	DX				{ get { return _viewDirections[Direction].X;  }}
		public int	DY				{ get { return _viewDirections[Direction].Y;  }}


		public static ViewLogic	Initialize(int viewAngle, int initialViewDirection = 0, int viewDistance = 0)
		{
			return new ViewLogic(viewAngle, initialViewDirection, viewDistance);
		}

		public bool				IsPointVisible(Level level, Coord playerCoord, int x, int y, bool isUnitSitting = false)
		{
			return IsPointVisible(level, playerCoord.X, playerCoord.Y, x, y, isUnitSitting);
		}
		public bool				IsPointVisible(Level level, Coord playerCoord, Coord tileCoord, bool isUnitSitting = false)
		{
			return IsPointVisible(level, playerCoord.X, playerCoord.Y, tileCoord.X, tileCoord.Y, isUnitSitting);
		}
		public bool				IsPointVisible(Level level, int playerX, int playerY, int x, int y, bool isUnitSitting = false)
		{
			if (x == playerX  &&  y == playerY)
				return false;

			var dx = x - playerX;
			var dy = y - playerY;

			if (ViewDistance > 0  &&  (dx*dx + dy*dy) > (ViewDistance*ViewDistance))
				return false;
			
			var currentRange = _viewRanges[Direction];
			var angle = Math.Atan2(dy, dx) * 180.0 / Math.PI;
			angle = (angle < currentRange.Min) ? 360 + angle : angle;
			var isAngleInRange = angle >= currentRange.Min  &&  angle <= currentRange.Max;
			if (!isAngleInRange)
				return false;

			return IsRayPossible(level, playerX, playerY, x, y, isUnitSitting);
		}


		public bool				IsRayPossible(Level level, int playerX, int playerY, int x, int y, bool isUnitSitting = false)
		{
			var unitHeight = isUnitSitting ? ObjectHeight.Half : ObjectHeight.Full;
			var targetTileHeight = level.Map[y, x].Type.Height;	//
			
			var steep = Math.Abs(y - playerY) > Math.Abs(x - playerX);
            if (steep)
            {
	            Utils.Swap(ref playerX, ref playerY); 
				Utils.Swap(ref x, ref y);
            }
			
			var dX = Math.Abs(x - playerX);
			var dY = Math.Abs(y - playerY);
			var fullDistance = dX*dX + dY*dY;	//
			var xDistancePassed = 0;	//
			var yDistancePassed = 0;	//
			var err = (dX/2);
			var xStep = playerX > x ? -1 : 1;
			var yStep = playerY > y ? -1 : 1;
			var cy = playerY;

			for (var cx = playerX; cx != x; cx += xStep)
            {
	            var tile = !steep ? level.Map[cy, cx] : level.Map[cx, cy];
	            var tileHeight = tile.Type.Height;

				//
				if (tileHeight == ObjectHeight.Half)
				{
					var halfHeightDistance = (xDistancePassed*xDistancePassed + yDistancePassed*yDistancePassed);
					if (targetTileHeight == ObjectHeight.None  &&  Math.Sqrt(fullDistance) / Math.Sqrt(halfHeightDistance) < 2)
					{
						return false;
					}
				}

				if (tileHeight >= unitHeight  &&  cx != playerX)
					return false;

	            xDistancePassed++;
				err = err - dY;
                if (err < 0)
                {
	                cy += yStep;
	                yDistancePassed++;
					err += dX;
                }
            }

			return true;
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
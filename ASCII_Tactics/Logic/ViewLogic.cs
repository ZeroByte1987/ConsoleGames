namespace ASCII_Tactics.Logic
{
	using System;
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

		#endregion
		

		#region Public Properties

		public int	Direction
		{
			get { return _direction; }
			set { _direction = Tools.SetIntoRange(value, 0, 7); }
		}

		public int	ViewDistance	{ get; set; }
		public int	DX				{ get { return _viewDirections[Direction].X;  }}
		public int	DY				{ get { return _viewDirections[Direction].Y;  }}

		#endregion

		
		#region Constructors

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


		#region Public Methods

		public static ViewLogic	Initialize(int viewWidth, int initialViewDirection = 0, int viewDistance = 0)
		{
			return new ViewLogic(viewWidth, initialViewDirection, viewDistance);
		}


		public bool				IsPointVisible(int playerX, int playerY, int x, int y)
		{
			var currentRange = _viewRanges[Direction];

			var dx = x - (playerX - DX);
			var dy = y - (playerY - DY);
			if (x == playerX - DX  &&  y == playerY - DY)
				return false;

			if (ViewDistance > 0 && (Math.Pow(dx,2)+Math.Pow(dy,2)) > (ViewDistance*ViewDistance))
				return false;
			
			var angle = Math.Atan2(dy, dx) * 180.0 / Math.PI;
			angle = (angle < currentRange.Min) ? 360 + angle : angle;
			return angle >= currentRange.Min && angle <= currentRange.Max;
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

		#endregion
	}
}
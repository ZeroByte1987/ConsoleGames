namespace ASCII_Tactics.Logic.View
{
	using System;
	using System.Collections.Generic;
	using Config;
	using Models.CommonEnums;
	using Models.Map;
	using Models.Tiles;
	using ZConsole;


	public static class RecursionFOV
    {
        public static Size			MapSize			{ get; set; }
        public static Tile[,]		Map				{ get; private set; }
        public static int			VisualRange		{ get { return GameConfig.DefaultViewDistance; } }
        public static List<Coord>	VisiblePoints	{ get; private set; }  // Cells the player can see

        private static Coord		PlayerPos;
		private static ObjectHeight	PlayerHeight;

        private static readonly List<int> VisibleOctants = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8 };


        //  Octant data
        //
        //    \ 1 | 2 /
        //   8 \  |  / 3
        //   -----+-----
        //   7 /  |  \ 4
        //    / 6 | 5 \
        //
        //  1 = NNW, 2 =NNE, 3=ENE, 4=ESE, 5=SSE, 6=SSW, 7=WSW, 8 = WNW

        /// <summary>
        /// Start here: go through all the octants which surround the player to determine which open cells are visible
        /// </summary>
        public static List<Coord> GetVisibleCells(Level level, Coord playerPosition, ObjectHeight playerHeight)
        {
	        MapSize = level.Size;
	        Map = level.Map;
	        PlayerPos = playerPosition;
	        PlayerHeight = playerHeight;

            VisiblePoints = new List<Coord>{ PlayerPos };
            foreach (var octant in VisibleOctants)
                ScanOctant(1, octant, 1.0, 0.0);

	        return VisiblePoints;

        }

        /// <summary>
        /// Examine the provided octant and calculate the visible cells within it.
        /// </summary>
        /// <param name="pDepth">Depth of the scan</param>
        /// <param name="pOctant">Octant being examined</param>
        /// <param name="pStartSlope">Start slope of the octant</param>
        /// <param name="pEndSlope">End slope of the octance</param>
        private static void		ScanOctant(int pDepth, int pOctant, double pStartSlope, double pEndSlope)
        {
            var visRange2 = VisualRange * VisualRange;
            var x = 0;
            var y = 0;

            switch (pOctant)
            {
                case 1: //nnw
                    y = PlayerPos.Y - pDepth;
                    if (y < 0) return;

                    x = PlayerPos.X - Convert.ToInt32((pStartSlope * Convert.ToDouble(pDepth)));
                    if (x < 0) 
						x = 0;

                    while (GetSlope(x, y, PlayerPos.X, PlayerPos.Y, false) >= pEndSlope)
                    {
                        if (GetVisDistance(x, y, PlayerPos.X, PlayerPos.Y) <= visRange2)
                        {
                            if (Map[y, x].Type.Height >= PlayerHeight) //current cell blocked
                            {
                                if (x-1 >= 0  &&  Map[y, x-1].Type.Height < PlayerHeight)
									//prior cell within range AND open, incremenet the depth, adjust the endslope and recurse
                                    ScanOctant(pDepth + 1, pOctant, pStartSlope, GetSlope(x - 0.5, y + 0.5, PlayerPos.X, PlayerPos.Y, false));
                            }
                            else
                            {
                                if (x-1 >= 0  &&  Map[y, x-1].Type.Height >= PlayerHeight) //prior cell within range AND open, adjust the startslope
                                    pStartSlope = GetSlope(x - 0.5, y - 0.5, PlayerPos.X, PlayerPos.Y, false);
                                VisiblePoints.Add(new Coord(x, y));
                            }
                        }
                        x++;
                    }
                    x--;
                    break;

                case 2: //nne
                    y = PlayerPos.Y - pDepth;
                    if (y < 0) return;

                    x = PlayerPos.X + Convert.ToInt32((pStartSlope * Convert.ToDouble(pDepth)));
                    if (x >= Map.GetLength(0))
						x = Map.GetLength(0) - 1;

                    while (GetSlope(x, y, PlayerPos.X, PlayerPos.Y, false) <= pEndSlope)
                    {
                        if (GetVisDistance(x, y, PlayerPos.X, PlayerPos.Y) <= visRange2)
                        {
                            if (Map[y, x].Type.Height >= PlayerHeight)
                            {
                                if (x+1 < Map.GetLength(0)  &&  Map[y, x+1].Type.Height < PlayerHeight)
                                    ScanOctant(pDepth + 1, pOctant, pStartSlope, GetSlope(x + 0.5, y + 0.5, PlayerPos.X, PlayerPos.Y, false));
                            }
                            else
                            {
                                if (x+1 < Map.GetLength(0)  &&  Map[y, x+1].Type.Height >= PlayerHeight)
                                    pStartSlope = -GetSlope(x + 0.5, y - 0.5, PlayerPos.X, PlayerPos.Y, false);

                                VisiblePoints.Add(new Coord(x, y));
                            }
                        }
                        x--;
                    }
                    x++;
                    break;

                case 3:
                    x = PlayerPos.X + pDepth;
                    if (x >= Map.GetLength(0)) return;

                    y = PlayerPos.Y - Convert.ToInt32((pStartSlope * Convert.ToDouble(pDepth)));
                    if (y < 0) y = 0;

                    while (GetSlope(x, y, PlayerPos.X, PlayerPos.Y, true) <= pEndSlope)
                    {
                        if (GetVisDistance(x, y, PlayerPos.X, PlayerPos.Y) <= visRange2)
                        {
                            if (Map[y, x].Type.Height >= PlayerHeight)
                            {
                                if (y-1 >= 0  &&  Map[y-1, x].Type.Height < PlayerHeight)
                                    ScanOctant(pDepth + 1, pOctant, pStartSlope, GetSlope(x - 0.5, y - 0.5, PlayerPos.X, PlayerPos.Y, true));
                            }
                            else
                            {
                                if (y-1 >= 0  &&  Map[y-1, x] .Type.Height >= PlayerHeight)
                                    pStartSlope = -GetSlope(x + 0.5, y - 0.5, PlayerPos.X, PlayerPos.Y, true);

                                VisiblePoints.Add(new Coord(x, y));
                            }
                        }
                        y++;
                    }
                    y--;
                    break;

                case 4:
                    x = PlayerPos.X + pDepth;
                    if (x >= Map.GetLength(0)) return;

                    y = PlayerPos.Y + Convert.ToInt32((pStartSlope * Convert.ToDouble(pDepth)));
                    if (y >= Map.GetLength(1)) y = Map.GetLength(1) - 1;

                    while (GetSlope(x, y, PlayerPos.X, PlayerPos.Y, false) >= pEndSlope)
                    {
                        if (GetVisDistance(x, y, PlayerPos.X, PlayerPos.Y) <= visRange2)
                        {
                            if (Map[y, x].Type.Height >= PlayerHeight)
                            {
                                if (y+1 < Map.GetLength(1)  &&  Map[y+1, x].Type.Height < PlayerHeight)
                                    ScanOctant(pDepth + 1, pOctant, pStartSlope, GetSlope(x - 0.5, y + 0.5, PlayerPos.X, PlayerPos.Y, true));
                            }
                            else
                            {
                                if (y+1 < Map.GetLength(1)  &&  Map[y+1, x].Type.Height >= PlayerHeight)
                                    pStartSlope = GetSlope(x + 0.5, y + 0.5, PlayerPos.X, PlayerPos.Y, true);

                                VisiblePoints.Add(new Coord(x, y));
                            }
                        }
                        y--;
                    }
                    y++;
                    break;

                case 5:
                    y = PlayerPos.Y + pDepth;
                    if (y >= Map.GetLength(1)) return;

                    x = PlayerPos.X + Convert.ToInt32((pStartSlope * Convert.ToDouble(pDepth)));
                    if (x >= Map.GetLength(0)) x = Map.GetLength(0) - 1;

                    while (GetSlope(x, y, PlayerPos.X, PlayerPos.Y, false) >= pEndSlope)
                    {
                        if (GetVisDistance(x, y, PlayerPos.X, PlayerPos.Y) <= visRange2)
                        {
                            if (Map[y, x].Type.Height >= PlayerHeight)
                            {
                                if (x+1 < Map.GetLength(1)  &&  Map[y, x+1].Type.Height < PlayerHeight)
                                    ScanOctant(pDepth + 1, pOctant, pStartSlope, GetSlope(x + 0.5, y - 0.5, PlayerPos.X, PlayerPos.Y, false));
                            }
                            else
                            {
                                if (x+1 < Map.GetLength(1)  &&  Map[y, x+1].Type.Height >= PlayerHeight)
                                    pStartSlope = GetSlope(x + 0.5, y + 0.5, PlayerPos.X, PlayerPos.Y, false);

                                VisiblePoints.Add(new Coord(x, y));
                            }
                        }
                        x--;
                    }
                    x++;
                    break;

                case 6:
                    y = PlayerPos.Y + pDepth;
                    if (y >= Map.GetLength(1)) return;

                    x = PlayerPos.X - Convert.ToInt32((pStartSlope * Convert.ToDouble(pDepth)));
                    if (x < 0) x = 0;

                    while (GetSlope(x, y, PlayerPos.X, PlayerPos.Y, false) <= pEndSlope)
                    {
                        if (GetVisDistance(x, y, PlayerPos.X, PlayerPos.Y) <= visRange2)
                        {
                            if (Map[y, x].Type.Height >= PlayerHeight)
                            {
                                if (x-1 >= 0  &&  Map[y, x-1].Type.Height < PlayerHeight)
                                    ScanOctant(pDepth + 1, pOctant, pStartSlope, GetSlope(x - 0.5, y - 0.5, PlayerPos.X, PlayerPos.Y, false));
                            }
                            else
                            {
                                if (x-1 >= 0  &&  Map[y, x-1].Type.Height >= PlayerHeight)
                                    pStartSlope = -GetSlope(x - 0.5, y + 0.5, PlayerPos.X, PlayerPos.Y, false);
                                VisiblePoints.Add(new Coord(x, y));
                            }
                        }
                        x++;
                    }
                    x--;
                    break;

                case 7:
                    x = PlayerPos.X - pDepth;
                    if (x < 0) return;

                    y = PlayerPos.Y + Convert.ToInt32((pStartSlope * Convert.ToDouble(pDepth)));
                    if (y >= Map.GetLength(1)) y = Map.GetLength(1) - 1;

                    while (GetSlope(x, y, PlayerPos.X, PlayerPos.Y, true) <= pEndSlope)
                    {
                        if (GetVisDistance(x, y, PlayerPos.X, PlayerPos.Y) <= visRange2)
                        {
                            if (Map[y, x].Type.Height >= PlayerHeight)
                            {
                                if (y+1 < Map.GetLength(1)  &&  Map[y+1, x].Type.Height < PlayerHeight)
                                    ScanOctant(pDepth + 1, pOctant, pStartSlope, GetSlope(x + 0.5, y + 0.5, PlayerPos.X, PlayerPos.Y, true));
                            }
                            else
                            {
                                if (y+1 < Map.GetLength(1)  &&  Map[y+1, x].Type.Height >= PlayerHeight)
                                    pStartSlope = -GetSlope(x - 0.5, y + 0.5, PlayerPos.X, PlayerPos.Y, true);

                                VisiblePoints.Add(new Coord(x, y));
                            }
                        }
                        y--;
                    }
                    y++;
                    break;

                case 8: //wnw
                    x = PlayerPos.X - pDepth;
                    if (x < 0) 
						return;

                    y = PlayerPos.Y - Convert.ToInt32((pStartSlope * Convert.ToDouble(pDepth)));
                    if (y < 0) 
						y = 0;

                    while (GetSlope(x, y, PlayerPos.X, PlayerPos.Y, true) >= pEndSlope)
                    {
                        if (GetVisDistance(x, y, PlayerPos.X, PlayerPos.Y) <= visRange2)
                        {
                            if (Map[y, x].Type.Height >= PlayerHeight)
                            {
                                if (y-1 >= 0  &&  Map[y-1, x].Type.Height < PlayerHeight)
                                    ScanOctant(pDepth + 1, pOctant, pStartSlope, GetSlope(x + 0.5, y - 0.5, PlayerPos.X, PlayerPos.Y, true));
                            }
                            else
                            {
                                if (y-1 >= 0  &&  Map[y-1, x].Type.Height >= PlayerHeight)
                                    pStartSlope = GetSlope(x - 0.5, y - 0.5, PlayerPos.X, PlayerPos.Y, true);

                                VisiblePoints.Add(new Coord(x, y));
                            }
                        }
                        y++;
                    }
                    y--;
                    break;
            }


            if (x < 0)
                x = 0;
            else if (x >= Map.GetLength(0))
                x = Map.GetLength(0) - 1;

            if (y < 0)
                y = 0;
            else if (y >= Map.GetLength(1))
                y = Map.GetLength(1) - 1;

            if (pDepth < VisualRange  &&  Map[x, y].Type.Height <= PlayerHeight)
                ScanOctant(pDepth + 1, pOctant, pStartSlope, pEndSlope);
        }

        /// <summary>
        /// Get the gradient of the slope formed by the two points
        /// </summary>
        /// <param name="pX1"></param>
        /// <param name="pY1"></param>
        /// <param name="pX2"></param>
        /// <param name="pY2"></param>
        /// <param name="pInvert">Invert slope</param>
        /// <returns></returns>
        private static double	GetSlope(double pX1, double pY1, double pX2, double pY2, bool pInvert)
        {
			return pInvert
				? (pY1 - pY2) / (pX1 - pX2)
				: (pX1 - pX2) / (pY1 - pY2);
        }

        private static int		GetVisDistance(int pX1, int pY1, int pX2, int pY2)
        {
            return ((pX1 - pX2) * (pX1 - pX2)) + ((pY1 - pY2) * (pY1 - pY2));
        }
    }
}
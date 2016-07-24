namespace ASCII_Tactics.Logic
{
	using Config;


	public static class TimeCost
	{
		public static int	GetTimeCostForMove(int dx, int dy, bool isSitting, ViewLogic view)
		{
			var moveRelativeDirection = view.GetDirectionForOffset(dx, dy);
			var moveTimeCost = ActionCostConfig.BasicMove * ActionCostConfig.DirectionsCostFactor[moveRelativeDirection];
			var isDiagonal = (moveRelativeDirection - view.Direction) % 2 != 0;

			moveTimeCost = isSitting ? moveTimeCost * ActionCostConfig.CrouchFactor : moveTimeCost;
			moveTimeCost = isDiagonal ? moveTimeCost * ActionCostConfig.DiagonalFactor : moveTimeCost;

			return (int)moveTimeCost;
		}
	}
}
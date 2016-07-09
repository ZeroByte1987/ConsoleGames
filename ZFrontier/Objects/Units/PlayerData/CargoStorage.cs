namespace ZFrontier.Objects.Units.PlayerData
{
	using System.Collections.Generic;
	using GameData;
	using Logic;
	using ZConsole;
	using ZLinq;


	public class CargoStorage : Dictionary<Merchandise, int>
	{
		#region Properties and Constructor
		
		public int			CurrentLoad		{  get	{	return Values.Sum();	}}


		public CargoStorage()
		{
			foreach (var merch in Enums.All_Merchandise)
			{
				Add(merch, 0);
			}
		}

		#endregion

		#region Public Methods

		public bool			HasAny(List<Merchandise> list)
		{
			return list.Any(t => this[t] > 0);
		}

		public void			RemoveAllSpecified(List<Merchandise> list)
		{
			foreach (var t in list)
			{
				this[t] = 0;
			}
		}

		public new void		Clear()
		{
			RemoveAllSpecified(Enums.All_Merchandise);
		}

		public new void		Add(Merchandise merchandise, int amount)
		{
			if (ContainsKey(merchandise))	this[merchandise] = this[merchandise] + amount;
			else							this[merchandise] = amount;
		}

		public void			Remove(Merchandise merchandise, int amount)
		{
			if (ContainsKey(merchandise))	this[merchandise] = this[merchandise] - amount;
			if (this[merchandise] < 0)		this[merchandise] = 0;
		}

		public CargoDrop	GetRandonDrop(int damage)
		{
			var existingGoods = this.Where(a => a.Value > 0).ToList();
			var goodsToDrop = new CargoDrop();
			
			foreach (var t in existingGoods)
			{
				var value = Tools.SetIntoRange(RNG.GetDiceDiv2()-damage, 0, this[t.Key]);
				if (value > 0)
				{
					this[t.Key] -= value;
					goodsToDrop.Add(new KeyValuePair<Merchandise, int>(t.Key, value));
				}
			}
			return goodsToDrop;
		}

		public CargoStorage Copy()
		{
			var result = new CargoStorage();
			foreach (var merch in Enums.All_Merchandise)
			{
				result[merch] = this[merch];
			}			
			return result;
		}

		#endregion
	}
}

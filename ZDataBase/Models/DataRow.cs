namespace ZDataBase.Models
{
	using System.Collections.Generic;
	using System.Linq;


	public class DataRow
	{
		public List<string>		Values		{ get; set; }


		public DataRow()
		{
			Values = new List<string>();
		}

		public DataRow(IEnumerable<object> values)
		{
			Values = values.Select(v => v.ToString()).ToList();
		}
	}
}
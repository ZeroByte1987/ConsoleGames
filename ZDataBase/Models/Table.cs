namespace ZDataBase.Models
{
	using System.Collections.Generic;
	using System.Linq;


	public class Table
	{
		public string			Name		{ get; set; }
		public List<Column>		Columns		{ get; set; }
		public List<DataRow>	Rows		{ get; set; }

		public Table(string name, params Column[] columns)
		{
			Name = name;
			Columns = columns.ToList();
			Rows = new List<DataRow>();
		}
	}
}
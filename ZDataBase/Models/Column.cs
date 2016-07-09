namespace ZDataBase.Models
{
	public class Column
	{
		public string		Name	{ get; set; }
		public ColumnType	Type	{ get; set; }

		public Column(string name, ColumnType type)
		{
			Name = name;
			Type = type;
		}
	}


	public enum ColumnType
	{
		Integer,
		Double,
		Boolean,
		String,
		Char,
		DateTime
	}
}
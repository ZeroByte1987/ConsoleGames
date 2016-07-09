namespace ZDataBase
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using Logic;
	using Models;


	public class ZDataBase
    {
	    private readonly string			DataBaseFileName;
		private readonly List<Table>	DataTables;


		public ZDataBase(string dataBaseFileName)
		{
			DataBaseFileName = dataBaseFileName;

			DataTables = File.Exists(DataBaseFileName) 
				? TableReader.ReadTables(DataBaseFileName)
				: new List<Table>();
		}



		public void		AddTable(string tableName, params Column[] columns)
		{
			if (hasTable(tableName))
				throw new ZException("Table with specified name already exists: {0}.", tableName);

			DataTables.Add(new Table(tableName, columns));
		}
		
		public void		RemoveTable(string tableName)
		{
			var tableToRemove = getTable(tableName);
			DataTables.Remove(tableToRemove);
		}


		public void		AddColumn(string tableName, string columnName, ColumnType columnType)
		{
			var tableForInsert = getTable(tableName);
			if (hasColumn(tableForInsert, columnName))
				throw new ZException("Table [{0}] already contains column with specified name: {1}.", tableName, columnName);

			tableForInsert.Columns.Add(new Column(columnName, columnType));
		}

		public void		RemoveColumn(string tableName, string columnName)
		{
			var tableToRemoveColumn = getTable(tableName);
			var columnToRemove = getColumn(tableToRemoveColumn, columnName);
			tableToRemoveColumn.Columns.Remove(columnToRemove);
		}


		public void		AddRow(string tableName, params object[] values)
		{
			var table = getTable(tableName);
			if (values.Length != table.Columns.Count)
				throw new ZException("Table [{0}] has {1} columns, but you've provided {2} values.", tableName, table.Columns.Count, values.Length);

			table.Rows.Add(new DataRow(values));
		}

		public void		RemoveRow(string tableName, string columnNameToFind, object valueToFind)
		{
			var table = getTable(tableName);
			var columnIndex = getColumnIndex(table, columnNameToFind);
			
			var rowToRemove = table.Rows.SingleOrDefault(r => r.Values[columnIndex].Equals(valueToFind.ToString()));
			if (rowToRemove == null)
				throw new ZException("Table [{0}] has no rows with specified data: column = {1}, value = {2}.",
					tableName, columnNameToFind, valueToFind.ToString());

			table.Rows.Remove(rowToRemove);
		}


		private Table	getTable(string tableName)
		{
			var table = DataTables.SingleOrDefault(dt => dt.Name.EqualsIC(tableName));
			if (table == null)
				throw new ZException("Table with specified name not found: {0}.", tableName);
			return table;
		}

		private bool	hasTable(string tableName)
		{
			return DataTables.Any(dt => dt.Name.EqualsIC(tableName));
		}


		private Column	getColumn(Table table, string columnName)
		{
			var column = table.Columns.SingleOrDefault(c => c.Name.EqualsIC(columnName));
			if (column == null)
				throw new ZException("Table [{0}] does not contain the column with specified name: {1}.", table.Name, columnName);
			return column;
		}

		private int		getColumnIndex(Table table, string columnName)
		{
			for (var i = 0; i < table.Columns.Count; i++)
			{
				if (table.Columns[i].Name.EqualsIC(columnName))
					return i;
			}
			throw new ZException("Table [{0}] does not contain the column with specified name: {1}.", table.Name, columnName);
		}

		private bool	hasColumn(Table table, string columnName)
		{
			return table.Columns.Any(c => c.Name.EqualsIC(columnName));
		}
    }
}
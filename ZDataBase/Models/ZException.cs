namespace ZDataBase.Models
{
	using System;


	public class ZException : Exception
	{
		public ZException(string message, params object[] values) : base(string.Format(message, values))
		{
		}
	}
}
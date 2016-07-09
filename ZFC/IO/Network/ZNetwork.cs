namespace ZFC.IO.Network
{
	using System;
	using System.Net;


	/// <summary>
	/// This class defines the static methods for networking purposes.
	/// </summary>
	public class ZNetwork
	{
		/// <summary>
		/// Gets data from specified remote address.
		/// </summary>
		/// <param name="uriAddress">String with Uri address.</param>
		/// <returns>Byte array with data from specified remote address.</returns>
		public static byte[]	Get_Data(string uriAddress)
		{
			var webClient = new WebClient();
			try
			{
				var resultContent = webClient.DownloadData(new Uri(uriAddress));
				return resultContent.Length != 0 ? resultContent : null;
			}
			catch	{	return null;	}
		}


		/// <summary>
		/// Checks whether remote file exists or not.
		/// </summary>
		/// <param name="urlAddress">String with URL address.</param>
		public static bool		RemoteFileExists(string urlAddress)
		{
			try
			{
				var request = WebRequest.Create(urlAddress) as HttpWebRequest;
				request.Method = "HEAD";
				var response = request.GetResponse() as HttpWebResponse;
				return (response.StatusCode == HttpStatusCode.OK);
			}
			catch
			{
				return false;
			}
		}
	}
}

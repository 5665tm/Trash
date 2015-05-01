// Changed: 2014 09 03 6:41 PM : 5665tm

namespace Molly
{
	internal class Coin
	{
		private readonly string _name;

		// json answer server
		// example:
		// {"success":1, "return":{ "funds":{
		//  "usd":325, "btc":23.998, "sc":121.998, "ltc":0, "ruc":0, "nmc":0},
		//  "rights":{ "info":1, "trade":1},
		//  "transaction_count":80, "open_orders":1, "server_time":1342123547 }}
		private static string _info = "   ";

		public double Volume { get { return Json.Parse(_info, _name); } }

		public Coin(string name)
		{
			_name = name;
		}

		public static void Refresh(string inf)
		{
			_info = inf;
		}
	}
}
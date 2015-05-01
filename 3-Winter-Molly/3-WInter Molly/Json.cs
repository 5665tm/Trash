// Changed: 2014 09 03 6:45 PM : 5665tm

using System;

// ReSharper disable UnusedMember.Local

namespace Molly
{
	internal static class Json
	{
		// example json answer:
		// {"success":1, "return":{ "funds":{
		//  "usd":325, "btc":23.998, "sc":121.998, "ltc":0, "ruc":0, "nmc":0},
		//  "rights":{ "info":1, "trade":1},
		//  "transaction_count":80, "open_orders":1, "server_time":1342123547 }}

		// parse for string argument (example - "usd")
		public static double Parse(string inf, string arg)
		{
			try
			{
				string[] words = inf.Split('{', ',', '}');
				double result = 0;
				for (int i = 0; i < words.Length; i++)
				{
					// if in words[i] there "usd"
					if (words[i].Contains(arg))
					{
						// for example, "usd": 34, then...
						words = words[i].Split(':');
						// ...words[0] - usd, words[1] - 34.
						result = Double.Parse(words[1]);
						break;
					}
				}
				return result;
			}
				// if incorrect json
			catch (SystemException)
			{
				return 0;
			}
		}

		// parse for number argument (example - 3)
		private static double Parse(string inf, int arg)
		{
			try
			{
				string[] words = inf.Split('{', ',', '}');
				// for example, if "usd": 34 , then...
				words = words[arg].Split(':');
				// words[0] - usd, words[1] - 34. Nya!
				return Double.Parse(words[1]);
			}
				// if incorrect json
			catch (SystemException)
			{
				return 0;
			}
		}
	}
}

// ReSharper restore UnusedMember.Local
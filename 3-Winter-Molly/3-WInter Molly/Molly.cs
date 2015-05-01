// Changed: 2014 09 03 7:33 PM : 5665tm

using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Molly
{
	internal static class Molly
	{
		// ### SETTING ### //
		private const string _APIKEY = "your apikey";
		private const string _SECRET = "your secret";

		private static void Main()
		{
			// create dictinary for object "Coin"
			string[] coinName = {"btc", "ltc", "nmc", "nvc", "trc", "ppc", "ftc", "xpm", "usd", "rur"};
			var coinDic = coinName.ToDictionary(t => t, t => new Coin(t));

			// create dictionary for object "Transmitter"
			string[] transmName =
			{
				"usd_rur", "ltc_usd", "nvc_usd", "nmc_usd", "ppc_usd", "btc_usd", "ltc_btc",
				"nvc_btc", "nmc_btc", "ppc_btc", "ftc_btc", "xpm_btc", "trc_btc"
			};
			var transmDic = transmName.ToDictionary(t => t,
				t => new Transmitter(coinDic[t.Split('_')[1]], coinDic[t.Split('_')[0]], t));

			string serverJsonAnswer = " ";
			var hashMaker = new HMACSHA512(Encoding.ASCII.GetBytes(_SECRET));

			// work!
			for (UInt32 previousSubTime = 0;;)
			{
				// delta time (for request data - &nonce=sub_time)
				double subTime = (DateTime.Now - new DateTime(1970, 1, 1)).TotalSeconds;

				// show time for refresh
				Console.CursorLeft = 0;
				Console.CursorTop = 0;
				Console.Write(Format(15 - ((subTime - 1)%15), 4));
				Thread.Sleep(110);

				// wait... if 0, 15, 30, 45 second -> starting!
				if ((UInt32) subTime%15 == 0 && (UInt32) subTime != previousSubTime)
				{
					previousSubTime = (UInt32) subTime;
					Console.CursorLeft = 0;
					Console.CursorTop = 0;
					Console.WriteLine("Please, wait");

					// create data (byte type) and hash (sha512)
					byte[] bData = Encoding.ASCII.GetBytes("nonce="
						+ (UInt32) subTime + Data());
					string hashData = BitConverter.ToString(hashMaker.ComputeHash(bData));
					hashData = hashData.Replace("-", "").ToLower();

					// refresh my info
					try
					{
						var req = (HttpWebRequest) WebRequest.Create("https://btc-e.com/tapi");
						req.Method = "POST";
						req.ContentType = "application/x-www-form-urlencoded";
						req.ContentLength = bData.Length;
						req.Headers.Add("Key", _APIKEY);
						req.Headers.Add("Sign", hashData);
						req.GetRequestStream().Write(bData, 0, bData.Length);
						HttpWebResponse resp;
						using (resp = (HttpWebResponse) req.GetResponse())
						{
							Stream istrm = resp.GetResponseStream();
							if (istrm != null)
							{
								var rdr = new StreamReader(istrm);
								serverJsonAnswer = rdr.ReadToEnd();
							}
						}
						Coin.Refresh(serverJsonAnswer);
					}
					catch (SystemException)
					{
						Console.WriteLine("Warning connection");
						Console.ReadLine();
					}

					// refresh course
					Console.CursorLeft = 0;
					Console.CursorTop = 9;
					Parallel.ForEach(transmDic, thTr => thTr.Value.Refresh());

					// calculate the amount of money
					double totalRur = (coinDic["ftc"].Volume*transmDic["ftc_btc"].Buy*transmDic["btc_usd"].Buy*transmDic["usd_rur"].Buy
						+ coinDic["trc"].Volume*transmDic["trc_btc"].Buy*transmDic["btc_usd"].Buy*transmDic["usd_rur"].Buy
						+ coinDic["xpm"].Volume*transmDic["xpm_btc"].Buy*transmDic["btc_usd"].Buy*transmDic["usd_rur"].Buy
						+ coinDic["ltc"].Volume*transmDic["ltc_usd"].Buy*transmDic["usd_rur"].Buy
						+ coinDic["nmc"].Volume*transmDic["nmc_usd"].Buy*transmDic["usd_rur"].Buy
						+ coinDic["nvc"].Volume*transmDic["nvc_usd"].Buy*transmDic["usd_rur"].Buy
						+ coinDic["btc"].Volume*transmDic["btc_usd"].Buy*transmDic["usd_rur"].Buy
						+ coinDic["ppc"].Volume*transmDic["ppc_usd"].Buy*transmDic["usd_rur"].Buy
						+ coinDic["usd"].Volume*transmDic["usd_rur"].Buy
						);

					// show current state (my info, course)
					Console.Clear();
					Console.WriteLine("               ;)         \"I LOVE My Job, Hard Rock and Coffee!!\"        {0}"
						, Format(totalRur)
						);
					Console.WriteLine("_______________________________________________________________________________");
					Console.WriteLine("rur {0}     btc {1}     ltc {2}   nmc {3}     nvc {4}"
						, Format(coinDic["rur"].Volume), Format(coinDic["btc"].Volume), Format(coinDic["ltc"].Volume)
						, Format(coinDic["nmc"].Volume), Format(coinDic["nvc"].Volume)
						);
					Console.WriteLine("trc {0}     ppc {1}     ftc {2}     xpm {3}     usd {4}"
						, Format(coinDic["trc"].Volume), Format(coinDic["ppc"].Volume), Format(coinDic["ftc"].Volume)
						, Format(coinDic["xpm"].Volume), Format(coinDic["usd"].Volume)
						);
					Console.WriteLine("_______________________________________________________________________________");
					Console.WriteLine("ltc/usd {0}      nvc/usd {1}      nmc/usd {2}      btc/usd {3}"
						, Format(transmDic["ltc_usd"].Buy), Format(transmDic["nvc_usd"].Buy)
						, Format(transmDic["nmc_usd"].Buy), Format(transmDic["btc_usd"].Buy)
						);
					Console.WriteLine("ppc/usd {0}      nvc/btc {1}      nmc/btc {2}      ppc/btc {3}"
						, Format(transmDic["ppc_usd"].Buy), Format(transmDic["nvc_btc"].Buy)
						, Format(transmDic["nmc_btc"].Buy), Format(transmDic["ppc_usd"].Buy)
						);
					Console.WriteLine("ftc/btc {0}      xpm/btc {1}      trc/btc {2}      ltc/btc {3}"
						, Format(transmDic["ftc_btc"].Buy), Format(transmDic["xpm_btc"].Buy)
						, Format(transmDic["trc_btc"].Buy), Format(transmDic["ltc_btc"].Buy)
						);
					Console.WriteLine("===============================================================================");
				}
			}
		}

		private static string Data()
		{
			return "&method=getInfo";
		}

		// 25.6 -> 25.6__, 0.5385938 -> 0.5385. Warning! if acc == 6 -> 75398390.73 -> 7539839! not correct!
		private static string Format(double a, int acc = 6)
		{
			string str = Convert.ToString(a);
			str += "                         ";
			return str.Substring(0, acc);
		}
	}
}
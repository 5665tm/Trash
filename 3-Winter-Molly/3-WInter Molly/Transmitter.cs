// Changed: 2014 09 03 6:45 PM : 5665tm

using System;
using System.IO;
using System.Net;

// ReSharper disable UnusedField.Compiler
// ReSharper disable NotAccessedField.Local

namespace Molly
{
	internal class Transmitter
	{
		private Coin _storage;
		private Coin _temp;
		// api url
		private readonly string _urlCourse;
		// example : 7/34 or 34/7?
		private bool _inverseCourse;
		// money in source or in temp?
		private bool _moneyInSource = false;
		// transmitter ON or OFF?
		private bool _on = false;
		private double _volumeSource;
		private double _volumeTemp;
		private double _buy;
		private double _sell;
		// command : action or idle(expect)?
		private bool _action = false;
		private string _ticker = " ";
		private HttpWebResponse _resp;

		public double Buy { get { return Json.Parse(_ticker, "buy"); } }

		public Transmitter(Coin storage, Coin temp, string course, bool inverseCourse = false)
		{
			_storage = storage;
			_temp = temp;
			_urlCourse = "https://btc-e.com/api/2/" + course + "/ticker";
			_inverseCourse = inverseCourse;
		}

		public void Refresh()
		{
			try
			{
				var req = (HttpWebRequest) WebRequest.Create(_urlCourse);
				req.Method = "POST";
				req.ContentType = "application/x-www-form-urlencoded";
				using (_resp = (HttpWebResponse) req.GetResponse())
				{
					Stream istrm = _resp.GetResponseStream();
					if (istrm != null)
					{
						var rdr = new StreamReader(istrm);
						_ticker = rdr.ReadToEnd();
					}
				}
				Console.WriteLine(_urlCourse);
			}
			catch (SystemException)
			{
				Console.WriteLine("Warning Connection");
				Console.ReadLine();
			}
		}
	}
}

// ReSharper restore NotAccessedField.Local
// ReSharper restore UnusedField.Compiler
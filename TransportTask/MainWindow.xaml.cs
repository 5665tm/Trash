// Last Change: 2015 02 28 16:25

using System.Collections.Generic;
using System.Windows;
using ExcelLib;

namespace TransportTask
{
	/// <summary>
	///     Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow
	{
		/// <summary>
		///     Координаты склада по широте
		/// </summary>
		private const float _CASTLE_LAT = 54.8557662f;

		/// <summary>
		///     Координаты склада по долготе
		/// </summary>
		private const float _CASTLE_LON = 83.0747375f;

		public MainWindow()
		{
			InitializeComponent();
			var table = new ExcelTable(@"D:\Desktop\2.xlsx", "A1:H88",
				typeof (int),
				typeof (int),
				typeof (int),
				typeof (float),
				typeof (float),
				typeof (string),
				typeof (string),
				typeof (string)
				);
			var listPoints = new List<Point>();
			listPoints.Add(new Point(_CASTLE_LAT, _CASTLE_LON, true));
			foreach (Record rec in table)
			{
				listPoints.Add(new Point(rec["lat"].f, rec["lon"].f));
			}
			MapWorker.Draw(Map, listPoints);
		}
	}
}
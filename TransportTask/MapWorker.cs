// Last Change: 2015 02 28 19:18

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TransportTask
{
	internal static class MapWorker
	{
		/// <summary>
		///     Скорость автомобиля
		/// </summary>
		private const double _SPEED = 50d;

		/// <summary>
		///     Список всех точек на карте
		/// </summary>
		private static List<Point> _listPoints = new List<Point>();

		/// <summary>
		///     Отрисовывает точки на карте
		/// </summary>
		/// <param name="map">Карта представленная канвасом</param>
		/// <param name="points">Точки для отрисовки на карте</param>
		public static void Draw(Canvas map, List<Point> points)
		{
			_listPoints = points;
			float minLat = points.Min(x => x.Lat);
			float minLon = points.Min(x => x.Lon);
			float maxLat = points.Max(x => x.Lat);
			float maxLon = points.Max(x => x.Lon);

			// расположение точек относительно друг друга
			// true - на карте будет свободное пространство сверху и снизу
			// false - на карте будет свободное пространство слева и справа
			bool narrow = (maxLat - minLat)/(maxLon - minLon) < (map.Height*1f)/map.Width;

			// вычислим число пикселей на градус
			double scale = narrow ? (map.Width*0.95)/(maxLon - minLon) : (map.Height*0.95)/(maxLat - minLat);

			foreach (var point in points)
			{
				var e = new Ellipse {StrokeThickness = 0, Stroke = Brushes.Black};
				if (point.IsCastle)
				{
					e.Fill = Brushes.DeepSkyBlue;
					e.Height = 10;
					e.Width = 10;
				}
				else
				{
					e.Fill = Brushes.Red;
					e.Height = 3;
					e.Width = 3;
				}
				map.Children.Add(e);
				double y = (point.Lat - minLat)*scale;
				double x = (point.Lon - minLon)*scale;
				e.Margin = new Thickness(x - (e.Width/2f) + 20, map.Height - 20 - (y + (e.Height/2f)), 0, 0);
			}
		}

		/// <summary>
		///     Возвращает расстояние в метрах между двумя точками
		/// </summary>
		/// <param name="p1">Точка 1</param>
		/// <param name="p2">Точка 2</param>
		/// <returns>Возвращает время в минутах между двумя точками</returns>
		private static double GetDistance(Point p1, Point p2)
		{
			return Math.Sqrt(Math.Pow(p1.Lon - p2.Lon, 2) + Math.Pow(p1.Lat - p2.Lat, 2))*100000;
		}

		/// <summary>
		///     Возвращает время в минутах между двумя точками
		/// </summary>
		/// <param name="p1">Точка 1</param>
		/// <param name="p2">Точка 2</param>
		/// <returns>Возвращает время в минутах между двумя точками</returns>
		private static double GetMinutes(Point p1, Point p2)
		{
			return (GetDistance(p1, p2)/1000f)/(_SPEED/60f);
		}
	}
}
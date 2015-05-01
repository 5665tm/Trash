// Last Change: 2015 02 28 16:07

namespace TransportTask
{
	/// <summary>
	///     Представляет собой точку на карте
	/// </summary>
	internal class Point
	{
		/// <summary>
		///     Координаты по широте
		/// </summary>
		public readonly float Lat;

		/// <summary>
		///     Координаты по долготе
		/// </summary>
		public readonly float Lon;

		/// <summary>
		///     Точка является складом?
		/// </summary>
		public readonly bool IsCastle;

		public Point(float lat, float lon, bool isCastle = false)
		{
			Lat = lat;
			Lon = lon;
			IsCastle = isCastle;
		}
	}
}
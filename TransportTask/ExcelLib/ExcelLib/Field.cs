using System;
using System.Globalization;

namespace ExcelLib
{
	/// <summary>
	/// 
	/// </summary>
	public class Field
	{
		public object obj;

		public Field(object obj)
		{
			this.obj = obj;
		}

		public static implicit operator float(Field d)
		{
			return (float) d.obj;
		}

		public static implicit operator Field(float d)
		{
			return new Field(d);
		}

		public static implicit operator int(Field d)
		{
			return (int) d.obj;
		}

		public static implicit operator Field(int d)
		{
			return new Field(d);
		}

		public static implicit operator string(Field d)
		{
			return d.obj.ToString();
		}

		public static implicit operator Field(string d)
		{
			return new Field(d);
		}

		public override string ToString()
		{
			return obj.ToString();
		}

		// ReSharper disable InconsistentNaming
		/// <summary>
		///     ¬озвращает представление пол€ в виде типа float
		/// </summary>
		/// <exception cref="Exception"></exception>
		public float f
		{
			get
			{
				float res;
				string str = (obj.ToString()).Replace(",", ".");
				bool isFloat = Single.TryParse(str, NumberStyles.Any, new CultureInfo("en-US"), out res);
				if (isFloat)
				{
					return res;
				}
				throw new Exception(String.Format("{0} not float type!", obj));
			}
		}

		/// <summary>
		///     ¬озвращает целочисленное представление пол€
		/// </summary>
		/// <exception cref="Exception"></exception>
		public int i
		{
			get
			{
				int res;
				bool isInt = Int32.TryParse(obj.ToString(), NumberStyles.Any, new CultureInfo("en-US"), out res);
				if (isInt)
				{
					return res;
				}
				throw new Exception(String.Format("{0} not float type!", obj));
			}
		}

		/// <summary>
		///     ¬озвращает строковое представление пол€
		/// </summary>
		public string s { get { return (string) obj; } }

		// ReSharper restore InconsistentNaming
	}
}
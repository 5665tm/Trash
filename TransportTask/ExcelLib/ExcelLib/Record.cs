using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExcelLib
{
	/// <summary>
	///     Представляет собой универсальный объект который может хранить любое число любых полей
	/// </summary>
	public class Record
	{
		/// <summary>
		/// </summary>
		/// <param name="name"></param>
		public Field this[string name] { set { Properties[GetNumFromName(name)] = new Field(value); } get { return Properties[GetNumFromName(name)]; } }

		private int GetNumFromName(string name)
		{
			for (int a = 0; a < _pinToExcelTable.Columns.Length; a++)
			{
				if (String.Equals(_pinToExcelTable.Columns[a].Name, name, StringComparison.InvariantCultureIgnoreCase))
				{
					return a;
				}
			}
			return -1;
		}

		/// <summary>
		///     Список полей объекта
		/// </summary>
		public Field[] Properties;

		/// <summary>
		///     Таблица к которой принадлежит запись
		/// </summary>
		private ExcelTable _pinToExcelTable;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="properties"></param>
		public Record(IEnumerable<object> properties)
		{
			object[] prop = properties.ToArray();
			Properties = new Field[prop.Length];
			for (int i = 0; i < Properties.Length; i++)
			{
				Properties[i] = new Field(prop[i]);
			}
		}

		public void SetTable(ExcelTable excelTable)
		{
			_pinToExcelTable = excelTable;
		}

		public override string ToString()
		{
			var sb = new StringBuilder();
			for (int i = 0; i < _pinToExcelTable.Columns.Length; i++)
			{
				sb.Append(_pinToExcelTable.Columns[i].Name + ": " + Properties[i] + ";");
				if (i < _pinToExcelTable.Columns.Length - 1)
				{
					sb.Append(" ");
				}
			}
			return sb.ToString();
		}
	}
}
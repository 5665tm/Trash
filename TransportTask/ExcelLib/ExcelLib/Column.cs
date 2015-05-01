using System;

namespace ExcelLib
{
	/// <summary>
	///     Представляет собой колонку в таблице
	/// </summary>
	public class Column
	{
		/// <summary>
		///     Имя колонки
		/// </summary>
		public readonly string Name;

		/// <summary>
		///     Тип данных хранящийся в колонке
		/// </summary>
		public readonly Type TypeField;

		/// <summary>
		///     Создает новую колонку в таблице
		/// </summary>
		/// <param name="name">Имя колонки</param>
		/// <param name="type">Тип данных хранящийся в колонке</param>
		public Column(string name, Type type)
		{
			Name = name;
			TypeField = type;
		}

		public override string ToString()
		{
			return Name + " : " + TypeField;
		}
	}
}
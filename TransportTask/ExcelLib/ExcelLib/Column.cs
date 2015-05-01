using System;

namespace ExcelLib
{
	/// <summary>
	///     ������������ ����� ������� � �������
	/// </summary>
	public class Column
	{
		/// <summary>
		///     ��� �������
		/// </summary>
		public readonly string Name;

		/// <summary>
		///     ��� ������ ���������� � �������
		/// </summary>
		public readonly Type TypeField;

		/// <summary>
		///     ������� ����� ������� � �������
		/// </summary>
		/// <param name="name">��� �������</param>
		/// <param name="type">��� ������ ���������� � �������</param>
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
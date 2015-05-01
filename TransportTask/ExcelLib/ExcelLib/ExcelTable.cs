// Last Change: 2015 02 28 16:13

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Microsoft.Office.Interop.Excel;
using ExcelApplication = Microsoft.Office.Interop.Excel.Application;

namespace ExcelLib
{
	/// <summary>
	/// </summary>
	public class ExcelTable : List<Record>
	{
		/// <summary>
		///     Список колонок таблицы
		/// </summary>
		public Column[] Columns;

		/// <summary>
		///     Адрес файла Excel с которым работает библиотека
		/// </summary>
		public string FilePath;

		/// <summary>
		///     Диапазон ячеек в формате "A1:B2"
		/// </summary>
		public string Range;

		// Итератор, возвращающий end-букв
		//		public new IEnumerator GetEnumerator()
		//		{
		//			for (int i = 0; i < Count; i++)
		//			{
		//				yield return this[i];
		//			}
		//		}

		/// <summary>
		///     Конструирует новый объект для работы с таблицой Excel
		/// </summary>
		/// <param name="filePath">Имя файла для обработки</param>
		/// <param name="range">Диапазон ячеек в формате "A1:B2"</param>
		/// <param name="types">Вручную указанные типы для колонок. Позволяет увеличить производительность</param>
		public ExcelTable(string filePath, string range, params Type[] types)
		{
			FilePath = filePath;
			Range = range;
			Run(filePath, range, types);
		}

		/// <summary>
		///     Отправляет все изменения в файл Excel
		/// </summary>
		public void Push()
		{
			StartExcel();

			Workbook workbook = _excel.Workbooks.Add(FilePath);
			Worksheet worksheet = workbook.Sheets[1];

			// узнаем числовые значения координат
			string range = Range.ToUpperInvariant();
			var coordString = range.Split(':');
			int[] coorditantes = new int[4];
			coorditantes[0] = LetterToNumber(coordString[0].Where(char.IsLetter).ToArray());
			coorditantes[1] = Convert.ToInt32(new string(coordString[0].Where(char.IsDigit).ToArray()));
			coorditantes[2] = LetterToNumber(coordString[1].Where(char.IsLetter).ToArray());
			coorditantes[3] = Convert.ToInt32(new string(coordString[1].Where(char.IsDigit).ToArray()));

			for (int i = 0; i < Count; i++)
			{
				for (int j = 0; j < this[i].Properties.Length; j++)
				{
					worksheet.Cells[i + coorditantes[1] + 1, j + coorditantes[0]] = this[i].Properties[j].ToString();
					//					worksheet.Cells[1, 1] = "fuck";
				}
			}

			//			var mv = Missing.Value;
			//			workbook.SaveAs(FilePath, XlFileFormat.xlWorkbookNormal, mv, mv, mv,
			//							mv, XlSaveAsAccessMode.xlExclusive, mv, mv, mv, mv, mv);
			//								workbook.Close();
			//								workbook = null;
			//								worksheet = null;
			//								// вызываем сборщик мусора
			//								GC.Collect();

			var b = _excel.GetSaveAsFilename(FilePath);

			workbook.SaveAs(b);
			workbook.Close();

			KillExcel();
		}

		/// <summary>
		///     Добавляет новую запись в таблицу
		/// </summary>
		/// <param name="args">Список аргументов</param>
		public void Add(params object[] args)
		{
			if (args.Length != Columns.Length)
			{
				throw new Exception("Number of arguments not equals number of columns");
			}
			var rec = new Record(args);
			rec.SetTable(this);
			base.Add(rec);
		}

		//		public void Remove(int index)
		//		{
		//			RemoveAt(index);
		//		}

		/// <summary>
		/// </summary>
		/// <param name="field"></param>
		/// <param name="value"></param>
		public void Remove(string field, object value)
		{
			for (int j = 0; j < Columns.Length; j++)
			{
				if (Columns[j].Name == field)
				{
					for (int i = 0; i < Count;)
					{
						if (this[i].Properties[j].ToString() == value.ToString())
						{
							RemoveAt(i);
						}
						else
						{
							i++;
						}
					}
					break;
				}
			}
		}

		private static ExcelApplication _excel;
		private static Hashtable _myHashtable;

		/// <summary>
		///     Запускает процесс сериализации
		/// </summary>
		/// <param name="file">Адрес файла</param>
		/// <param name="range">Диапазон клеток</param>
		/// <returns>Возвращаем отпарсенную таблицу</returns>
		public void Run(string file, string range, Type[] types)
		{
			range = range.ToUpperInvariant();
			var coordString = range.Split(':');
			StartExcel();
			Workbook workbook = _excel.Workbooks.Add(file);
			Worksheet worksheet = workbook.Sheets[1];

			// узнаем числовые значения координат
			var coorditantes = new int[4];
			coorditantes[0] = LetterToNumber(coordString[0].Where(char.IsLetter).ToArray());
			coorditantes[1] = Convert.ToInt32(new string(coordString[0].Where(char.IsDigit).ToArray()));
			coorditantes[2] = LetterToNumber(coordString[1].Where(char.IsLetter).ToArray());
			coorditantes[3] = Convert.ToInt32(new string(coordString[1].Where(char.IsDigit).ToArray()));

			// узнаем число столбцов и строк
			int numberOfColumns = coorditantes[2] - coorditantes[0] + 1;
			if (types.Length != 0 && numberOfColumns != types.Length)
			{
				throw new Exception("Number of arguments not equals number of columns");
			}
			int numberOfRecords = coorditantes[3] - coorditantes[1];
			var columns = new Column[numberOfColumns];
			// собираем информацию о всех столбцах
			for (int i = 0; i < numberOfColumns; i++)
			{
				Type type;

				// Если программист не указал типы хранящиеся в колонках, пытаемся определить их автоматически
				if (types.Length == 0)
				{
					// тип значений в столбце
					type = typeof (int);
					// проверяем значения на наличие разделителя целой части и соответствие шаблону числа
					// предполагаем тип значения по принципу строгости значения: int -> float -> string
					for (int j = coorditantes[1] + 1; j <= coorditantes[3]; j++)
					{
						string value = Convert.ToString(worksheet.Cells[j, coorditantes[0] + i].Text);
						value = value.Replace(",", ".");
						// может колонка содержит тип float?
						if (value.Contains(".") || type == typeof (float))
						{
							const string NUMBERS = "01234567890.";
							bool isFloat = true;
							foreach (var c in value.ToCharArray())
							{
								if (!NUMBERS.Contains(c))
								{
									isFloat = false;
									break;
								}
							}
							if (isFloat)
							{
								type = typeof (float);
							}
							else
							{
								type = typeof (string);
								break;
							}
						}
						else
						{
							int res;
							bool isInt = Int32.TryParse(value, NumberStyles.Any, new CultureInfo("en-US"), out res);
							if (!isInt)
							{
								type = typeof (string);
								break;
							}
						}
					}
				}
				else
				{
					type = types[i];
				}
				// имя столбца
				string name = worksheet.Cells[coorditantes[1], coorditantes[0] + i].Value;
				columns[i] = new Column(name, type);
			}
			Columns = columns.ToArray();

			for (int i = 0; i < numberOfRecords; i++)
			{
				var properties = new object[numberOfColumns];
				for (int j = 0; j < numberOfColumns; j++)
				{
					properties[j] = (object) Convert.ToString(worksheet.Cells[i + coorditantes[1] + 1, coorditantes[0] + j].Text);
				}
				base.Add(new Record(properties));
				this[i].SetTable(this);
			}
			KillExcel();

			// проверяем соответствие числа колонок и соответствие числа полей у всех объектов
			//			Column[] columnTypes = columns;
			//			int columnsLen = columnTypes.Count();
			//			for (int index = 0; index < numberOfRecords; index++)
			//			{
			//				var o = this[index];
			//				if (o.Properties.Length != columnsLen)
			//				{
			//					throw new Exception(string.Format(
			//						"Number of columns ({0}) not equals number of fields: ({1}) in object number {2}",
			//						columnsLen, o.Properties.Length, index));
			//				}
			//			}
		}

		/// <summary>
		///     Стартует Excel и заносит в HashTable Id процесса для его дальнейшего убийства
		/// </summary>
		private static void StartExcel()
		{
			//ищем все процессы excel
			var allProcesses = Process.GetProcessesByName("excel");
			var table = new Hashtable();
			int iCount = 0;
			foreach (var excelProcess in allProcesses)
			{
				table.Add(excelProcess.Id, iCount++);
			}
			// создаем новый процесс excel
			_excel = new ExcelApplication
			{
				SheetsInNewWorkbook = 1,
				ScreenUpdating = false,
				Visible = false
			};
			// узнаем ид только что созданного процесса excel
			allProcesses = Process.GetProcessesByName("excel");
			_myHashtable = new Hashtable();
			foreach (var excelProcess in
				allProcesses.Where(excelProcess => table.ContainsKey(excelProcess.Id) == false))
			{
				_myHashtable.Add(excelProcess.Id, iCount++);
			}
		}

		/// <summary>
		///     Уничтожает процесс екселя с котором мы работали
		/// </summary>
		private static void KillExcel()
		{
			var allProcesses = Process.GetProcessesByName("excel");
			foreach (var excelProcess in
				allProcesses.Where(excelProcess => _myHashtable.ContainsKey(excelProcess.Id)))
			{
				excelProcess.Kill();
			}
		}

		/// <summary>
		///     Конвертирует буквенный индекс в числовой. Пример AB => 28
		/// </summary>
		/// <param name="charLetter">Буквенный индекс</param>
		/// <returns>Числовой индекс</returns>
		private static int LetterToNumber(char[] charLetter)
		{
			int index = 0;
			string m = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
			for (int i = 0; i < charLetter.Length; i++)
			{
				int position = m.IndexOf(charLetter[i]) + 1;
				int factor = Convert.ToInt32(Math.Pow(26, (charLetter.Length - i - 1)));
				index += position*factor;
			}
			return index;
		}
	}
}
using System;
using System.Linq;
using ExcelLib;

namespace Demo
{
	class Program
	{
		static void Main()
		{
			// Парсим файл ексель
			ExcelTable excelTable = new ExcelTable(@"C:\Users\КараваевВЮ\Desktop\Cars.xlsx", "F14:J18");

			// Выводим информацию о записи 1
			Console.WriteLine(excelTable[1].ToString());
			
			// Выводим информацию о поле "Cost" записи 1
			Console.WriteLine(excelTable[1]["Cost"].f);

			// Присваиваем переменной типа float значение поля Cost записи 1
			float m = excelTable[1]["Cost"].f;

			// Присваиваем полю "Cost" записи 1 значение 19f
			excelTable[1]["Cost"] = 19f;

			// Снова выводим информацию о поле "Cost" записи 1
			Console.WriteLine(excelTable[1]["Cost"].f);

			// Снова выводим информацию о записи 1
			Console.WriteLine(excelTable[1]);

			// Добавляем новые записи
			excelTable.Add("Lamborgini", "Gallardo", 320, "Red", 50000f);
			excelTable.Add("Lada", "Kalina", 120, "Silver", 50f);
			excelTable.Add("Gaz", "Volga", 140, "Black", 300f);

			// Удаляем запись c указанным индексом
			excelTable.RemoveAt(3);

			// Удаляем запись по критерию
			excelTable.Remove("Name", "Mazda");

			// Класс поддерживает foreach
			foreach (var record in excelTable)
			{
				Console.WriteLine("--- " + record);
			}

			// Используем Linq - выбрать последнюю запись
			Console.WriteLine(excelTable.Last());

			// Используем Linq - отобразить записи автомобилей чья скорость выше 200 км/ч
			Console.WriteLine("\n*** Cars 200 kmh ***\n");
			foreach (var source in excelTable.Where(x => x["Speed"].i >= 200))
			{
				Console.WriteLine(source);
			}

			// Записываем все изменения в ексель
//			excelTable.Push();

			Console.ReadLine();
		}
	}
}
// Created 2014 08 06 6:58 PM
// Changed 2014 08 06 10:55 PM

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;

namespace NyanCompiler
{
	public static class Compiler
	{
		public static bool StartCompile(string sourceCode, string projectName)
		{
			if (!CheckSource(sourceCode))
			{
				return false;
			}
			// создаем сборку
			string filename = projectName + ".exe";

			//Для того, чтобы компилировать в уме сделать компилятор, необходимо разобраться с классом AssemblyBuilder.
			//Результатом операций над ним становится исполняемый файл или библиотека.
			AssemblyBuilder assemblyBuilder =
				AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("BrainFuck Compiled Program"),
					AssemblyBuilderAccess.RunAndSave); //Создаем сборку

			//Этот код  создаст пустую сборку. Для того, чтобы эта сборка не была пустой, ее надо наполнить модулями.
			ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(filename); //Создаем модуль в сборке

			//Этот код создаст модуль с именем равным имени сборки.
			//И создадим в нем класс Program
			TypeBuilder typeBuilder = moduleBuilder.DefineType("Program", TypeAttributes.Class); //Создаем класс в модуле

			//Теперь у нас есть класс. Все операции будем производить в нем.
			//Нам необходим массив – память и указатель в ней.
			FieldBuilder fieldMemory = typeBuilder.DefineField("Memory", typeof (byte[]), FieldAttributes.Static);
			// private byte[] Memory; //Память для операций.
			FieldBuilder fieldPoint = typeBuilder.DefineField("Point", typeof (int), FieldAttributes.Static);
			//private int Point; //Указатель в памяти.

			//Вся программа будет в Main.
			//Ну что ж, напишем Main и сделаем его точкой входа:
			MethodBuilder mainMethod = typeBuilder.DefineMethod("Main", MethodAttributes.Static, CallingConventions.Standard);
			//static void Main() //Main Procedure
			assemblyBuilder.SetEntryPoint(mainMethod.GetBaseDefinition());

			//Теперь необходимо инициализировать переменные, заведенные ранее. MSDN говорит, что это делается так:
			ILGenerator ilGenerator = mainMethod.GetILGenerator();
			ilGenerator.Emit(OpCodes.Ldc_I4, 300); //Загружаем в стек 300 - длина нового массива памяти
			ilGenerator.Emit(OpCodes.Newarr, typeof (byte)); //Создаем массив 30000 байтов
			ilGenerator.Emit(OpCodes.Stsfld, fieldMemory); //Ставим указатель Memory на созданный массив

			var scopes = new Stack<Label>();

			var operandString = sourceCode.Split(new []{"\r\n"}, StringSplitOptions.RemoveEmptyEntries);

			foreach (var t in operandString) //Так как каждый оператор самодостаточен, то можно просто записывать каждый.
			{
				Console.WriteLine(operandString);
				var operand = t.Split(' ')[0].Trim().ToLower();
				var result = t.Split(' ')[1].Trim().ToCharArray();
				switch (operand)
				{
					case "meow":
					{
						foreach (var leter in result)
						{
							ilGenerator.Emit(OpCodes.Ldsfld, fieldMemory); //Помещаем в стек поле MEMORY
							ilGenerator.Emit(OpCodes.Ldsfld, fieldPoint); //Помещаем в стек поле POINT
							ilGenerator.EmitCall(OpCodes.Call, typeof (Console).GetMethod("ReadLine"), new[] {typeof (string)});
							//Console.ReadLine();
							ilGenerator.Emit(OpCodes.Call, typeof (Convert).GetMethod("ToByte", new[] {typeof (string)}));
							//Конвертация в байт.
							ilGenerator.Emit(OpCodes.Stelem_I1); //Сохраняем
						}
						foreach (var leter in result)
						{
							ilGenerator.Emit(OpCodes.Ldsfld, fieldMemory); //Помещаем в стек поле MEMORY
							ilGenerator.Emit(OpCodes.Ldsfld, fieldPoint); //Помещаем в стек поле POINT
							ilGenerator.Emit(OpCodes.Ldelem_U1); //Помещаем в стек элемент MEMORY[POINT]
							ilGenerator.EmitCall(OpCodes.Call, typeof (Console).GetMethod("Write", new[] {typeof (char)}),
								new[] {typeof (char)}); //Console.WriteLine(MEMORY[POINT]);
							ilGenerator.Emit(OpCodes.Nop);
						}
						break;
					}
					case ">":
					{
						ilGenerator.Emit(OpCodes.Ldsfld, fieldPoint); //Помещаем в стек поле POINT
						ilGenerator.Emit(OpCodes.Ldc_I4_1); //Помещаем в стек 1
						ilGenerator.Emit(OpCodes.Add); //Плюс
						ilGenerator.Emit(OpCodes.Stsfld, fieldPoint); //Выполняем операцию с Point то есть сдвигаем указатель вправо
						break;
					}
					case "<":
					{
						ilGenerator.Emit(OpCodes.Ldsfld, fieldPoint); //Помещаем в стек поле POINT
						ilGenerator.Emit(OpCodes.Ldc_I4_1); //Помещаем в стек 1
						ilGenerator.Emit(OpCodes.Sub); //Минус
						ilGenerator.Emit(OpCodes.Stsfld, fieldPoint); //Выполняем операцию с Point то есть сдвигаем указатель вправо
						break;
					}
					case "+":
					{
						ilGenerator.Emit(OpCodes.Ldsfld, fieldMemory); //Помещаем в стек поле MEMORY
						ilGenerator.Emit(OpCodes.Ldsfld, fieldPoint); //Помещаем в стек поле POINT
						ilGenerator.Emit(OpCodes.Ldelema, typeof (byte)); //Помещаем в стек элемент MEMORY[POINT]
						ilGenerator.Emit(OpCodes.Dup);
						ilGenerator.Emit(OpCodes.Ldobj, typeof (byte));
						ilGenerator.Emit(OpCodes.Ldc_I4_1);
						ilGenerator.Emit(OpCodes.Add); //Плюс
						ilGenerator.Emit(OpCodes.Conv_U1);
						ilGenerator.Emit(OpCodes.Stobj, typeof (byte)); //Сохраняем
						break;
					}
					case "-":
					{
						ilGenerator.Emit(OpCodes.Ldsfld, fieldMemory); //Помещаем в стек поле MEMORY
						ilGenerator.Emit(OpCodes.Ldsfld, fieldPoint); //Помещаем в стек поле POINT
						ilGenerator.Emit(OpCodes.Ldelema, typeof (byte)); //Помещаем в стек элемент MEMORY[POINT]
						ilGenerator.Emit(OpCodes.Dup);
						ilGenerator.Emit(OpCodes.Ldobj, typeof (byte));
						ilGenerator.Emit(OpCodes.Ldc_I4_1);
						ilGenerator.Emit(OpCodes.Sub); //Минус
						ilGenerator.Emit(OpCodes.Conv_U1);
						ilGenerator.Emit(OpCodes.Stobj, typeof (byte)); //Сохраняем
						break;
					}
					case "[":
					{
						var lbl = ilGenerator.DefineLabel(); //Объявляем метку
						ilGenerator.MarkLabel(lbl); //И помечаем ей то место куда будем возвращаться, то есть начало цикла
						scopes.Push(lbl); //В стек. Иначе не узнаем куда :)
						break;
					}
					case "]":
					{
						ilGenerator.Emit(OpCodes.Ldsfld, fieldMemory); //Конструкция из этих 3х элементов
						ilGenerator.Emit(OpCodes.Ldsfld, fieldPoint); //загружает в стек значение из массива 
						ilGenerator.Emit(OpCodes.Ldelem_U1); //FDB_1 по адресу FDB_2
						ilGenerator.Emit(OpCodes.Ldc_I4_0); //Загружаем в стек 0
						ilGenerator.Emit(OpCodes.Ceq); // Если текущая ячейка=0
						ilGenerator.Emit(OpCodes.Brtrue, 5); //Переход на послеследующую инструкцию
						ilGenerator.Emit(OpCodes.Br, scopes.Pop()); //Иначе на начало цикла. Занимает 5 байт.
						break;
					}
					case ".":
					{
						ilGenerator.Emit(OpCodes.Ldsfld, fieldMemory); //Помещаем в стек поле MEMORY
						ilGenerator.Emit(OpCodes.Ldsfld, fieldPoint); //Помещаем в стек поле POINT
						ilGenerator.Emit(OpCodes.Ldelem_U1); //Помещаем в стек элемент MEMORY[POINT]
						ilGenerator.EmitCall(OpCodes.Call, typeof (Console).GetMethod("Write", new[] {typeof (char)}),
							new[] {typeof (char)}); //Console.WriteLine(MEMORY[POINT]);
						ilGenerator.Emit(OpCodes.Nop);
						break;
					}
					case ",":
					{
						ilGenerator.Emit(OpCodes.Ldsfld, fieldMemory); //Помещаем в стек поле MEMORY
						ilGenerator.Emit(OpCodes.Ldsfld, fieldPoint); //Помещаем в стек поле POINT
						ilGenerator.EmitCall(OpCodes.Call, typeof (Console).GetMethod("ReadLine"), new[] {typeof (string)});
						//Console.ReadLine();
						ilGenerator.Emit(OpCodes.Call, typeof (Convert).GetMethod("ToByte", new[] {typeof (string)}));
						//Конвертация в байт.
						ilGenerator.Emit(OpCodes.Stelem_I1); //Сохраняем
						break;
					}
				}
			}
			ilGenerator.EmitCall(OpCodes.Call, typeof (Console).GetMethod("ReadLine"), new[] {typeof (string)});
			//Console.ReadLine();


			ilGenerator.Emit(OpCodes.Ret); //Заканчиваем
			typeBuilder.CreateType(); //Завершаем класс
			assemblyBuilder.Save(filename); //Сохраняем сборку

			Process.Start(filename);

			return true;
		}

		private static bool CheckSource(string sourceCode)
		{
			if (sourceCode.Length == 0)
			{
				return false;
			}
			return true;
		}
	}
}
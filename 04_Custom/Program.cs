using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Custom
{
	class Program
	{
		static void Main(string[] args)
		{
			// оборачиваем вызов метода в делегат

			Action action = LongWork;

			// у класса Delegate есть метод для асинхронного выполнения

			action.BeginInvoke(ar => 
				{
					try
					{
						action.EndInvoke(ar);
                        Console.WriteLine("Long work completed");
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
					}
				}, 
				null);

			// в основном потоке выполняем какую-то полезную работу

			Thread.Sleep(1000);

			Console.WriteLine("Main completed");

			Console.Read();
		}

		static void LongWork()
		{
			Thread.Sleep(1000);
			throw new Exception("Long exception");
		}
	}
}

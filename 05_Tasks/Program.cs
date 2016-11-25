using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tasks
{
	class Program
	{
		static void Main(string[] args)
		{

            var task3 = new Task(() =>
            {
                Thread.Sleep(1000);
                Console.WriteLine("Task 3 completed");
            });

            // запускаем выполнение

            task3.Start();


			// определяем делегат,
			// который необходимо выполнить асинхронно,
			// и запускаем выполнение немедленно

			var task1 = Task.Factory.StartNew(() =>
				{
					Thread.Sleep(1000);
					Console.WriteLine("Task 1 completed");
				});

			// или

			// Task.Run() - это шорткат на TaskFactory.StartNew()
			// нужно знать, какие именно значения по умолчанию используются,
			// иначе поведение может быть не очевидным

			var task2 = Task.Run(() =>
				{
					Thread.Sleep(1000);
					Console.WriteLine("Task 2 completed");
				});

			// или

			// если требуется разделить во времени создание задачи и её выполнение
			// (это может понадобиться, если после создания задачи нужно выполнить с ней какие-либо действия,
			// например, подписаться на её события, передать внешнему коду и т.п.),
			// то используем непосредственно класс Task


			Console.Read();
		}
	}
}

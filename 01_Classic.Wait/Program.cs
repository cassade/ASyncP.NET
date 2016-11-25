using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Classic.Wait
{
	class Program
	{
		static void Main(string[] args)
		{
			// будем читать данные конфигурации приложения с диска

			var stream = new FileStream(Assembly.GetExecutingAssembly().Location + ".config", FileMode.Open);
			var buffer = new Byte[stream.Length];

            // класс, предоставляющий возможность выполнения некоторых операций асинхронно,
            // имеет как правило три метода:
            // - XXX() - выполняет операцию синхронно
            // - BeginXXX() - запускает асинхронную операцию и возвращает управление
            // - EndXXX() - возвращает результат выполнения операции

			// запускаем чтение в параллельном потоке

			var result = stream.BeginRead(buffer, 0, buffer.Length, null, null);

			// в основном потоке выполняем какую-то полезную работу

			Thread.Sleep(1000);

			Console.WriteLine("Main completed");

			// проверить или подождать окончания асинхронной операции можно так:

			while (!result.IsCompleted)
			{
				Thread.Sleep(10);
			}

			// или так:

			result.AsyncWaitHandle.WaitOne();

			// или просто вызывать EndXXX, 
			// который заблокирует вызывающий поток до окончания операции

			// EndXXX нужно вызывать в любом случае,
            // чтобы завершить операцию, освободить ресурсы и получить её результат

            try
            {
                stream.EndRead(result);
                Console.WriteLine(Encoding.UTF8.GetString(buffer));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

			Console.Read();
		}
	}
}

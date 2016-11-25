using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Classic.Callback
{
	class Program
	{
		static void Main(string[] args)
		{
			// будем читать данные конфигурации приложения с диска

			var stream = new FileStream(Assembly.GetExecutingAssembly().Location + ".config", FileMode.Open);
			var buffer = new Byte[stream.Length];

			// определяем метод обратного вызова,
			// в котором получаем результат асинхронной операции

			AsyncCallback callback = asyncResult =>
			{
                try
                {
                    stream.EndRead(asyncResult);
                    Console.WriteLine(Encoding.UTF8.GetString(buffer));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
			};

			// запускаем чтение в параллельном потоке

			var ar = stream.BeginRead(buffer, 0, buffer.Length, callback, null);

			// в основном потоке выполняем какую-то полезную работу

			Thread.Sleep(1000);

			Console.WriteLine("Main completed");

			Console.Read();
		}
	}
}

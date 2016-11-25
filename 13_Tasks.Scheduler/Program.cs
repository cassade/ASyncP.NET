using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tasks.Scheduler
{
	class Program
	{
		static void Main(string[] args)
		{
			// выделением ресурсов под задачи занимается TaskScheduler,
			// по умолчанию используется ThreadPool
			
			// в примерах к TPL от Microsoft есть собственная реализация планировщика,
			// ограничивающего количество используемых потоков двумя

			// задаем используемый планировщик

			var factory = new TaskFactory(TaskScheduler.Default);

			// это функциональный класс,
			// полезной информации из него можно извлечь немного

			Console.WriteLine(factory.Scheduler.MaximumConcurrencyLevel);

			Console.Read();
		}
	}
}

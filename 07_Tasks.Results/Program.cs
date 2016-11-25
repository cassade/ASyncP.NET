using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tasks.Results
{
	class Program
	{
		static void Main(string[] args)
		{
			var task = Task.Run<int>(() =>
				{
					Thread.Sleep(1000);
					return 42;
				});

			// явное ожидание не обязательно,
			// обращение к Task.Result блокирует вызывающий поток до появления результата
			// по аналогии с методом EndXXX() в классической модели

			// task.Wait();

			Console.WriteLine("The answer is {0}!", task.Result);
			Console.Read();
		}
	}
}

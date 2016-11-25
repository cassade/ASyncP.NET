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
			// асинхронный запуск операции

			var task = Task.Run(() =>
				{
					Thread.Sleep(1000);
				});

			// System.Threading.Tasks.Task : IAsyncResult

			while (!task.IsCompleted)
			{
				Thread.Sleep(10);
			}

			// некоторые члены IAsyncResult реализованы явно

			((IAsyncResult)task).AsyncWaitHandle.WaitOne();

			// самый короткий способ

			// Task.Wait() блокирует вызывающий поток до окончания операции

			task.Wait();

			Console.WriteLine("Task completed");

			Console.WriteLine("Task status: {0},\n\t.IsCompleted: {1}",
				task.Status,
				task.IsCompleted);

			Console.Read();
		}
	}
}

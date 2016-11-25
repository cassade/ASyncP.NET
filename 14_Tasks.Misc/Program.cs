using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tasks.Misc
{
	class Program
	{
		static void Main(string[] args)
		{
			var stopwatch = Stopwatch.StartNew();

			var tasks = new List<Task<int>>
			{
				Task.Run(() => { Thread.Sleep(1000); Console.WriteLine("Task 1 completed"); return 1; }),
				Task.Run(() => { throw new Exception("!!!"); Thread.Sleep(1000); Console.WriteLine("Task 2 completed"); return 2; }),
				Task.Run(() => { Thread.Sleep(1000); Console.WriteLine("Task 3 completed"); return 3; }),
			};

			// Task.WhenAll() возвращает задачу, которая завершится после выполнения всех задач,
			// результатом этой задачи будет массив результатов исходных задач

			Task.WhenAll(tasks)
				.ContinueWith(all =>
				{
					Console.WriteLine("All completed in {0} msec, Result: {1}",
						stopwatch.ElapsedMilliseconds,
						all.Result.Sum());
				});;

			// Task.WhenAny() возвращает задачу, которая завершится после выполнения любой из задач,
			// результатом этой задачи будет первая завершившаяся задача

            //Task.WhenAny(tasks)
            //    .Unwrap()
            //    .ContinueWith(any =>
            //    {
            //        Console.WriteLine("Any completed in {0} msec, Result: {1}",
            //            stopwatch.ElapsedMilliseconds,
            //            any.Result);
            //    });

            //// можно подождать выполнения всех или любой из задач

            //Task.WaitAny(tasks.ToArray());
            //Task.WaitAll(tasks.ToArray());

			// вспомогательный метод для задач временной задержки

			stopwatch = Stopwatch.StartNew();

			Task.Delay(1000)
				.ContinueWith(delay =>
				{
					Console.WriteLine();
					Console.WriteLine("Delay completed in {0} msec",
						stopwatch.ElapsedMilliseconds);
				});

			// вспомогательный метод для возврата результата в асинхронном стиле
			// без фактического асинхронного запуска задачи

			Task.FromResult<int>(42)
				.ContinueWith(theQuestion =>
				{
					Console.WriteLine();
					Console.WriteLine("The answer is " + theQuestion.Result);
				});

			Console.Read();
		}
	}
}

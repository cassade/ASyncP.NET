using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tasks.Exceptions
{
	class Program
	{
		static void Main(string[] args)
		{
			// запускаем задачу, выбрасывающую исключение

			var task = Task.Factory.StartNew<int>(() =>
			{
				throw new InvalidOperationException();
			});

			// ожидаем завершения

			while(!task.IsCompleted)
			{
				Thread.Sleep(10);
			}

			// после выполнения задачи можно проверить свойства Task.IsFaulted и Task.Exception

			Console.WriteLine("Task status: {0},\n\t.IsCompleted: {1},\n\t.IsFaulted: {2},\n\t.Exception: {3}",
				task.Status,
				task.IsCompleted,
				task.IsFaulted,
				task.Exception == null ? "null" : task.Exception.GetType().Name);

			// для обработки ошибок достаточно обернуть обращение к TaskResult в try { } catch { },
 			// при этом будет выброшено AggregateException, свойство InnerException которого
			// будет содержать актуальное исключение

			try
			{
				Console.WriteLine("The answer is {0}!", task.Result);
			}
			catch (Exception ex)
			{
				Console.WriteLine();
				Console.WriteLine("{0} : {1}", ex.GetType().Name, ex.Message);
				Console.WriteLine("{0} : {1}", ex.InnerException.GetType().Name, ex.InnerException.Message);
			}

			// при вызове Task.Wait() также следует ожидать исключительных ситуаций

			try
			{
				task.Wait();
			}
			catch (Exception ex)
			{
				Console.WriteLine();
				Console.WriteLine("{0} : {1}", ex.GetType().Name, ex.Message);
				Console.WriteLine("{0} : {1}", ex.InnerException.GetType().Name, ex.InnerException.Message);
			}

			Console.Read();
		}
	}
}

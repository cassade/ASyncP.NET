using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tasks.Cancellation
{
	class Program
	{
		static void Main(string[] args)
		{
			var tokenSource = new CancellationTokenSource();
			var token = tokenSource.Token;

			Action action = () =>
				{
					// выполненяем действие и периодически проверем,
					// не нужно ли отменить выполнение

					while (!token.IsCancellationRequested)
					{
						Thread.Sleep(300);
					}

					// ИЛИ регистрируем обработчик запроса на отмену операции

					token.Register(() =>
						{
							Console.WriteLine("Cancellation requested");
						});

					// непосредственно для отмены операции нужно
					// выбросить OperationCanceledException,
					// оно будет обработано инфраструкторой как сигнал об отмене операции

					token.ThrowIfCancellationRequested();

					// ИЛИ его можно выбросить вручную,
					// но важно не забыть передать в конструктор именно тот
					// экземпляр CancellationToken,
					// который использовался для запроса на отмену

					throw new OperationCanceledException(token);
				};

			var task = Task.Run(action, token);

			Thread.Sleep(1000);

			Console.WriteLine("Main completed");

			// запрашиваем отмену операции

			tokenSource.Cancel();

			// дожидаемся завершения операции
			// при этом будет выброешено исключение AggregateException,
			// свойство InnerException которого будет содержать
			// TaskCanceledException (не OperationCanceledException).

			try
			{
				task.Wait();
			}
			catch (Exception ex)
			{
				Console.WriteLine();

				Console.WriteLine("Task status: {0},\n\t.IsCompleted: {1},\n\t.IsFaulted: {2},\n\t.Exception: {3},\n\t.IsCanceled: {4}",
					task.Status,
					task.IsCompleted,
					task.IsFaulted,
					task.Exception == null ? "null" : task.Exception.GetType().Name,
					task.IsCanceled);

				Console.WriteLine();

				Console.WriteLine("{0} : {1}", ex.GetType().Name, ex.Message);
				Console.WriteLine("{0} : {1}", ex.InnerException.GetType().Name, ex.InnerException.Message);
			}

			Console.Read();
		}
	}
}

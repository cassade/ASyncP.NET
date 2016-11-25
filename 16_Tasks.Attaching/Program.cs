using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tasks.Attaching
{
	class Program
	{
		static void Main(string[] args)
		{
			// внутреннюю задачу можно привязать к внешней,
			// тем самым синхронизируя их выполнение,
			// при этом нельзя использовать Task.Run() для внешней задачи,
			// так как по умолчанию там используется TaskCreationOptions.DenyChildAttach,
			// что приведет к успешному выполнению каждой задачи без выполнения привязки

			Console.WriteLine("AttachedToParent");

			Task.Factory.StartNew(() =>
					{
						Task.Factory.StartNew(() =>
								{
									Thread.Sleep(1000);
								},
								TaskCreationOptions.AttachedToParent)
							.ContinueWith(_ =>
								{
									Console.WriteLine("Inner completed");
								});
					})
				.ContinueWith(_ => 
					{
						Console.WriteLine("Outer completed");
					})
				.Wait();

			// привязка обеспечивает пробрасывание исключений вверх по стеку задач

			Console.WriteLine("\nAttachedToParent - Exception Handling");

			var task1 = Task.Factory.StartNew(() =>
				{
					Task.Factory.StartNew(() =>
						{
							throw new Exception("Inner Task Exception");
						},
						TaskCreationOptions.AttachedToParent);
				});

			while (!task1.IsCompleted)
			{
				Thread.Sleep(100);
			}

			Console.WriteLine("Task 1 status: {0},\n\t.IsCompleted: {1},\n\t.IsFaulted: {2},\n\t.Exception: {3},\n\t.IsCanceled: {4}",
				task1.Status,
				task1.IsCompleted,
				task1.IsFaulted,
				task1.Exception == null ? "null" : task1.Exception.GetType().Name,
				task1.IsCanceled);

			// а вот отмену нужно отслеживать в каждой задаче по отдельности

			Console.WriteLine("\nAttachedToParent - Cancellation");

			var tokenSource = new CancellationTokenSource();
			var token = tokenSource.Token;

			tokenSource.CancelAfter(500);

			var task2 = Task.Factory.StartNew(() =>
				{
					Task.Factory.StartNew(() =>
						{
							while (!token.IsCancellationRequested)
							{
								Thread.Sleep(100);
								token.ThrowIfCancellationRequested();
							}
						},
						token,
						TaskCreationOptions.AttachedToParent,
						TaskScheduler.Default);
				},
				token);

			while (!task2.IsCompleted)
			{
				Thread.Sleep(100);
			}

			Console.WriteLine("Task 2 status: {0},\n\t.IsCompleted: {1},\n\t.IsFaulted: {2},\n\t.Exception: {3},\n\t.IsCanceled: {4}",
				task2.Status,
				task2.IsCompleted,
				task2.IsFaulted,
				task2.Exception == null ? "null" : task2.Exception.GetType().Name,
				task2.IsCanceled);

			Console.Read();
		}
	}
}

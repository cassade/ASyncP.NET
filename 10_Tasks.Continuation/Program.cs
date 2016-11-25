using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tasks.Continuation
{
	class Program
	{
		static void Main(string[] args)
		{
			// задачи можно выполнять последовательно

			Console.WriteLine("Chain");

			var task1 = new Task<int>(() => 
				{
					Console.WriteLine("1");
                    return 42;
				});

			var task2 = task1.ContinueWith(previous =>
				{
                    Console.WriteLine(previous.Result);
					Console.WriteLine("2");
				});

			// запускать нужно начальную задачу,
			// task2.Start() - выбросит InvalidOperationException,
			// а ожидать выполнения - последней

			task1.Start();
			task2.Wait();

			// с помощью TaskContinuationOptions можно управлять условиями запуска задач-продолжений
			// и/или предоствлять дополнительную информацию планировщику

			// планировщик старается распараллелить задачи

			Console.WriteLine("\nExecuteSynchronously - without");
			var t = Task.Run(() =>
			{
				Console.WriteLine(Thread.CurrentThread.ManagedThreadId);
			});
			Task.WaitAll(new[] 
			{
				t.ContinueWith(_ => Console.WriteLine(Thread.CurrentThread.ManagedThreadId)),
				t.ContinueWith(_ => Console.WriteLine(Thread.CurrentThread.ManagedThreadId))
			});

			// запрашиваем выполнение продолжений в потоке предыдущей задачи

			Console.WriteLine("\nExecuteSynchronously - with");
			t = Task.Run(() =>
			{
				Console.WriteLine(Thread.CurrentThread.ManagedThreadId);
			});
			Task.WaitAll(new[] 
			{
				t.ContinueWith(_ => Console.WriteLine(Thread.CurrentThread.ManagedThreadId), TaskContinuationOptions.ExecuteSynchronously),
				t.ContinueWith(_ => Console.WriteLine(Thread.CurrentThread.ManagedThreadId), TaskContinuationOptions.ExecuteSynchronously)
			});

			// задание условий выполнения для цепочки задач

			Console.WriteLine("\nChain options");

			var tokenSource = new CancellationTokenSource();
			var token = tokenSource.Token;

			tokenSource.Cancel();

			Task.Run(() => 
					{ 
						throw new Exception(); 
					})
				.ContinueWith(_ => Console.WriteLine("1"), TaskContinuationOptions.NotOnFaulted)
				.ContinueWith(_ => Console.WriteLine("2"))
				.ContinueWith(_ => 
					{
						Console.WriteLine("3");
						token.ThrowIfCancellationRequested();
					}, 
					token/*,
					TaskContinuationOptions.LazyCancellation,
					TaskScheduler.Default*/)
                .ContinueWith(_ => Console.WriteLine("22"))
                .ContinueWith(_ =>
                    {
                        Console.WriteLine("33");
                        token.ThrowIfCancellationRequested();
                    },
                    token/*,
                    TaskContinuationOptions.LazyCancellation,
                    TaskScheduler.Default*/)
				.ContinueWith(_ => Console.WriteLine("4"), TaskContinuationOptions.NotOnCanceled)
				.ContinueWith(_ => Console.WriteLine("5"), TaskContinuationOptions.OnlyOnRanToCompletion)
                .ContinueWith(_ => { Console.WriteLine("6"); return "result of 6"; })
                .ContinueWith(_ => Console.WriteLine(_.Result))
                ;
		
			Console.Read();
		}
	}
}

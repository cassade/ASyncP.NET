using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tasks.Unwrap
{
    class Program
    {
        static void Main(string[] args)
        {
			// если не извлечь вложенную задачу через Task.Unwrap(),
			// то в момент выполнения продолжения она еще не будет выполнена

			Console.WriteLine("Unwrap");

            Task.Factory.StartNew(() => 
                    {
                        return Task.Run(() => 1);
                    })
                .Unwrap()
                .ContinueWith(prev =>
                    {
                        Console.WriteLine(prev.Result);
                        return Task.Run(() => 2);
                    })
                .Unwrap()
                .ContinueWith(prev =>
                    {
                        Console.WriteLine(prev.Result);
                    })
				.Wait();

			// одна из перегрузок Task.Run() уже содержит логику Unwrap()

			Console.WriteLine("\nRun - Auto Unwrap");

			Task.Run(() => Task.Run(() => 3))
			  //.Unwrap() не требуется
				.ContinueWith(prev => Console.WriteLine(prev.Result))
				.Wait();

            Console.Read();
        }
    }
}

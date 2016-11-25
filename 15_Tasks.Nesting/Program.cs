using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tasks.Nesting
{
	class Program
	{
		static void Main(string[] args)
		{
			// задачи можно вкладывать друг в друга,
			// но в этом случае задачи выполняются независимо

			Task.Run(() =>
					{
						Task.Delay(1000)
							.ContinueWith(_ =>
								{
									Console.WriteLine("Inner completed");
								});
					})
				.ContinueWith(_ =>
					{
						Console.WriteLine("Outer completed");
					});

			Console.Read();
		}
	}
}

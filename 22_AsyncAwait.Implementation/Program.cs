using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncAwait.Implementation
{
	class Program
	{
		static void Main(string[] args)
		{
            // подобно тому, как для цикла foreach требуется лишь объект с методом IEnumerator GetEnumerator(),
            // для оператора await требуется любой объект с методом INotifyCompletion GetAwaiter()

            Func<Task> start = async () =>
                {
                    await new MyAsync();
                };

            start().Wait();

            Console.Read();
		}
	}

    public class MyAsyncAwaiter : System.Runtime.CompilerServices.INotifyCompletion
    {
        private int timeout;

        public MyAsyncAwaiter(int timeout)
        {
            this.timeout = timeout;
        }

        public bool IsCompleted
        {
            get
            {
                Console.WriteLine("IsCompleted called");
                return timeout <= 0;
            }
        }
        public void OnCompleted(Action continuation)
        {
            Console.WriteLine("OnCompleted called");
            Task.Delay(timeout).ContinueWith(_ =>
            {
                Console.WriteLine("Before continuation");
                timeout = 0;
                continuation();
                Console.WriteLine("After continuation");
            });
        }
        public void GetResult()
        {
            Console.WriteLine("GetResult called");
        }
    }

    public class MyAsync
    {
        public MyAsyncAwaiter GetAwaiter()
        {
            return new MyAsyncAwaiter(3000);
        }
    }
}

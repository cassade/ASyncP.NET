using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncAwait
{
    class Program
    {
        static void Main(string[] args)
        {
            // сам по себе модификатор async не делает метод асинхронным
            // он разрешает использовать внутри метода оператор await,
            // а также предписывает компилятору специальным образом, через задачи,
            // обрабатывать результат выполнения и выброшенные методом исключения,
            // см. пример в проекте AsyncAwait.Exceptions

            Console.WriteLine("async method is not async");

            Console.WriteLine(Thread.CurrentThread.ManagedThreadId);

            PseudoAsync();

            ReallyAsync();

            // async метод должен возвращать Task, Task<T> или void

            Console.WriteLine();
            Console.WriteLine("async config");

            Read();

            Console.Read();
        }

        static async Task PseudoAsync()
        {
            Console.WriteLine(Thread.CurrentThread.ManagedThreadId);
        }

        static async Task ReallyAsync()
        {
            await Task.Run(() =>
                {
                    Console.WriteLine(Thread.CurrentThread.ManagedThreadId);
                });
        }

        static async Task Read()
        {
            using (var stream = new FileStream(Assembly.GetExecutingAssembly().Location + ".config", FileMode.Open))
            {
                var buffer = new Byte[stream.Length];

                // запускаем чтение файла асинхронно
                // и ожидаем завершения задачи,
                // инфраструктура извлечет результат выполнения задачи

                var length = await stream.ReadAsync(buffer, 0, buffer.Length);

                // остальной код будет выполнен как продолжение задачи

                Console.WriteLine(Encoding.UTF8.GetString(buffer));
            }
        }
    }
}

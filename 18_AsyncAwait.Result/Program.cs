using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncAwait.Result
{
    class Program
    {
        static void Main(string[] args)
        {
            // т.к. основной код приложения нельзя разделить на запуск операции, ожидание и вызов продолжения,
            // то к методу Main() нельзя применять модификатор async и использовать в нем await
            // но т.к. async-метод возвращает Task или Task<T> (или void, но это особый случай),
            // то мы можем получить задачу и работать с ней как обычно

            // запускаем операцию

            Console.WriteLine("ThreadId, in main: {0}", Thread.CurrentThread.ManagedThreadId);

            var myTask = CalcAsync();
            var myTask1 = CalcAsync();


            // ожидаем завершения задачи и выводим результат
            // в UI приложении такой код может привести к взаимоблокировке!

            Console.WriteLine("Result: {0}", myTask.Result);


            // ожидаем завершения задачи и выводим результат
            // в UI приложении такой код может привести к взаимоблокировке!

            Console.WriteLine("Result: {0}", myTask1.Result);


            Console.Read();
        }

        static async Task<int> CalcAsync()
        {
            Console.WriteLine("ThreadId, b/await: {0}", Thread.CurrentThread.ManagedThreadId);

            await Task.Delay(1000);

            // в консольных приложениях используется контекст синхронизации,
            // не привязывающийся к основному потоку, а использующий пул,
            // поэтому код после ожидания выполняется в параллельном потоке

            Console.WriteLine("ThreadId: a/await: {0}", Thread.CurrentThread.ManagedThreadId);

            return 42;
        }
    }
}

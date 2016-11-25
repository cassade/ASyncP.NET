using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncAwait.Awaitless
{
    class Program
    {
        static void Main(string[] args)
        {
            // общий подход к реализации обработчиков событий предусматривает отсутствие возвращаемого значения,
            // специально для этого в модель async/await была добавлена поддержка async void методов

            AppDomain.CurrentDomain.UnhandledException += async (s, e) =>
                {
                    using (var writer = new StreamWriter(Assembly.GetExecutingAssembly().Location + ".txt", false))
                    {
                        await writer.WriteAsync(e.ExceptionObject.ToString());
                    }
                };

            RunAsyncVoid();
            
            Console.Read();
        }

        static async Task RunAsyncVoid()
        {
            // по сравнению с async Task/Task<T> методами:

            // 1. другая семантика обработки ошибок:
            // для async void методов генерируется особый код без возврата вызывающему коду экземпляра задачи,
            // поэтому вызывающий код не может перехватить выбрасываемые таким методом исключения

            // 2. другая семантика композиции:
            // await использовать нельзя, также как Wait(), WaitAll() и т.д., 
            // таким образом при использовании async void сложнее определить,
            // когда именно метод закончит работу

            // 3. другая семантика вызова:
            // определить, что void метод асинхронный можно только
            // по исходному коду/документации (если она существует)

            try
            {
                ThrowException();
            }
            catch (Exception ex)
            {
                // этот код не будет выполнен

                Console.WriteLine("{0} : {1}", ex.GetType().Name, ex.Message);
            }

            Console.Read();
        }

        static async void ThrowException()
        {
            // исключения, выброшенные async void методом, генерируются 
            // непосредственно в том контексте синхронизации, в котором выполняется метод,
            // в данном случае исключение будет выброшено в основном потоке приложения
            // и мы получим Unhandled Exception

            throw new InvalidOperationException("Foreground");
        }
    }
}

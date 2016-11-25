using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tasks.CompletionSource
{
    class Program
    {
        static void Main(string[] args)
        {
            // для внешнего кода Classic Async Programming Model
            // превращается в Task Model

            var task = TaskApi.GetAsync();

            try
            {
                Console.WriteLine("{0} bytes read", task.Result);
            }
            catch (AggregateException ex)
            {
                Console.WriteLine(ex.InnerException.Message);
            }

            Console.Read();
        }
    }

    static class TaskApi
    {
        public static Task<int> GetAsync()
        {
            var completionSource = new TaskCompletionSource<int>();

            // будем читать содержимое конфигурационного файла

            var stream = new FileStream(Assembly.GetExecutingAssembly().Location + ".config", FileMode.Open);
            var buffer = new Byte[stream.Length];
            
            // запускем чтение асинхронно

            stream.BeginRead(buffer, 0, buffer.Length, ar =>
                {
                    // имитация долгого чтения

                    Thread.Sleep(1000);

                    // получаем количество считанных байт и устанавливаем результат задачи,
                    // при этом обрабатываем ошибки и переводим задачу в состояние Faulted при их возникновении,
                    // а также учитываем, что задача может быть отменена

                    lock (completionSource)
                    {
                        try
                        {
                            if (!completionSource.Task.IsCompleted && !completionSource.Task.IsCanceled)
                            {
                                completionSource.SetResult(stream.EndRead(ar));
                            }
                        }
                        catch (Exception ex)
                        {
                            completionSource.SetException(ex);
                        }
                    }

                }, null);

            // определяем политику отмены операции чтения:
            // отменяем, если чтение длится дольше секунды

            Action cancelAction = () =>
                {
                    Thread.Sleep(1000);

                    lock (completionSource)
                    {
                        if (!completionSource.Task.IsCompleted && !completionSource.Task.IsFaulted)
                        {
                            completionSource.SetCanceled();
                        }
                    }
                };

            // проверку отмены также запускаем асинхронно

            cancelAction.BeginInvoke(r => cancelAction.EndInvoke(r), null);

            // немедленно возвращаем задачу внешнему коду

            return completionSource.Task;
        }
    }
}

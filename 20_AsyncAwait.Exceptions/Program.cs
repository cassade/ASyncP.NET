using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncAwait.Exceptions
{
    class Program
    {
        static void Main(string[] args)
        {
            // к точке входа в приложение нельзя применять модификатор async,
            // следовательно, в Main() нельзя использовать await,
            // такой awaitless вызов async метода приведет к инкапсуляции результата/ошибок в объекте Task,
            // который нигде не используется (нет кода, сгенерированного для оператора await) и, соответственно, к потере этой информации,
            // хотя в консольном приложении можно использовать обычную обработку Task:

            try
            {
                ThrowAsync()/*.Wait()*/;
            }
            catch (Exception ex)
            {
                // этот код будет выполнен только 
                // если используется явное ожидание завершения задачи

                Console.WriteLine("{0} : {1}", ex.GetType().Name, ex.Message);
            }

            Console.Read();
        }

        static async Task ThrowAsync()
        {
            // запускаем и ожидаем окончание выполнения операции,
            // инфраструктура обработает задачу, 
            // извлечет из неё информацию об исключении и выбросит его снова так, 
            // чтобы его мог перехватить код продолжения,
            // при этом в отличие от обработки исключений в методе Task.Wait()
            // выбрасывается оргинальное исключение, а не AggregeteException

            throw new InvalidOperationException();

            try
            {
                await Task.Factory.StartNew(() =>
                    {
                        Thread.Sleep(1000);
                        throw new InvalidOperationException();
                    });
            }
            catch (Exception e)
            {
                Console.WriteLine("{0} : {1}", e.GetType().Name, e.Message);

                // заново выбрасываем исключение для демонстрации
                // особенностей awaitless вызова

                throw;
            }
        }
    }
}

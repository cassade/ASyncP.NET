using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace EventBased
{
	class Program
	{
		static void Main(string[] args)
		{
			var client = new WebClient();

            // запускаем загрузку

            client.DownloadStringAsync(new Uri(" http://basicdata.ru/api/json/fias/addrobj/0c5b2444-70a0-4932-980c-b4dc0d3f02b5/"));

            // определяем обработчик результата

            client.DownloadStringCompleted += (s, e) =>
                {
                    if (e.Error != null)
                    {
                        Console.WriteLine(e.Error.Message);
                    }
                    else
                    {
                        dynamic result = JsonConvert.DeserializeObject(e.Result);
                        foreach(var item in result.data)
                        {
                            Console.WriteLine("{0}, {1}", item.offname, item.shortname);
                        }
                    }
                };

            // в основном потоке выполняем какую-то полезную работу

            Thread.Sleep(1000);

            Console.WriteLine("Main completed");

            Console.Read();
		}
	}
}

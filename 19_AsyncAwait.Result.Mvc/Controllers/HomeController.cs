using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace AsyncAwait.Result.Mvc.Controllers
{
    public class HomeController : Controller
    {
        public async Task<string> CalculateAsync()
        {
            await Task.Delay(1000);

            // код после await в выполняется в контексте синхронизации вызывающего потока,
            // однако он уже используется для ожидания задачи,
            // которую вернул текущий метод

            return "Async";
        }

        public ActionResult Index()
        {
            var task = CalculateAsync();

            // обращение к Result или вызов Wait() блокирует текущий поток

            ViewBag.Value = task.Result;

            return View();
        }

        #region bubble async

        public async Task<ActionResult> Async()
        {
            // позволяем модели async/await распространиться по коду,
            // инфраструктура ASP.NET MVC знает, как выполнять async сontroller actions

            ViewBag.Value = await CalculateAsync();

            return View("Index");
        }

        #endregion

        #region configure await

        public async Task<string> CalculateAsyncSafe()
        {
            // best practice:
            // для всех асинхронных вызовов, которые не требуют взаимодействия с GUI потоком,
            // нужно вызывать ConfigureAwait(false)

            await Task.Delay(1000)
                .ConfigureAwait(continueOnCapturedContext: false);

            // код после await выполняется в контесте синхронизации пула потоков

            return "ConfigureAwait";
        }

        public ActionResult ConfigureAwait()
        {
            ViewBag.Value = CalculateAsyncSafe().Result;

            return View("Index");
        }

        #endregion

        #region task model

        public Task<string> CalculateAsyncTask()
        {
            return Task.Run(() =>
            {
                Thread.Sleep(1000);
                return "TaskModel";
            });
        }

        public ActionResult TaskModel()
        {
            ViewBag.Value = CalculateAsyncTask().Result;

            return View("Index");
        }

        #endregion
	}
}
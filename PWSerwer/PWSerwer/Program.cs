using System;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Hosting;
using Owin;
using PWSerwer.Dispatcher;

namespace PWSerwer
{
    class Program
    {
        static void Main(string[] args)
        {
            DispatcherProcessor processor = DispatcherProcessor.Instance;
            var url = "http://localhost:8080/";
            using (WebApp.Start<Startup>(url))
            {
                Console.WriteLine($"Serwer działa na: {url}");
                Console.ReadLine();
            }
        }
    }

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(CorsOptions.AllowAll);
            app.MapSignalR("/pw", new HubConfiguration());
        }
    }
}

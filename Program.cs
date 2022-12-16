using Blog_MVC.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace Blog_MVC
{
    public class Program
    {
        public static async Task Main(string[] args)
        {


            var host = CreateHostBuilder(args).Build();

            // Pull out my registered service
            var dataService = host.Services.CreateScope().ServiceProvider.GetRequiredService<DataService>();

            await dataService.ManageDataAsync();


            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}

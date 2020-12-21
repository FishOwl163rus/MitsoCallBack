using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace MitsoCallBack
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseKestrel();
                    webBuilder.UseIISIntegration();
                    webBuilder.UseUrls("http://91.206.14.12:80");
                    webBuilder.UseStartup<Startup>();
                });
    }
}
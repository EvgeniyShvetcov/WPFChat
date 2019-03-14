using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace ChatServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            var deployingUrl = "http://localhost:5000";
            if (args.Length > 0 && !string.IsNullOrWhiteSpace(args[0])) deployingUrl = args[0];
            return WebHost.CreateDefaultBuilder(args)
                .UseUrls(deployingUrl)
                .UseStartup<Startup>();
        }
    }

}

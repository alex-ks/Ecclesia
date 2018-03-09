using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Ecclesia.LocalExecutor.Endpoint
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var hostingConfig = new ConfigurationBuilder()
                .AddJsonFile("hosting.json")
                .Build();
            BuildWebHost(args, hostingConfig).Run();
        }
        
        public static IWebHost BuildWebHost(string[] args, IConfiguration hostingConf) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseUrls(hostingConf["server.url"])
                .Build();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.IO;


namespace Ecclesia.LocalExecutor.Endpoint
{
    public class MethodManager : IMethodManager
    {
        public static IConfiguration Configuration { get; set; }

        public string GetMethodSource(string methodName)
        {
            var ConfigurationMethods = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("methodsConf.json");

            Configuration = ConfigurationMethods.Build();

            return File.ReadAllText(Configuration[methodName].ToString());
        }
    }
}

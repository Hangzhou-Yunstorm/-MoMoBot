using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace MoMoBot.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseUrls("http://+:4567")
                .UseStartup<Startup>()
                .UseSerilog((webHostBuilderContext, loggerConfiguration) =>
                {
                    var connection = webHostBuilderContext.Configuration.GetConnectionString("LogConnection");
                    loggerConfiguration
                        .Enrich.FromLogContext()
                        .WriteTo.Console(LogEventLevel.Verbose)
                        .WriteTo.PostgreSQL(connection, "system_logs", restrictedToMinimumLevel: LogEventLevel.Warning, needAutoCreateTable: true);
                });
    }
}

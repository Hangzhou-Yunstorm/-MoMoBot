using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace MoMoBot.ElasticSearch.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddMoMoBotSearch(this IServiceCollection services, string connection = "http://localhost:9200")
        {
            services.AddSingleton(p => new MoMoBotSearch(connection));
        }
    }
}

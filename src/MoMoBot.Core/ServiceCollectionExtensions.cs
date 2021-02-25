using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace MoMoBot.Core
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMoMoBot<TBot>(this IServiceCollection services) where TBot : class, IBot
        {
            services.AddTransient<IBot, TBot>();
            return services;
        }
    }
}

using Microsoft.Extensions.DependencyInjection;

namespace MoMoBot.ML.Extensions
{
    public static class MoMoBotMLServiceCollectionExtensions
    {
        public static IServiceCollection AddMoMoBotMLServices(this IServiceCollection services, string modelPath, string fileName="intent_model.zip")
        {
            services.AddScoped(_ => new MoMoBotML(modelPath, fileName));
            return services;
        }
    }
}

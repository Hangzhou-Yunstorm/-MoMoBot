using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using Microsoft.Extensions.Caching.Redis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MoMoBot.Infrastructure.Cache;
using MoMoBot.Infrastructure.Handlers;
using MoMoBot.Infrastructure.Luis;
using MoMoBot.Infrastructure.Luis.Enums;
using MoMoBot.Infrastructure.ViewModels;
using Polly;
using Polly.Extensions.Http;

namespace MoMoBot.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        //public static IServiceCollection AddCustomServices(this IServiceCollection services)
        //{
        //    services.AddHttpClient<IAnswerService, AnswerService>(client =>
        //    {
        //        client.Timeout = TimeSpan.FromMinutes(1);
        //    })
        //        .AddPolicyHandler(GetRetryPolicy())
        //        .AddPolicyHandler(GetCircuitBreakerPolicy());

        //    services.AddHttpClient<DingDingApprovalService>(client =>
        //    {
        //        client.Timeout = TimeSpan.FromMinutes(1);
        //    })
        //        .AddPolicyHandler(GetRetryPolicy())
        //        .AddPolicyHandler(GetCircuitBreakerPolicy());

        //    services.AddHttpClient<ILuisService, LuisService>(client =>
        //    {
        //        client.Timeout = TimeSpan.FromMinutes(1);
        //    })
        //        .AddPolicyHandler(GetRetryPolicy())
        //        .AddPolicyHandler(GetCircuitBreakerPolicy());

        //    return services;
        //}

        //private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        //{
        //    return HttpPolicyExtensions
        //      .HandleTransientHttpError()
        //      .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
        //      .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        //}

        //private static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
        //{
        //    return HttpPolicyExtensions
        //        .HandleTransientHttpError()
        //        .CircuitBreakerAsync(3, TimeSpan.FromSeconds(30));
        //}

        private const string PoliciesConfigurationSectionName = "Policies";
        private const string HttpClientConfigurationSectionName = "HttpClientOptions";

        public static IServiceCollection AddPolicyHttpClient<TClient, TImplementation, TClientOptions>(
            this IServiceCollection services,
            IConfiguration configuration,
            string configurationSectionName = HttpClientConfigurationSectionName
            )
            where TClient : class
            where TImplementation : class, TClient
            where TClientOptions : HttpClientOptions, new() =>
            services
                .Configure<TClientOptions>(configuration.GetSection(configurationSectionName))
                .AddTransient<CorrelationIdDelegatingHandler>()
                .AddTransient<UserAgentDelegatingHandler>()
                .AddHttpClient<TClient, TImplementation>()
                .ConfigureHttpClient(
                    (sp, options) =>
                    {
                        var httpClientOptions = sp
                            .GetRequiredService<IOptions<TClientOptions>>()
                            .Value;
                        options.BaseAddress = httpClientOptions.BaseAddress;
                        options.Timeout = httpClientOptions.Timeout;
                    })
                .ConfigurePrimaryHttpMessageHandler(x => new DefaultHttpClientHandler())
                .AddPolicyHandlerFromRegistry(PolicyName.HttpRetry)
                .AddPolicyHandlerFromRegistry(PolicyName.HttpCircuitBreaker)
                .AddHttpMessageHandler<CorrelationIdDelegatingHandler>()
                .AddHttpMessageHandler<UserAgentDelegatingHandler>()
                .Services;

        public static IServiceCollection AddPolicyHttpClient<TClient, TImplementation>(
            this IServiceCollection services
            )
            where TClient : class
            where TImplementation : class, TClient =>
            services.AddTransient<CorrelationIdDelegatingHandler>()
                .AddTransient<UserAgentDelegatingHandler>()
                .AddHttpClient<TClient, TImplementation>()
                .ConfigureHttpClient(
                    (sp, options) =>
                    {
                        var httpClientOptions = GetDefaultHttpClientOptions();
                        options.Timeout = httpClientOptions.Timeout;
                    })
                .ConfigurePrimaryHttpMessageHandler(x => new DefaultHttpClientHandler())
                .AddPolicyHandlerFromRegistry(PolicyName.HttpRetry)
                .AddPolicyHandlerFromRegistry(PolicyName.HttpCircuitBreaker)
                .AddHttpMessageHandler<CorrelationIdDelegatingHandler>()
                .AddHttpMessageHandler<UserAgentDelegatingHandler>()
                .Services;

        public static IServiceCollection AddPolicyHttpClient<TImplementation>(
        this IServiceCollection services)
            where TImplementation : class =>
        services
            .AddTransient<CorrelationIdDelegatingHandler>()
            .AddTransient<UserAgentDelegatingHandler>()
            .AddHttpClient<TImplementation>()
            .ConfigureHttpClient(
                (sp, options) =>
                {
                    var httpClientOptions = GetDefaultHttpClientOptions();
                    options.Timeout = httpClientOptions.Timeout;
                })
            .ConfigurePrimaryHttpMessageHandler(x => new DefaultHttpClientHandler())
            .AddPolicyHandlerFromRegistry(PolicyName.HttpRetry)
            .AddPolicyHandlerFromRegistry(PolicyName.HttpCircuitBreaker)
            .AddHttpMessageHandler<CorrelationIdDelegatingHandler>()
            .AddHttpMessageHandler<UserAgentDelegatingHandler>()
            .Services;

        public static IServiceCollection AddPolicies(
            this IServiceCollection services,
            IConfiguration configuration,
            string configurationSectionName = PoliciesConfigurationSectionName)
        {
            var section = configuration.GetSection(configurationSectionName);
            services.Configure<PolicyOptions>(configuration);
            var policyOptions = section.Get<PolicyOptions>();

            var policyRegistry = services.AddPolicyRegistry();
            policyRegistry.Add(
                PolicyName.HttpRetry,
                HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .WaitAndRetryAsync(
                        policyOptions.HttpRetry.Count,
                        retryAttempt => TimeSpan.FromSeconds(Math.Pow(policyOptions.HttpRetry.BackoffPower, retryAttempt))));
            policyRegistry.Add(
                PolicyName.HttpCircuitBreaker,
                HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .CircuitBreakerAsync(
                        handledEventsAllowedBeforeBreaking: policyOptions.HttpCircuitBreaker.ExceptionsAllowedBeforeBreaking,
                        durationOfBreak: policyOptions.HttpCircuitBreaker.DurationOfBreak));

            return services;
        }

        private static HttpClientOptions GetDefaultHttpClientOptions() => new HttpClientOptions
        {
            Timeout = TimeSpan.FromMinutes(1)
        };
    }

    public static class PolicyName
    {
        public const string HttpCircuitBreaker = nameof(HttpCircuitBreaker);
        public const string HttpRetry = nameof(HttpRetry);
    }

    public class PolicyOptions
    {
        public CircuitBreakerPolicyOptions HttpCircuitBreaker { get; set; }
        public RetryPolicyOptions HttpRetry { get; set; }
    }

    public class CircuitBreakerPolicyOptions
    {
        public TimeSpan DurationOfBreak { get; set; } = TimeSpan.FromSeconds(30);
        public int ExceptionsAllowedBeforeBreaking { get; set; } = 12;
    }

    public class RetryPolicyOptions
    {
        public int Count { get; set; } = 3;
        public int BackoffPower { get; set; } = 2;
    }

    public class DefaultHttpClientHandler : HttpClientHandler
    {
        public DefaultHttpClientHandler() =>
            this.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
    }

    public class HttpClientOptions
    {
        public Uri BaseAddress { get; set; }

        public TimeSpan Timeout { get; set; }
    }
}

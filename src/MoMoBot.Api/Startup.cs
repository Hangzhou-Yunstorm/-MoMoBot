using System;
using System.IO;
using System.Text;
using CorrelationId;
using EasyCaching.Core;
using EasyCaching.Redis;
using IdentityModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Redis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MoMoBot.Api.Hubs;
using MoMoBot.Infrastructure;
using MoMoBot.Infrastructure.Cache;
using MoMoBot.Infrastructure.Database;
using MoMoBot.Infrastructure.DTalk;
using MoMoBot.Infrastructure.DTalk.Models;
using MoMoBot.Infrastructure.ViewModels;
using MoMoBot.Service.Abstractions;
using MoMoBot.Service.Implements;
using MoMoBot.Infrastructure.Extensions;
using MoMoBot.Api.BackgroundTasks;
using MoMoBot.Infrastructure.Models;
using MoMoBot.Core;
using MoMoBot.Api.Dialogs;
using System.Linq;
using reCAPTCHA.AspNetCore;
using Hangfire;
using Hangfire.PostgreSql;
using MoMoBot.Infrastructure.Middlewares;
using MoMoBot.ML.Extensions;
using Microsoft.Azure.CognitiveServices.Language.LUIS.Authoring;
using Microsoft.Azure.CognitiveServices.Language.LUIS.Runtime;
using ApiKeyServiceClientCredentials = Microsoft.Azure.CognitiveServices.Language.LUIS.Authoring.ApiKeyServiceClientCredentials;
using MoMoBot.Service.Map;
using MoMoBot.ElasticSearch.Extensions;

namespace MoMoBot.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }

        private readonly IHostingEnvironment _env;
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.Configure<RedisCacheOptions>(Configuration.GetSection("RedisOptions"));
            services.Configure<DTalkConfig>(Configuration.GetSection("DTalkConfig"));

            var connection = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<MoMoDbContext>(options =>
            {
                options.UseNpgsql(connection, builder =>
                {
                    builder.EnableRetryOnFailure(3);
                    var assembly = typeof(MoMoDbContext).Assembly.GetName().Name;
                    builder.MigrationsAssembly(assembly);
                });
            });
            services.AddDbContext<LogDbContext>(options =>
             {
                 options.UseNpgsql(Configuration.GetConnectionString("LogConnection"));
             });

            // 添加identity身份验证
            services.AddIdentity<MoMoBotUser, IdentityRole>()
                .AddEntityFrameworkStores<MoMoDbContext>()
                .AddDefaultTokenProviders()
                .AddErrorDescriber<MoMoBotIdentityErrorDescriber>();

            // Add Hangfire services.
            services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UsePostgreSqlStorage(Configuration.GetConnectionString("HangfireConnection"))
            );

            // Add the processing server as IHostedService
            services.AddHangfireServer();

            #region 服务注入
            services.AddSingleton<IRedisCacheService>(p =>
                {
                    var options = p.GetRequiredService<IOptions<RedisCacheOptions>>()?.Value
                        ?? throw new ArgumentNullException(nameof(RedisCacheOptions));
                    return new RedisCacheService(options);
                });

            services.AddLUIS(Configuration);

            services.AddScoped<IUnknownService, UnknownService>()
                    .AddScoped<IParameterService, ParamenterService>()
                    .AddTransient<ILoginService<IdentityUser>, EFLoginService>()
                    .AddScoped<IFeedbackService, FeedbackService>()
                    .AddScoped<IServiceRecordService, ServiceRecordService>()
                    .AddScoped<IModularService, ModularService>()
                    .AddScoped<IDepartmentService, DepartmentService>()
                    .AddScoped<IPermissionService, PermissionService>()
                    .AddScoped<IHotIntentService, HotIntentService>()
                    .AddScoped<IChatService, ChatService>()
                    .AddScoped<ArtificialServices>()
                    .AddScoped<DingTalkHelper>()
                    .AddScoped<IUserService, UserService>()
                    .AddScoped<IAppSettings, AppSettings>()
                    .AddScoped<ISystemLogReader, SystemLogReader>()
                    .AddScoped<IKnowledgeService, KnowledgeService>();

            services.AddCorrelationId();
            services.AddPolicies(Configuration)
                .AddPolicyHttpClient<IAnswerService, AnswerService>()
                .AddPolicyHttpClient<ILuisService, LuisService>()
                .AddPolicyHttpClient<DingDingApprovalService>()
                .AddPolicyHttpClient<IVoiceService, VoiceService>()
                .AddPolicyHttpClient<HttpRequestHelper>();

            var modelPath = Path.Combine(_env.ContentRootPath, "MLModels");
            services.AddMoMoBotMLServices(modelPath);

            // services.AddHostedService<StatisticsMatchIntentCountTask>();

            services.AddScoped<KnowledgeMapContext>();
            services.AddMoMoBotSearch();
            #endregion

            #region Microsoft Bot Builder 的实现
            IStorage dataStore = new MemoryStorage();
            services.AddSingleton(dataStore);
            var conversationState = new ConversationState(dataStore);
            services.AddSingleton(conversationState)
                    // The BotStateSet enables read() and write() in parallel on multiple BotState instances.
                    .AddSingleton(new BotStateSet(conversationState));
            services.AddSingleton(sp => new DomainPropertyAccessors(conversationState));
            services.AddTransient<IDialogFactory, DialogFactory>();
            services.AddMoMoBot<MoMoBot>();
            #endregion


            services.AddCors();
            services.AddSignalR();

            // Google reCaptcha
            services.AddGoogleRecaptcha(Configuration.GetSection("RecaptchaSettings"));

            services.AddMvc(options =>
            {
                //var policy = new AuthorizationPolicyBuilder()
                //    .RequireAuthenticatedUser()
                //    .Build();
                //options.Filters.Add(new AuthorizeFilter(policy));
            })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddEasyCaching(options =>
            {
                options.UseRedis(Configuration, "MoMoBotDEV");
            });

            var tokenValidationParameters = new TokenValidationParameters
            {
                NameClaimType = JwtClaimTypes.Name,
                RoleClaimType = JwtClaimTypes.Role,

                ValidIssuer = "yunstorm",
                ValidAudience = "momobot.api",
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Constants.Secret))
            };
            services.AddSingleton<IMemoryCache>(factory =>
            {
                var cache = new MemoryCache(new MemoryCacheOptions());
                return cache;
            });
            services.AddSingleton(p => tokenValidationParameters);
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o =>
            {
                o.TokenValidationParameters = tokenValidationParameters;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,
            IHostingEnvironment env,
            IApplicationLifetime lifetime)
        {
            app.UseCorrelationId();

            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}

            app.UseMoMoExceptionHandler();

            app.UseCors(builder =>
            {
                var origins = new[]
                {
                    "http://localhost:8000",
                    "http://10.0.1.46:8000",
                    "http://localhost:3000",
                    "http://10.0.1.46:3000",
                    "http://13.70.26.233:3000",
                    "http://13.70.26.233:8000",
                    "http://52.184.99.37",
                    "Http://52.184.99.37:7777",
                    "https://momo.yunstorm.com"
                };
                builder
                    .WithOrigins(origins)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
            app.UseSignalR(builder =>
            {
                builder.MapHub<ArtificialServicesHub>("/humanservice");
            });
            app.UseStaticFiles(new StaticFileOptions
            {
                //OnPrepareResponse = context => {
                //    context.Context.Response.Headers.Remove("Content-Length");
                //}
                RequestPath = "/files",
                FileProvider = new PhysicalFileProvider(Path.Combine(_env.ContentRootPath, "upload"))
            });

            app.UseHangfireDashboard(pathMatch: "/_hangfire");

            app.UseAuthentication();

            app.UseMoMoBot();

            app.UseMvc(routes =>
            {
                routes.MapRoute("api", "api/[controller]");
            });
            lifetime.ApplicationStopping.Register(async () =>
            {
                using (var scope = app.ApplicationServices.CreateScope())
                {
                    var artificial = scope.ServiceProvider.GetService<ArtificialServices>();
                    await artificial.ShutdownAsync();
                }
            });
            lifetime.ApplicationStarted.Register(() =>
            {
                // 热门意图统计任务，每半小时触发
                RecurringJob.AddOrUpdate<StatisticsMatchIntentCountTask>(x => x.ExecuteAsync(), "0 0/30 * * * ? ");
                // 小摩摩ML训练任务，每日凌晨2点触发
                RecurringJob.AddOrUpdate<MoMoBotMLTrainingTask>("momobot_ml_training", x => x.Train(), "0 0 2 1/1 * ? ", TimeZoneInfo.Local);
            });
        }

        /// <summary>
        /// CORS
        /// </summary>
        /// <param name="origin"></param>
        /// <returns></returns>
        private bool IsOriginAllowed(string origin)
        {
            var origins = new[]
                {
                    "http://localhost:8000",
                    "http://10.0.1.46:8000",
                    "http://localhost:3000",
                    "http://10.0.1.46:3000",
                    "http://52.184.99.37",
                    "Http://52.184.99.37:7777"
                };
            return origins.Any(o => o == origin);
        }
    }

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddLUIS(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<LuisSettings>(configuration.GetSection("Luis"));
            var settings = services.BuildServiceProvider().GetRequiredService<IOptions<LuisSettings>>()?.Value
                        ?? throw new ArgumentNullException(nameof(LuisSettings));
            services.AddScoped<ILUISAuthoringClient>(_ =>
            {
                var credentials = new ApiKeyServiceClientCredentials(settings.AuthoringKey);

                var client = new LUISAuthoringClient(credentials);
                client.Endpoint = settings.AuthoringEndpoint;
                return client;
            })
                .AddScoped<ILUISRuntimeClient>(_ =>
                {
                    var credentials = new ApiKeyServiceClientCredentials(settings.SubscriptionKey);

                    var client = new LUISRuntimeClient(credentials);
                    client.Endpoint = settings.RuntimeEndpoint;
                    return client;
                });

            return services;
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using MoMoBot.Core.Schema;
using Newtonsoft.Json;

namespace MoMoBot.Core
{
    public class MessageHandler
    {
        private const string InvokeReponseKey = "MessageHandler.InvokeResponse";
        public static readonly JsonSerializer BotMessageSerializer = JsonSerializer.Create();
        public delegate Task BotCallbackHandler(TurnContext turnContext, CancellationToken cancellationToken);

        private async Task<InvokeResponse> ProcessMessageRequestion(HttpRequest request, BotCallbackHandler botCallbackHandler, CancellationToken cancellationToken)
        {
            var requestBody = request.Body;

            if (requestBody.CanSeek)
            {
                requestBody.Position = 0;
            }
            var activity = default(Activity);
            using (var reader = new StreamReader(requestBody, Encoding.UTF8, false, 1024, true))
            {
                using (var bodyReader = new JsonTextReader(reader))
                {
                    activity = BotMessageSerializer.Deserialize<Activity>(bodyReader);
                }
            }

            var invokeResponse = await ProcessActivityAsync(
                 request.Headers["Authorization"],
                     activity,
                     botCallbackHandler,
                     cancellationToken
                     );

            return invokeResponse;
        }

        private async Task<InvokeResponse> ProcessActivityAsync(string authHeader, Activity activity, BotCallbackHandler callback, CancellationToken cancellationToken)
        {
            MoMoBotAssert.NotNull(activity);
            using (var context = new TurnContext(activity))
            {
                //context.TurnState.Add<IIdentity>(BotIdentityKey, identity);
                await callback?.Invoke(context, cancellationToken);

                if (activity.Type == ActivityTypes.Invoke)
                {
                    var activityInvokeResponse = context.TurnState.Get<Activity>(InvokeReponseKey);
                    if (activityInvokeResponse == null)
                    {
                        return new InvokeResponse { Status = (int)HttpStatusCode.NotImplemented };
                    }
                    else
                    {
                        return (InvokeResponse)activityInvokeResponse.Value;
                    }
                }
                return null;
            }
        }


        public async Task HandlerAsync(HttpContext httpContext)
        {
            var request = httpContext.Request;
            var response = httpContext.Response;
            if (request.Method != HttpMethods.Post)
            {
                response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                return;
            }
            if (request.ContentLength == 0)
            {
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                return;
            }
            if (!MediaTypeHeaderValue.TryParse(request.ContentType, out var mediaTypeHeaderValue)
                    ||
                mediaTypeHeaderValue.MediaType != "application/json")
            {
                response.StatusCode = (int)HttpStatusCode.NotAcceptable;
                return;
            }
            var requestServices = httpContext.RequestServices;
            var bot = requestServices.GetRequiredService<IBot>();

            var invokeResponse = await ProcessMessageRequestion(request, bot.OnTurnAsync, default);
            if (invokeResponse == null)
            {
                response.StatusCode = (int)HttpStatusCode.OK;
            }
            else
            {
                response.StatusCode = invokeResponse.Status;
                if (response.Body != null)
                {
                    response.ContentType = "application/json";
                    using (var writer = new StreamWriter(response.Body))
                    {
                        using (var jsonWriter = new JsonTextWriter(writer))
                        {
                            BotMessageSerializer.Serialize(jsonWriter, invokeResponse.Body);
                        }
                    }
                }
            }
        }
    }
}

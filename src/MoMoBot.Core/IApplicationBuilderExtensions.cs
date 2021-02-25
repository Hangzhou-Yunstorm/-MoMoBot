using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace MoMoBot.Core
{
    public static class IApplicationBuilderExtensions
    {
        public static void UseMoMoBot(this IApplicationBuilder app)
        {
            app.Map("/api/messages", builder => builder.Run(new MessageHandler().HandlerAsync));
        }
    }
}

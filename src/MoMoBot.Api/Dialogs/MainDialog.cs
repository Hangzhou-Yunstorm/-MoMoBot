using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MoMoBot.Core;
using MoMoBot.Core.Schema;
using MoMoBot.Service.Abstractions;

namespace MoMoBot.Api.Dialogs
{
    public class MainDialog : ComponentDialog
    {
        public const string Name = nameof(MainDialog);

        private readonly ILuisService _luisService;
        private readonly DomainPropertyAccessors _accessors;
        public MainDialog(DomainPropertyAccessors accessors, ILuisService luisService) : base(Name)
        {
            _luisService = luisService;
            _accessors = accessors;
        }

        protected override Task<DialogTurnResult> OnBeginDialogAsync(DialogContext innerDc, object options, CancellationToken cancellationToken = default(CancellationToken)) => OnContinueDialogAsync(innerDc, cancellationToken);


        protected override async Task<DialogTurnResult> OnContinueDialogAsync(DialogContext innerDc, CancellationToken cancellationToken = default)
        {
            var activity = innerDc.Context.Activity;
            var result = await innerDc.ContinueDialogAsync();
            switch (result.Status)
            {
                case DialogTurnStatus.Empty:
                    {
                        await RouteAsync(innerDc);
                        break;
                    }
                case DialogTurnStatus.Complete:
                    {
                        await CompleteAsync(innerDc);
                        await innerDc.EndDialogAsync();
                        break;
                    }
                default:
                    {
                        break;
                    }
            }

            return EndOfTurn;
        }

        private async Task RouteAsync(DialogContext innerDc)
        {
            await Task.CompletedTask;
        }

        private async Task CompleteAsync(DialogContext innerDc)
        {
            await Task.CompletedTask;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using MoMoBot.Service.Abstractions;

namespace MoMoBot.Api.Dialogs
{
    public class DialogFactory : IDialogFactory
    {
        private readonly ILuisService _luisService;
        private readonly DomainPropertyAccessors _accessors;
        public DialogFactory(ILuisService luisService, DomainPropertyAccessors accessors)
        {
            _accessors = accessors;
            _luisService = luisService;
        }

        public MainDialog MainDialog => new MainDialog(_accessors, _luisService);
    }

    public interface IDialogFactory
    {
        MainDialog MainDialog { get; }
    }
}

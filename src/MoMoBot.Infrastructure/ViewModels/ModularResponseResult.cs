using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoMoBot.Infrastructure.ViewModels
{
    public class ModularResponseResult
    {
        public string Title { get; set; }
        public string ID { get; set; }
        public string ListName { get; set; }
        public string SheetName { get; set; }
    }

    public class ModularResponseFinnalResult
    {
        public string TName { get; set; }
        public string TValue { get; set; }
    }
}

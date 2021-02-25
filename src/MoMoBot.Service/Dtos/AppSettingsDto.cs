using System;
using System.Collections.Generic;
using System.Text;

namespace MoMoBot.Service.Dtos
{
    [Serializable]
    public class AppSettingsDto
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace MoMoBot.Service.Dtos
{
    public class DialogFlowDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Key { get; set; }
        public DateTime CreationTime { get; set; }
        public string Remark { get; set; }
    }
}

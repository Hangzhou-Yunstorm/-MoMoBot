using System;
using System.Collections.Generic;
using System.Text;

namespace MoMoBot.Service.Dtos
{
    public class ServiceRecordsDto
    {
        public long Id { get; set; }
        public long ChatId { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public bool IsDone { get; set; }
        public DateTime EndOfServiceTime { get; set; }
        public DateTime RecordCompletionTime { get; set; }
        public int Score { get; set; }
        public string Remarks { get; set; }
    }
}

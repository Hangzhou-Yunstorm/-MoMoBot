using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoMoBot.Infrastructure.Models
{
    public class QAPermission
    {
        public long DepartmentId { get; set; }
        public Guid QAId { get; set; }

        public QandA QA { get; set; }
        public Department Department { get; set; }
    }
}

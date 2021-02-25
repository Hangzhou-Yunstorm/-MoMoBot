using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoMoBot.Infrastructure.Models
{
    public class ModularPermission
    {
        public long ModularId { get; set; }
        public long DepartmentId { get; set; }
        public virtual Modular Modular { get; set; }
        public virtual Department Department { get; set; }
    }
}

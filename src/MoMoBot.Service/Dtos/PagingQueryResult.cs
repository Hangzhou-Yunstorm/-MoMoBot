using System;
using System.Collections.Generic;
using System.Text;

namespace MoMoBot.Service.Dtos
{
    public class PagingQueryResult<T>
    {
        public int Total { get; set; }
        public IEnumerable<T> Data { get; set; }
    }
}

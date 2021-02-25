using MoMoBot.Service.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MoMoBot.Service.Abstractions
{
    public interface ISystemLogReader
    {
        Task<PagingQueryResult<Log>> GetLogsAsync(List<int> levels, DateTime? start, DateTime? end, int offset = 0, int limit = 20);

        /// <summary>
        /// 获取发生的错误数（ Error || Fatal）
        /// </summary>
        /// <returns></returns>
        Task<int> GetErrorCountAsync();
    }
}

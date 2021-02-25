using MoMoBot.Infrastructure.Models;
using MoMoBot.Service.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoMoBot.Service.Abstractions
{
    public interface IServiceRecordService
    {
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="serviceRecord"></param>
        /// <returns></returns>
        Task<ServiceRecord> AddAsync(ServiceRecord serviceRecord);
        /// <summary>
        /// 客户评分
        /// </summary>
        /// <param name="id"></param>
        /// <param name="score"></param>
        /// <returns></returns>
        Task ScoreAsync(long id, int score);
        /// <summary>
        /// 查找记录
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ServiceRecord> FindTask(string userId, long id);
        /// <summary>
        /// 通过条件来筛选数据
        /// </summary>
        /// <param name="search">关键字</param>
        /// <param name="lowScore">是否筛选低分（小于3分的为低分）</param>
        /// <param name="userId">当前登录的用户</param>
        /// <param name="offset">从哪一行开始取</param>
        /// <param name="limit">取多少行</param>
        /// <param name="done">筛选归档了的</param>
        /// <returns></returns>
        Task<PaginationViewModel> GetPaginationAsync(string search, bool? lowScore, string userId, int offset, int limit, bool? done);
        /// <summary>
        /// 已归档
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        Task TaskCompletion(ServiceRecord record);
        /// <summary>
        /// 获取已归档的数据
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<List<ServiceRecord>> GetIsDone(string userId);
        /// <summary>
        /// 生成归档记录
        /// </summary>
        /// <param name="serverId"></param>
        /// <param name="chatId"></param>
        /// <returns></returns>
        Task<ServiceRecord> GenerateServiceRecordAsync(string serverId, long chatId);
        /// <summary>
        /// 获取对应客户的服务记录
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="identityId"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        Task<IEnumerable<ServiceRecordsDto>> GetServiceRecordsAsync(string owner, string identityId, int limit = 10);
        /// <summary>
        /// 删除并且保存（软删除）
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteAndSaveChangeAsync(string userId, long id);
    }
}

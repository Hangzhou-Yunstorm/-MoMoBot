using MoMoBot.Infrastructure.Models;
using MoMoBot.Infrastructure.Models.KnowledgeMapModel;
using MoMoBot.Service.Dtos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MoMoBot.Service.Abstractions
{
    public interface IAnswerService
    {
        #region Base Methods
        /// <summary>
        /// 获得所有答案
        /// </summary>
        /// <returns></returns>
        Task<List<QandA>> GetAllQandAsAsync();

        Task<int> GetTotalCountAsync();

        /// <summary>
        /// 分页获得答案
        /// </summary>
        /// <param name="startIndex">跳过多少条数据</param>
        /// <param name="endIndex">取多少条数据</param>
        /// <returns></returns>
        Task<List<QandA>> GetPagingQandAsAsync(int skipNum, int count);

        /// <summary>
        /// 根据关键字搜索答案
        /// </summary>
        /// <param name="Keyword"></param>
        /// <returns></returns>
        Task<List<QandA>> SearchAsync(string Keyword, int limit, int offset);

        /// <summary>
        /// 根据关键字来获取总记录
        /// </summary>
        /// <param name="Keyword"></param>
        /// <returns></returns>
        Task<int> GetTotalCountAsync(string Keyword);

        /// <summary>
        /// 根据意图获得答案
        /// </summary>
        /// <param name="intent"></param>
        /// <returns></returns>
        Task<DialogDto> GetAnswerByIntent(string intent, List<long> departIds, string conversationId = null);

        /// <summary>
        /// 根据Id来获得答案
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<QandA> GetAsync(Guid id);

        /// <summary>
        /// 根据id来删除
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        Task<bool> RemoveById(Guid Id);

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="qandA"></param>
        /// <returns></returns>
        Task<QandA> AddAsync(QandA qandA);

        /// <summary>
        /// 添加多个
        /// </summary>
        /// <param name="qandAs"></param>
        /// <returns></returns>
        Task AddRangeAsync(List<QandA> qandAs);

        Task UpdateAsync(QandA qandA);

        Task UpdateAllProperties(QandA qandA);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<bool> SaveChangesAsync();

        Task<bool> Exist(string intent, string answer);
        #endregion

        Task<string> GetJsonDataAsync();
        Task AddParameters(List<AnswerQueries> answerQueries);

        Task<QandA> GetParameterSettings(Guid answerId);

        Task RemoveParameters(Guid answerId);
        Task ClearCache();

        void SetCurrentDingDingUser(DingDingUser user);
        IEnumerable<KnowledgeMap> GetDialogFlowsQuery(Func<KnowledgeMap, bool> where);
    }
}

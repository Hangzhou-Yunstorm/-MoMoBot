using MoMoBot.Infrastructure.Models;
using MoMoBot.Infrastructure.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MoMoBot.Service.Abstractions
{
    public interface IFeedbackService
    {

        /// <summary>
        /// 分页获得答案
        /// </summary>
        /// <param name="startIndex">跳过多少条数据</param>
        /// <param name="endIndex">取多少条数据</param>
        /// <returns></returns>
        Task<List<FeedbackInfo>> GetPagingFeedbackAsync(int skipNum, int count);


        /// <summary>
        /// 根据反馈来过滤数据
        /// </summary>
        /// <returns></returns>
        Task<List<FeedbackInfo>> FilterByFeedbackAsync(int keyword, int skipNum, int count);
        /// <summary>
        /// 获得数据总条数
        /// </summary>
        /// <returns></returns>
        Task<int> GetTotalCountAsync(int keyword);

        /// <summary>
        /// 添加一条数据
        /// </summary>
        /// <param name="feedbackInfo"></param>
        /// <returns></returns>
        Task<FeedbackInfo> AddAsync(FeedbackInfo feedbackInfo);

        /// <summary>
        /// 软删除一条数据
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        Task<bool> RemovByIdAsync(Guid Id);

        /// <summary>
        /// 获得统计反馈数据图表
        /// </summary>
        /// <returns></returns>
        Task<FeedbackConsolePieModel> GetFeedbackPieAsync();

        /// <summary>
        /// 反馈统计条形图
        /// </summary>
        /// <param name="result"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        Task<IList<FeedbackStatisticsResult>> FeedbackStatistics(int result, int limit = 10);
        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        Task<MemoryStream> Export(int type);
    }
}

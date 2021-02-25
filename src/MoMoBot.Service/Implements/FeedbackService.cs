using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Extensions;
using Microsoft.Extensions.Logging;
using MoMoBot.Infrastructure.Database;
using MoMoBot.Infrastructure.Models;
using MoMoBot.Infrastructure.ViewModels;
using MoMoBot.Service.Abstractions;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoMoBot.Service.Implements
{
    public class FeedbackService : IFeedbackService
    {
        private readonly MoMoDbContext _context;
        private readonly ILogger<FeedbackService> _logger;
        public FeedbackService(MoMoDbContext context,
            ILogger<FeedbackService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<FeedbackInfo> AddAsync(FeedbackInfo feedbackInfo)
        {
            var entry = await _context.AddAsync(feedbackInfo);
            await _context.SaveChangesAsync();
            return entry.Entity;
        }

        public async Task<MemoryStream> Export(int type)
        {
            var memory = new MemoryStream();
            IDataReader reader = null;
            var workbook = new XSSFWorkbook();
            try
            {
                var sql = new StringBuilder("SELECT \"QuestionContent\",\"Intent\",\"Score\",\"AnswerTextContent\",\"TimeOfOccurrence\",\"FeedbackResults\" FROM \"FeedbackInfos\" WHERE \"IsDelete\"=FALSE");

                // 添加worksheet
                var excelSheet = workbook.CreateSheet("用户反馈");
                var row = excelSheet.CreateRow(0);
                //添加头
                row.CreateCell(0).SetCellValue("用户问题");
                row.CreateCell(1).SetCellValue("匹配意图");
                row.CreateCell(2).SetCellValue("意图匹配度");
                row.CreateCell(3).SetCellValue("回复内容");
                row.CreateCell(4).SetCellValue("时间");
                row.CreateCell(5).SetCellValue("反馈意见");

                if (type != 0)
                {
                    sql.Append(" AND \"FeedbackResults\"=@type");
                }
                reader = await _context.ExecuteReaderAsync(sql.ToString(), new { type });
                var index = 1;
                while (reader.Read())
                {
                    row = excelSheet.CreateRow(index);
                    row.CreateCell(0).SetCellValue(reader[0]?.ToString());
                    row.CreateCell(1).SetCellValue(reader[1]?.ToString());
                    row.CreateCell(2).SetCellValue(reader[2]?.ToString());
                    row.CreateCell(3).SetCellValue(reader[3]?.ToString());
                    row.CreateCell(4).SetCellValue(reader[4]?.ToString());
                    row.CreateCell(5).SetCellValue(FeedbackTypeToString(reader[5]?.ToString()));

                    index++;
                }

                workbook.Write(memory);
            }
            catch (Exception)
            {
            }
            finally
            {
                reader?.Close();
                reader?.Dispose();
                workbook.Close();
            }
            return memory;
        }

        public async Task<List<FeedbackInfo>> FilterByFeedbackAsync(int keyword, int skipNum, int count)
        {
            if (keyword != 0)
            {
                return await _context.FeedbackInfos.Where(f => f.IsDelete == false && f.FeedbackResults == keyword)
                  .OrderByDescending(f => f.TimeOfOccurrence)
                  .Skip(skipNum)
                  .Take(count)?.ToListAsync();

            }
            else
            {
                return await _context.FeedbackInfos.Where(f => f.IsDelete == false)
                  .OrderByDescending(f => f.TimeOfOccurrence)
                  .Skip(skipNum)
                  .Take(count)?.ToListAsync();
            }

        }

        public async Task<FeedbackConsolePieModel> GetFeedbackPieAsync()
        {
            FeedbackConsolePieModel model = new FeedbackConsolePieModel();
            model.Total = await GetTotalCountAsync(0);
            model.SatisfiedCount = await GetTotalCountAsync(1);
            model.CommonlyCount = await GetTotalCountAsync(2);
            model.DissatisfiedCount = await GetTotalCountAsync(3);
            return model;
        }

        public async Task<List<FeedbackInfo>> GetPagingFeedbackAsync(int skipNum, int count)
        {
            return await _context.FeedbackInfos.Where(f => f.IsDelete == false).OrderBy(f => f.TimeOfOccurrence).Skip(skipNum).Take(count)?.ToListAsync();
        }

        /// <summary>
        /// 根据反馈结果来获得所有
        /// </summary>
        /// <param name="keyword">0:代表所有,1:代表满意,2:基本解决问题,3:答非所问</param>
        /// <returns></returns>
        public async Task<int> GetTotalCountAsync(int keyword)
        {
            if (keyword != 0)
            {
                return await _context.FeedbackInfos.Where(f => f.IsDelete == false && f.FeedbackResults == keyword).CountAsync();
            }
            else
            {
                return await _context.FeedbackInfos.Where(f => f.IsDelete == false).CountAsync();
            }

        }

        public async Task<bool> RemovByIdAsync(Guid Id)
        {
            FeedbackInfo feedback = await _context.FeedbackInfos.FirstOrDefaultAsync(f => f.FBId == Id);
            feedback.IsDelete = true;
            var result = await _context.SaveChangesAsync();
            if (result >= 1)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        // TODO : 反馈统计
        // select "Intent","FeedbackResults",COUNT(*) as "Count" from "FeedbackInfos" GROUP BY "Intent","FeedbackResults" HAVING "FeedbackResults"=1 ORDER BY "Count" DESC LIMIT 10
        public async Task<IList<FeedbackStatisticsResult>> FeedbackStatistics(int result, int limit = 10)
        {
            var sql = $"select \"Intent\",COUNT(*) as \"Count\" from \"FeedbackInfos\" GROUP BY \"Intent\",\"FeedbackResults\" HAVING \"FeedbackResults\"={result} ORDER BY \"Count\" DESC LIMIT {limit}";
            var data = await _context.SqlQueryAsync<FeedbackStatisticsResult>(sql);
            return data?.ToList();
        }


        private string FeedbackTypeToString(string type)
        {
            if (int.TryParse(type, out var result))
            {
                // 1：满意 2：一般 3：糟糕
                switch (result)
                {
                    case 1:
                        return "满意";
                    case 2:
                        return "一般";
                    case 3:
                        return "糟糕";
                    default:
                        break;
                }
            }
            return "未知";
        }
    }
}

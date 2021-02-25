using Microsoft.EntityFrameworkCore.Extensions;
using MoMoBot.Infrastructure.Database;
using MoMoBot.Infrastructure.Models;
using MoMoBot.Infrastructure.ViewModels;
using MoMoBot.Service.Abstractions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoMoBot.Service.Implements
{
    public class PermissionService
        : IPermissionService
    {
        private readonly MoMoDbContext _dbContext;
        public PermissionService(MoMoDbContext context)
        {
            _dbContext = context;
        }

        public async Task<IList<ModularResponseResult>> FilterModulePermissionAsync(List<long> departIds, List<ModularResponseResult> source)
        {
            if (source?.Count > 0)
            {
                var sql = string.Empty;
                if (departIds?.Count > 0)
                {
                    sql = $"SELECT a.* FROM \"Modulars\" a LEFT JOIN \"ModularPermissions\" b ON a.\"Id\" = b.\"ModularId\" WHERE b.\"DepartmentId\" IN({string.Join(",", departIds)}) OR a.\"IsPublic\" = TRUE";
                }
                else
                {
                    sql = "SELECT a.* FROM \"Modulars\" a LEFT JOIN \"ModularPermissions\" b ON a.\"Id\" = b.\"ModularId\" WHERE a.\"IsPublic\" = TRUE";
                }
                var queryResult = await _dbContext.SqlQueryAsync<Modular>(sql);
                var modules = queryResult?.ToList();
                if (modules?.Count > 0)
                {
                    return source.Where(s => modules.Any(m => m.ModularId == s.ListName)).ToList();
                }
            }
            return new List<ModularResponseResult>();
        }

        public async Task SyncPermisions()
        {
            var sql = "UPDATE \"Answers\" SET \"IsPublic\"=TRUE WHERE \"IsPublic\"<>TRUE AND \"Id\" NOT IN( SELECT \"QAId\" FROM \"QAPermissions\" GROUP BY \"QAPermissions\".\"QAId\");UPDATE \"Modulars\" SET \"IsPublic\"=TRUE WHERE \"IsPublic\"<>TRUE AND \"Id\" NOT IN( SELECT \"ModularId\" FROM \"ModularPermissions\" GROUP BY \"ModularId\");";
            await _dbContext.ExecuteSqlAsync(sql);
        }
    }
}

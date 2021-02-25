using Microsoft.EntityFrameworkCore.Extensions;
using MoMoBot.Infrastructure.Database;
using MoMoBot.Infrastructure.ViewModels;
using MoMoBot.Service.Abstractions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoMoBot.Service.Implements
{
    public class HotIntentService
        : IHotIntentService
    {
        private readonly MoMoDbContext _dbContext;
        public HotIntentService(MoMoDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<HotIntentBarViewModel>> GetHotIntentBarTopTen()
        {
            var sql = "SELECT * FROM \"HotIntents\" ORDER BY \"Count\" DESC LIMIT 10";
            return await _dbContext.SqlQueryAsync<HotIntentBarViewModel>(sql);
        }
    }
}

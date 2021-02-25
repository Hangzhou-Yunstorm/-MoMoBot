using Microsoft.EntityFrameworkCore;
using MoMoBot.Infrastructure.Database;
using MoMoBot.Infrastructure.Models;
using MoMoBot.Service.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MoMoBot.Service.Implements
{
    public class ParamenterService
        : IParameterService
    {
        private readonly MoMoDbContext _context;
        public ParamenterService(MoMoDbContext context)
        {
            _context = context;
        }

        public async Task<List<QueryParameter>> GetAllEnableParameters()
        {
            return await Query(p => p.Enable)?.ToListAsync();
        }

        public async Task<List<QueryParameter>> GetAllParameters()
        {
            return await Query(p => true)?.ToListAsync();
        }

        public IQueryable<QueryParameter> Query(Expression<Func<QueryParameter, bool>> where)
        {
            return _context.QueryParameters
                .Where(where);
        }
    }
}

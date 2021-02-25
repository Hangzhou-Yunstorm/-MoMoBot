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
    public class ModularService
        : IModularService
    {
        private readonly MoMoDbContext _dbContext;
        public ModularService(MoMoDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> AddModular(Modular modular)
        {
            if (!Existed(modular.ModularId))
            {
                await _dbContext.Modulars.AddAsync(modular);
                return await _dbContext.SaveChangesAsync() > 0;
            }
            return true;
        }

        public async Task<bool> DeleteModular(long id)
        {
            var model = FindById(id);
            _dbContext.Entry(model).State = EntityState.Deleted;
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public bool Existed(string identity, long? id = null)
        {
            if (id.HasValue)
            {
                return _dbContext.Modulars.Any(m => m.ModularId == identity && m.Id != id.Value);
            }
            return _dbContext.Modulars.Any(m => m.ModularId == identity);
        }

        public IQueryable<Modular> Find(Expression<Func<Modular, bool>> where)
        {
            return _dbContext.Modulars.Where(where);
        }

        public Modular FindById(long id)
        {
            return _dbContext.Modulars.FirstOrDefault(m => m.Id == id);
        }

        public async Task<int> GetTotalAsync(string search)
        {
            if (string.IsNullOrEmpty(search))
            {
                return await _dbContext.Modulars.CountAsync();
            }
            else
            {
                return await _dbContext.Modulars.Where(m => m.ModularName.Contains(search) || m.ModularId.Contains(search)).CountAsync();
            }
        }

        public async Task<List<Modular>> SearchAsync(string search, int limit, int offset)
        {
            if (string.IsNullOrEmpty(search))
            {
                return await _dbContext.Modulars.Skip(offset).Take(limit)?.ToListAsync();
            }
            else
            {
                return await _dbContext.Modulars.Where(m => m.ModularName.Contains(search) || m.ModularId.Contains(search)).Skip(offset).Take(limit)?.ToListAsync();
            }
        }

        public async Task<bool> UpdateAsync(Modular model)
        {
            _dbContext.Entry(model).State = EntityState.Modified;

            return await _dbContext.SaveChangesAsync() > 0;
        }
    }
}

using MoMoBot.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MoMoBot.Service.Abstractions
{
    public interface IModularService
    {
        IQueryable<Modular> Find(Expression<Func<Modular, bool>> where);

        Task<int> GetTotalAsync(string search);
        Task<List<Modular>> SearchAsync(string search, int limit, int offset);
        Modular FindById(long id);
        Task<bool> UpdateAsync(Modular model);

        Task<bool> AddModular(Modular modular);

        Task<bool> DeleteModular(long id);
        bool Existed(string identity, long? id = null);
    }
}

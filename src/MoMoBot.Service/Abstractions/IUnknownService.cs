using MoMoBot.Infrastructure.Models;
using MoMoBot.Service.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoMoBot.Service.Abstractions
{
    public interface IUnknownService
    {
        Task Record(Unknown issue);

        Task<int> GetUnknowCount();

        Task<PagingQueryResult<Unknown>> GetAllUnknowns(int limit = 10, int offset = 0);
        Task MarkResolved(Guid id, string intent = null);
    }
}

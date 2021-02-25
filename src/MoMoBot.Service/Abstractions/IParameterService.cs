using MoMoBot.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoMoBot.Service.Abstractions
{
    public interface IParameterService
    {
        Task<List<QueryParameter>> GetAllEnableParameters();
        Task<List<QueryParameter>> GetAllParameters();
    }
}

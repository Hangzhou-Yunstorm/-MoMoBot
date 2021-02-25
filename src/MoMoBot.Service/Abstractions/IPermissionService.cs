using MoMoBot.Infrastructure.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoMoBot.Service.Abstractions
{
    public interface IPermissionService
    {
        Task SyncPermisions();
        Task<IList<ModularResponseResult>> FilterModulePermissionAsync(List<long> departIds, List<ModularResponseResult> source);
    }
}

using MoMoBot.Infrastructure.Models.KnowledgeMapModel;
using MoMoBot.Service.Dtos;
using MoMoBot.Service.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoMoBot.Service.Abstractions
{
    public interface IKnowledgeService
    {
        Task<IList<EdgeDto>> GetMapEdges(long mapId);
        Task<IList<NodeDto>> GetMapNodes(long mapId);
        Task<PagingQueryResult<DialogFlowDto>> GetFlows();
        Task SaveStepsAsync(long flowId, IEnumerable<MapStep> steps);
        Task<KnowledgeMap> GetFlowByIdAsync(long flowId);
        Task<IList<MapStep>> GetFlowStepsByIdAsync(long flowId);
        Task UpdateStepsAsync(IList<MapStep> steps);
        Task<string> CreateFlowAsync(KnowledgeMap flow);
        void RemoveFlowById(long flowId);
        string UpdateFlow(KnowledgeMap flow);
    }
}

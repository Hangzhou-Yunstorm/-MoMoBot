using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Extensions;
using Microsoft.Extensions.Logging;
using MoMoBot.Infrastructure.Database;
using MoMoBot.Infrastructure.Models.KnowledgeMapModel;
using MoMoBot.Service.Abstractions;
using MoMoBot.Service.Dtos;
using MoMoBot.Service.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MoMoBot.Service.Implements
{
    public class KnowledgeService : IKnowledgeService
    {
        private readonly ILogger<KnowledgeService> _logger;
        private readonly MoMoDbContext _dbContext;
        public KnowledgeService(MoMoDbContext dbContext,
            ILogger<KnowledgeService> logger)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<string> CreateFlowAsync(KnowledgeMap flow)
        {
            if (_dbContext.KnowledgeMaps.Any(f => f.Identifier == flow.Identifier))
            {
                return $"{flow.Identifier}已存在";
            }

            await _dbContext.KnowledgeMaps.AddAsync(flow).ConfigureAwait(false);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            return string.Empty;
        }

        public async Task<KnowledgeMap> GetFlowByIdAsync(long flowId)
        {
            return await _dbContext.KnowledgeMaps.FirstOrDefaultAsync(f => f.Id == flowId);
        }

        public async Task<PagingQueryResult<DialogFlowDto>> GetFlows()
        {
            var result = new PagingQueryResult<DialogFlowDto>();
            result.Total = await _dbContext.KnowledgeMaps.CountAsync(m => m.IsDeleted == false);
            result.Data = _dbContext.KnowledgeMaps.Where(m => m.IsDeleted == false).Select(m => new DialogFlowDto
            {
                Id = m.Id,
                Name = m.Name,
                Key = m.Identifier,
                Remark = m.Remark,
                CreationTime = m.CreationTime
            }).OrderBy(d => d.CreationTime);

            return result;
        }

        public async Task<IList<MapStep>> GetFlowStepsByIdAsync(long flowId)
        {
            return await _dbContext.Steps.Where(s => s.MapId == flowId).ToListAsync().ConfigureAwait(false);
        }

        public async Task<IList<EdgeDto>> GetMapEdges(long mapId)
        {
            var sql = "SELECT DISTINCT \"Id\" AS \"source\",\"NextStep\" AS \"target\", NULL AS \"Label\" FROM \"Steps\" AS a WHERE \"MapId\"=@mapId AND a.\"NextStep\" <> 0 AND a.\"Id\" <> 0 UNION SELECT \"PrevStep\" AS \"source\", \"Id\" AS \"target\", \"TriggeredResult\" AS \"Label\" FROM \"Steps\" AS b WHERE \"MapId\" = @mapId AND b.\"PrevStep\" <> 0 and b.\"Id\" <> 0";

            return (await _dbContext.SqlQueryAsync<EdgeDto>(sql, new { mapId }))?.ToList();
        }

        public async Task<IList<NodeDto>> GetMapNodes(long mapId)
        {
            var sql = "SELECT \"Id\",\"Question\", \"Key\",\"Function\" AS \"Func\", \"Label\", \"X\", \"Y\", CASE WHEN \"StepType\" =4 THEN TRUE ELSE FALSE END AS \"IsEnd\", CASE WHEN  \"StepType\" IN (1,4) THEN 'flow-circle' WHEN \"StepType\"=3 THEN 'flow-rhombus' ELSE 'flow-rect' END AS \"Shape\"  FROM \"Steps\" WHERE \"MapId\"=@mapId";
            return (await _dbContext.SqlQueryAsync<NodeDto>(sql, new { mapId }))?.ToList();
        }

        public void RemoveFlowById(long flowId)
        {
            var flow = _dbContext.KnowledgeMaps.FirstOrDefault(f => f.Id == flowId);
            if (flow != null)
            {
                var steps = _dbContext.Steps.Where(s => s.MapId == flowId);
                if (steps != null)
                {
                    _dbContext.Steps.RemoveRange(steps);
                }
                // flow.IsDeleted = true;
                _dbContext.KnowledgeMaps.Remove(flow);
                _dbContext.SaveChanges();
            }
        }

        public async Task SaveStepsAsync(long flowId, IEnumerable<MapStep> steps)
        {
            var strategy = _dbContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                using (var tran = _dbContext.Database.BeginTransaction())
                {
                    try
                    {
                        _dbContext.Steps.RemoveRange(_dbContext.Steps.Where(s => s.MapId == flowId));
                        _dbContext.Steps.AddRange(steps);
                        await _dbContext.SaveChangesAsync();

                        tran.Commit();
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e.Message);
                        tran.Rollback();
                    }
                }
            });
        }

        public string UpdateFlow(KnowledgeMap flow)
        {
            if (_dbContext.KnowledgeMaps.Any(f => f.Id != flow.Id && f.Identifier == flow.Identifier))
            {
                return $"{flow.Identifier}已经存在";
            }
            _dbContext.KnowledgeMaps.Update(flow);
            _dbContext.SaveChanges();
            return string.Empty;
        }

        public async Task UpdateStepsAsync(IList<MapStep> steps)
        {
            _dbContext.Steps.UpdateRange(steps);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}

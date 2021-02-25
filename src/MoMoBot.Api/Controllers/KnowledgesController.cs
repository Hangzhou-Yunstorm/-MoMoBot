using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoMoBot.Api.ViewModels;
using MoMoBot.Infrastructure.Models.KnowledgeMapModel;
using MoMoBot.Service.Abstractions;
using MoMoBot.Service.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoMoBot.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class KnowledgesController : ControllerBase
    {
        private readonly IUnknownService _unknownService;
        private readonly IKnowledgeService _knowledgeService;
        public KnowledgesController(IUnknownService unknownService,
            IKnowledgeService knowledgeService)
        {
            _knowledgeService = knowledgeService;
            _unknownService = unknownService;
        }

        [HttpGet("unknowns")]
        public async Task<IActionResult> Unknowns(int pageIndex = 1, int pageSize = 10)
        {
            pageIndex = pageIndex <= 0 ? 1 : pageIndex;
            pageSize = pageSize <= 0 ? 10 : pageSize;
            var offset = (pageIndex - 1) * pageSize;
            var unknowns = await _unknownService.GetAllUnknowns(pageSize, offset);
            return Ok(unknowns);
        }

        [HttpPost("resolved")]
        public async Task<IActionResult> ResolvedUnknown([FromBody]ResolvedUnknownViewModel vm)
        {
            await _unknownService.MarkResolved(vm.Id, vm.Intent);
            return Ok();
        }

        #region 对话流程
        [HttpGet("flow")]
        public async Task<IActionResult> GetDialogFlow(long flowId)
        {
            var flow = await _knowledgeService.GetFlowByIdAsync(flowId);
            if (flow != null)
            {
                var nodes = await _knowledgeService.GetMapNodes(flowId);
                var edges = await _knowledgeService.GetMapEdges(flowId);

                return Ok(new { flow.Name, data = new { nodes, edges } });
            }
            return NotFound();
        }

        [HttpGet("flows")]
        public async Task<IActionResult> GetDialogFlows()
        {
            var result = await _knowledgeService.GetFlows();
            return Ok(result);
        }

        [HttpPost("create-flow")]
        public async Task<IActionResult> CreateFlow([FromBody]CreateFlowViewModel vm)
        {
            var flow = new KnowledgeMap
            {
                CreationTime = DateTime.Now,
                Name = vm.Name,
                Identifier = vm.Key,
                Remark = vm.Remark
            };
            var error = await _knowledgeService.CreateFlowAsync(flow).ConfigureAwait(false);
            if (!string.IsNullOrEmpty(error))
            {
                return BadRequest(error);
            }
            return Ok();
        }

        [HttpPost("edit-flow")]
        public async Task<IActionResult> EditFlow([FromBody]EditFlowViewModel vm)
        {
            var flow = await _knowledgeService.GetFlowByIdAsync(vm.FlowId).ConfigureAwait(false);
            if (flow != null && flow.IsDeleted == false)
            {
                flow.Name = vm.Name;
                flow.Identifier = vm.Key;
                flow.Remark = vm.Remark;

                var error = _knowledgeService.UpdateFlow(flow);
                if (!string.IsNullOrEmpty(error))
                {
                    return BadRequest(error);
                }
            }
            return Ok();
        }

        [HttpPost("update-flow")]
        public async Task<IActionResult> UpdateDialogFlows([FromBody]UpdateFlowViewModel vm)
        {
            var steps = vm.Nodes?.AsParallel().Select(n => new MapStep
            {
                MapId = vm.FlowId,
                X = n.X,
                Y = n.Y,
                Label = n.Label,
                Function = n.Func,
                Question = n.Question,
                Key = n.Key,
                StepType = n.isEnd.Value ? StepTypes.EndNode : GetType(n.Shape),
                TempId = $"{vm.FlowId}#{n.Id}"
            });

            await _knowledgeService.SaveStepsAsync(vm.FlowId, steps);

            await UpdateEdgesAsync(vm.FlowId, vm.Edges);

            return Ok();
        }

        [HttpDelete("remove-flow")]
        public IActionResult RemoveFlow(long flowId)
        {
            _knowledgeService.RemoveFlowById(flowId);
            return Ok();
        }
        #endregion

        private async Task UpdateEdgesAsync(long mapId, List<Edge> edges)
        {
            var steps = await _knowledgeService.GetFlowStepsByIdAsync(mapId);
            steps.AsParallel().ForAll(step =>
            {
                var id = step.TempId?.Split("#", StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
                var edge = edges.FirstOrDefault(e => e.Source == id);
                var preEdge = edges.FirstOrDefault(e => e.Target == id);
                if (edge == null)
                {
                    return;
                }

                step.TriggeredResult = preEdge?.Label;

                switch (step.StepType)
                {
                    case StepTypes.StartNode:
                        {
                            step.PrevStep = 0;
                            step.NextStep = steps.FirstOrDefault(s => s.TempId == $"{mapId}#{edge.Target}")?.Id;
                            break;
                        }
                    case StepTypes.EndNode:
                        {
                            step.PrevStep = 0;
                            step.NextStep = 0;
                            break;
                        }
                    case StepTypes.BranchNode:
                        {
                            step.PrevStep = steps.FirstOrDefault(s => s.TempId == $"{mapId}#{preEdge?.Source}")?.Id;
                            step.NextStep = 0;
                            break;
                        }
                    case StepTypes.RunningNode:
                        {
                            step.PrevStep = steps.FirstOrDefault(s => s.TempId == $"{mapId}#{preEdge?.Source}")?.Id;
                            step.NextStep = steps.FirstOrDefault(s => s.TempId == $"{mapId}#{edge.Target}")?.Id;
                            break;
                        }
                }
            });

            await _knowledgeService.UpdateStepsAsync(steps).ConfigureAwait(false);
        }

        private StepTypes GetType(string sharp)
        {
            switch (sharp)
            {
                case "flow-rhombus":
                    return StepTypes.BranchNode;
                case "flow-rect":
                    return StepTypes.RunningNode;
                case "flow-circle":
                    return StepTypes.StartNode;
            }
            throw new Exception("invalid sharp");
        }
    }
}

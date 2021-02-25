using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MoMoBot.Api.ViewModels;
using MoMoBot.Infrastructure.Enums;
using MoMoBot.Infrastructure.Luis.Models;
using MoMoBot.Infrastructure.Models;
using MoMoBot.Service.Abstractions;
using Newtonsoft.Json;
using NPOI.XSSF.UserModel;

namespace MoMoBot.Api.Controllers
{
    [Route("api/[controller]")]
    //[Authorize]
    [ApiController]
    public class LuisController : ControllerBase
    {
        private readonly IAnswerService _answerService;
        private readonly ILuisService _luisService;
        private readonly IDepartmentService _departmentService;
        private readonly ILogger<LuisController> _logger;
        public LuisController(IAnswerService answerService,
            ILuisService luisService,
            IDepartmentService departmentService,
            ILogger<LuisController> logger)
        {
            _departmentService = departmentService;
            _answerService = answerService;
            _luisService = luisService;
            _logger = logger;
        }

        #region 知识库管理
        [HttpGet("knowledges")]
        public async Task<IActionResult> Knowledges(string search = "", int pageIndex = 1, int pageSize = 10)
        {
            var limit = pageSize <= 0 ? 10 : pageSize;
            pageIndex = pageIndex < 0 ? 1 : pageIndex;
            var offset = (pageIndex - 1) * limit;

            var result = new PaginationViewModel();
            var data = await _answerService.SearchAsync(search, limit, offset);
            if (data != null)
            {
                result.Rows = data;
                result.Total = await _answerService.GetTotalCountAsync(search);
            }
            result.PageIndex = pageIndex;
            result.PageSize = limit;
            return Ok(result);
        }

        [HttpGet("knowledge/{id}")]
        public async Task<IActionResult> Knowledge(Guid id)
        {
            var answer = await _answerService.GetParameterSettings(id);
            if (answer != null)
            {
                var requestUrl = string.IsNullOrWhiteSpace(answer.RequestUrl) ? "" : answer.RequestUrl;
                return Ok(new
                {
                    answer.FlowId,
                    answer.AnswerType,
                    answer.Id,
                    answer.Answer,
                    answer.Intent,
                    requestUrl,
                    answer.Method,
                    answer.AnswerQueries
                });
            }
            return NotFound();
        }

        [HttpPost("update-knowledge")]
        public async Task<IActionResult> UpdateKnowledge(UpdateKnowledgeViewModel vm)
        {
            var model = await _answerService.GetAsync(vm.Id);
            if (model != null)
            {
                model.Intent = vm.Intent;
                model.RequestUrl = vm.RequestUrl;
                model.Answer = vm.Answer ?? vm.FlowId;
                model.Method = vm.Method;
                model.FlowId = vm.FlowId;
                model.AnswerType = (AnswerTypes)vm.AnswerType;

                // 先删除原来的
                await _answerService.RemoveParameters(model.Id);
                if (vm.ParameterIds?.Count > 0)
                {
                    List<AnswerQueries> parameters = new List<AnswerQueries>();
                    foreach (var parameter in vm.ParameterIds)
                    {
                        parameters.Add(new AnswerQueries
                        {
                            AnswerId = model.Id,
                            ParameterId = parameter
                        });
                    }
                    await _answerService.AddParameters(parameters);
                }

                await _answerService.UpdateAllProperties(model);
                var result = await _answerService.SaveChangesAsync();
                return result ? Ok() : (IActionResult)BadRequest();
            }
            return NotFound();
        }

        [HttpDelete("delete-knowledge/{id}")]
        public async Task<IActionResult> DeleteKnowledge(Guid Id)
        {
            await _answerService.RemoveById(Id);
            bool result = await _answerService.SaveChangesAsync();  //保存一下修改
            return new StatusCodeResult(201);
        }

        [HttpPost("create-knowledge")]
        public async Task<IActionResult> AddKnowledge(CreateKnowledgeViewModel vm)
        {
            var id = Guid.NewGuid();
            await _answerService.AddAsync(new QandA { Id = id, Answer = vm.Answer, Intent = vm.Intent, IsPublic = true });
            await _answerService.SaveChangesAsync();
            return Created("/api/luis/knowledge", new { id });
        }

        /// <summary>
        /// 导出数据
        /// </summary>
        /// <param name="format">导出格式</param>
        /// <returns></returns>
        [HttpPost("export-knowledges")]
        [AllowAnonymous]
        public async Task<IActionResult> Export([FromForm]string format = "excel")
        {
            var fileName = DateTime.Now.ToString("yyyyMMddHHmmss");
            // 导出为json文件
            if (format.Equals("json", StringComparison.OrdinalIgnoreCase))
            {
                var content = await _answerService.GetJsonDataAsync();
                var buffer = Encoding.UTF8.GetBytes(content);
                return File(buffer, "application/x-javascript", $"{fileName}.json");
            }
            // 导出为excel文件
            else if (format.Equals("excel", StringComparison.OrdinalIgnoreCase))
            {
                return await ExportExcelFile($"{fileName}.xlsx");
            }
            return BadRequest("文件格式错误");
        }

        [HttpPost("import-knowledges")]
        public async Task<IActionResult> Import([FromForm]IFormFile file)
        {
            if (file != null)
            {
                try
                {
                    if (file.Length >= 1024 * 1000)
                    {
                        return BadRequest("文件大小不能超过10M");
                    }
                    var ext = file.FileName.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries).Last();
                    if (ext.Equals("json", StringComparison.CurrentCultureIgnoreCase))
                    {
                        var count = await ImportFromJsonFile(file);
                        return Ok($"成功导入{count}条数据");
                    }
                    else if (ext.Equals("xlsx", StringComparison.CurrentCultureIgnoreCase) ||
                       ext.Equals("xls", StringComparison.CurrentCultureIgnoreCase))
                    {
                        var count = await ImportFromExcelFile(file);
                        return Ok($"成功导入{count}条数据");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    return BadRequest("导入数据时发生异常，请重试！");
                }
            }
            return BadRequest("请选择文件上传！");
        }

        [HttpGet("dialog-flows")]
        public IEnumerable<DialogFlowViewModel> GetAllDialogFlows()
        {
            var flows = _answerService.GetDialogFlowsQuery(m => m.IsDeleted == false)?.Select(m => new DialogFlowViewModel
            {
                FlowId = m.Identifier,
                Name = m.Name
            });

            return flows;
        }
        #endregion

        #region 意图管理
        [HttpGet("search-intents")]
        public async Task<IActionResult> SearchIntents(string search)
        {
            var intents = await _luisService.Search(search);
            return Ok(intents);
        }

        [HttpPost("create-intent")]
        public async Task<IActionResult> CreateIntent([FromBody]CreateIntentViewModel model)
        {
            var result = await _luisService.AddIntentAsync(model.Intent);
            return Ok(result);
        }

        [HttpGet("intents/{id:guid}")]
        public async Task<IActionResult> GetIntent(Guid id)
        {
            var intent = await _luisService.GetIntentByIdAsync(id);
            if (intent == null)
            {
                return NotFound();
            }
            var examples = await _luisService.GetExamplesAsync(intent.Name);
            return Ok(new { intent, examples });
        }

        [HttpPost("example")]
        public async Task<IActionResult> AddExample([FromBody]InputExampleViewModel vm)
        {
            var utterance = await _luisService.AddExampleAsync(vm.Text, vm.Intent);
            return Ok(utterance);
        }

        [HttpPost("update-intent")]
        public async Task<IActionResult> UpdateIntent([FromBody]UpdateIntentViewModel model)
        {
            await _luisService.RenameIntentAsync(model.Id, model.Intent);
            if (model.UpdatePermission)
            {
                await _departmentService.UpdatePowerAsync(model.Intent, model.Permissions);
            }
            return Ok();
        }

        [HttpDelete("delete-intent/{id}")]
        public async Task<IActionResult> DeleteIntent(Guid id)
        {
            if (Guid.Empty != id)
            {
                await _luisService.DeleteIntentAsync(id);
                return Ok();
            }
            return BadRequest("id不能为空或者null");
        }
        #endregion

        #region LUIS
        [HttpGet("endpointhitshistory")]
        public async Task<IActionResult> GetEndpointHitHistory(int? perDays = 7)
        {
            var history = await _luisService.GetEndpointHitHistory(perDays);
            return Ok(history);
        }

        [HttpGet("appinfo")]
        public async Task<IActionResult> GetLuisAppInfo()
        {
            var app = await _luisService.GetCurrentLuisAppInfoAsync();
            return Ok(app);
        }

        [HttpPost("train")]
        public async Task<IActionResult> Train()
        {
            var details = await _luisService.TrainAsync();
            return Ok(details);
        }

        [HttpGet("trainingstatus")]
        public async Task<IActionResult> GetTrainingStatusList()
        {
            var list = await _luisService.GetTrainingStatusListAsync();
            return Ok(list);
        }

        [HttpPost("publish")]
        public async Task<IActionResult> Publish()
        {
            var msg = await _luisService.PublishAsync();
            return Ok(msg);
        }

        [HttpGet("statsmetadata")]
        public async Task<LuisStatsMetadata> GetStatsMetadata()
        {
            return await _luisService.GetCurrentVersionStatsMetadataAsync();
        }

        [HttpGet("intentstats")]
        public async Task<object> GetIntentStats(string intentId)
        {
            var stats = await _luisService.GetIntentStatsAsync(intentId);

            if (stats != null)
            {
                var incorrectly = new List<IncorrectlyResult>();
                var unclear = new List<IncorrectlyResult>();
                var temp1 = stats.Utterances?.Where(u => u.MisclassifiedIntentPredictions?.Count > 0)
                    .Select(u => new
                    {
                        u.MisclassifiedIntentPredictions,
                        u.AmbiguousIntentPredictions
                    });
                if (temp1 != null)
                {
                    foreach (var item in temp1)
                    {
                        item.MisclassifiedIntentPredictions.ForEach(e =>
                        {
                            var existed = incorrectly.FirstOrDefault(i => i.Id == e.Id);
                            if (existed != null)
                            {
                                incorrectly.Remove(existed);
                                existed.Count++;
                                incorrectly.Add(existed);
                            }
                            else
                            {
                                incorrectly.Add(new IncorrectlyResult
                                {
                                    Name = item.AmbiguousIntentPredictions.FirstOrDefault(a => a.Id == e.Id)?.Name,
                                    Count = 1,
                                    Id = e.Id
                                });
                            }
                        });
                    }
                }

                var temp2 = stats.Utterances?.Where(u => u.MisclassifiedIntentPredictions?.Count <= 0 && u.AmbiguousIntentPredictions?.Count > 0).Select(u => u.AmbiguousIntentPredictions);
                if (temp2 != null)
                {
                    foreach (var item in temp2)
                    {
                        item.ForEach(e =>
                        {
                            var existed = unclear.FirstOrDefault(i => i.Id == e.Id);
                            if (existed != null)
                            {
                                unclear.Remove(existed);
                                existed.Count++;
                                unclear.Add(existed);
                            }
                            else
                            {
                                unclear.Add(new IncorrectlyResult
                                {
                                    Name = e.Name,
                                    Count = 1,
                                    Id = e.Id
                                });
                            }
                        });
                    }
                }

                return new { incorrectly, unclear };
            }
            return null;
        }

        [HttpGet("intentstatsmetadata")]
        public async Task<IntentStatsMetadata> GetIntentStatsMetadata(string intentId)
        {
            return await _luisService.GetIntentStatsAsync(intentId);
        }

        #endregion


        /// <summary>
        /// 导出excel文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private async Task<FileResult> ExportExcelFile(string fileName)
        {
            var memory = new MemoryStream();
            var list = await _answerService.GetAllQandAsAsync();

            var workbook = new XSSFWorkbook();
            // 添加worksheet
            var excelSheet = workbook.CreateSheet("小摩摩知识库");
            var row = excelSheet.CreateRow(0);
            //添加头
            row.CreateCell(0).SetCellValue("意图");
            row.CreateCell(1).SetCellValue("回答");

            if (list != null)
            {
                for (var i = 0; i < list.Count; i++)
                {
                    row = excelSheet.CreateRow(i + 1);
                    row.CreateCell(0).SetCellValue(list[i].Intent);
                    row.CreateCell(1).SetCellValue(list[i].Answer);
                }
            }
            workbook.Write(memory);
            var buffer = memory.GetBuffer();
            return File(buffer, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        private async Task<int> ImportFromJsonFile(IFormFile file)
        {
            try
            {
                using (var stream = file.OpenReadStream())
                {
                    var buffer = new byte[file.Length];
                    await stream.ReadAsync(buffer, 0, buffer.Length);
                    var json = Encoding.UTF8.GetString(buffer);
                    var list = JsonConvert.DeserializeObject<List<QandA>>(json);

                    var answers = new List<QandA>();
                    if (list != null)
                    {
                        foreach (var item in list)
                        {
                            if (await ValidImportData(item.Intent, item.Answer))
                            {
                                answers.Add(new QandA { Id = Guid.NewGuid(), Answer = item.Answer, Intent = item.Intent });
                            }
                        }
                        await _answerService.AddRangeAsync(answers);
                        await _answerService.SaveChangesAsync();
                    }
                    return answers.Count;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"从json导入数据失败：{ex.Message}");
                throw ex;
            }
        }
        private async Task<int> ImportFromExcelFile(IFormFile file)
        {
            try
            {
                using (var stream = file.OpenReadStream())
                {
                    //创建 XSSFWorkbook和ISheet实例
                    XSSFWorkbook workbook = new XSSFWorkbook(stream);

                    var sheet = workbook.GetSheetAt(0);
                    //获取sheet的首行
                    int rowCount = sheet.LastRowNum;
                    var answers = new List<QandA>();
                    for (var i = sheet.FirstRowNum; i <= sheet.LastRowNum; i++)
                    {
                        var row = sheet.GetRow(i);
                        if (row.GetCell(0) != null &&
                             row.GetCell(1) != null)
                        {
                            var intent = row.GetCell(0).StringCellValue.Trim();
                            var answer = row.GetCell(1).StringCellValue;
                            if (await ValidImportData(intent, answer))
                            {
                                answers.Add(new QandA
                                {
                                    Id = Guid.NewGuid(),
                                    Intent = intent,
                                    Answer = answer
                                });
                            }
                        }
                    }
                    await _answerService.AddRangeAsync(answers);
                    await _answerService.SaveChangesAsync();
                    return answers.Count;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"从excel导入数据失败：{ex.Message}");
                throw ex;
            }
        }
        private async Task<bool> ValidImportData(string intent, string answer)
        {
            if (!string.IsNullOrWhiteSpace(intent) &&
                !string.IsNullOrWhiteSpace(answer))
            {
                //if (!await _answerService.Exist(intent, answer))
                //{
                //    return true;
                //}
                return await Task.FromResult(true);
            }
            return await Task.FromResult(false);
        }

    }

    public class IncorrectlyResult
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Count { get; set; } = 1;
    }
}

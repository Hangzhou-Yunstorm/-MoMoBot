using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoMoBot.Infrastructure.Models;
using MoMoBot.Service.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoMoBot.Api.Controllers
{
    [Route("api/department")]
    //[Authorize]
    [ApiController]
    public class DepartmentController : Controller
    {
        private readonly IDepartmentService _departmentService;
        private readonly IPermissionService _permissionService;
        public DepartmentController(IDepartmentService departmentService,
            IPermissionService permissionService)
        {
            _departmentService = departmentService;
            _permissionService = permissionService;
        }

        [HttpGet("tree")]
        public async Task<IActionResult> Tree()
        {
            var models = await _departmentService
                .Find(w => true)
                .ToListAsync();

            //TODO : 缓存
            var treeData = DepartmentsToTreeData(models);

            return Ok(treeData);
        }

        [HttpGet("intentpower")]
        public IActionResult GetPowerByIntent(string intent)
        {
            var result = _departmentService.GetIntentPower(intent);
            return Ok(result);
        }

        private IList<TreeData> DepartmentsToTreeData(IEnumerable<Department> departments, long? parentId = null)
        {
            IList<TreeData> tree = new List<TreeData>();
            if (departments != null)
            {
                var tops = departments.Where(d => d.ParentId == parentId);
                if (tops != null)
                {
                    foreach (var first in tops)
                    {
                        tree.Add(new TreeData
                        {
                            Id = first.Id,
                            Text = first.DepartName,
                            Children = DepartmentsToTreeData(departments, first.Id)
                        });
                    }
                }
            }

            return tree;
        }
    }

    public class TreeData
    {
        public TreeData()
        {
            Children = new List<TreeData>();
        }
        public long Id { get; set; }
        public string Text { get; set; }
        public IList<TreeData> Children { get; set; }
    }
}

using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MoMoBot.Infrastructure.ViewModels
{
    public class InputDepartViewModel
    {
        public InputDepartViewModel()
        {
            Departs = new List<SelectListItem>();
        }

        [Required(ErrorMessage = "部门名称不能为空")]
        public string DepartName { get; set; }
        public long? ParentId { get; set; }

        [JsonIgnore]
        public List<SelectListItem> Departs { get; set; }
    }
}

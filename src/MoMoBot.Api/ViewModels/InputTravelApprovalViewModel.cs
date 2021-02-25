using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MoMoBot.Infrastructure.DTalk;

namespace MoMoBot.Api.ViewModels
{
    public class InputTravelApprovalViewModel
    {
        public InputTravelApprovalViewModel()
        {
            StartTime = DateTime.Now;
            EndTime = DateTime.Now.AddDays(1);
            TicketService = "false";
        }

        [Required]
        public string Cause { get; set; }
        [Required]
        public string Vehicle { get; set; }
        [Required]
        public string OneWayOrRoundTrip { get; set; }
        [Required]
        public string DepartureCity { get; set; }
        [Required]
        public string DestinationCity { get; set; }
        [Required]
        public DateTime StartTime { get; set; }
        [Required]
        public DateTime EndTime { get; set; }
        public string Name { get; set; }
        public string UserId { get; set; }
        public string Remarks { get; set; }
        public string Peers { get; set; }
        public string TicketService { get; set; }
        public string DepartId { get; set; }

        public TravelApprovalRequestModel ConvertToRequestModel(string approvers)
        {
            var days = (EndTime - StartTime).Days + 1;
            return new TravelApprovalRequestModel
            {
                originator_user_id = UserId,
                dept_id = DepartId,
                approvers = approvers,
                form_component_values = new List<FormComponentValue> {
                    new FormComponentValue{name="出差事由",value=Cause },
                    new FormComponentValue{name="单程/往返",value=OneWayOrRoundTrip },
                    new FormComponentValue{name="交通工具",value=Vehicle },
                    new FormComponentValue{name="出发城市",value=DepartureCity },
                    new FormComponentValue{name="目的城市",value=DestinationCity },
                    new FormComponentValue{name="开始时间",value=StartTime.ToString("yyyy年MM月dd日 HH:mm") },
                    new FormComponentValue{name="结束时间",value=EndTime.ToString("yyyy年MM月dd日 HH:mm") },
                    new FormComponentValue{name="出差天数",value=$"{days}" },
                    new FormComponentValue{name="需要订票",value=TicketService },
                    new FormComponentValue{name="同行人",value=Peers },
                    new FormComponentValue{name="备注",value=Remarks }
                }
            };
        }
    }
}

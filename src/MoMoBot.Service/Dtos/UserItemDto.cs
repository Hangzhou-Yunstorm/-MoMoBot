using System;
using System.Collections.Generic;
using System.Text;

namespace MoMoBot.Service.Dtos
{
    public class UserItemDto : UserDto
    {
        public List<string> Roles { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace MoMoBot.Service.Dtos
{
    public class UserDto
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public bool? LockoutEnabled { get; set; }
        public string Nickname { get; set; }
    }
}

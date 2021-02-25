using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace MoMoBot.Infrastructure.Models
{
    public class MoMoBotUser : IdentityUser
    {
        [StringLength(20)]
        public string Nickname { get; set; }
        [StringLength(100)]
        public string Avatar { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MoMoBot.Api.ViewModels
{
    public class CreateUserViewModel
    {
        [MaxLength(10)]
        public string Nickname { get; set; }
        [Required]
        [MaxLength(18)]
        public string Username { get; set; }
        [Required]
        [MaxLength(30)]
        public string Email { get; set; }
        [Required]
        [MaxLength(20)]
        public string Password { get; set; }
        public string[] Roles { get; set; }
    }
}

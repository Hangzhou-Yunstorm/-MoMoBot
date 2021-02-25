using System.ComponentModel.DataAnnotations;
using MoMoBot.Infrastructure.Models;

namespace MoMoBot.Infrastructure.ViewModels
{
    public class InputModularViewModel
    {
        [Required]
        public string ModularId { get; set; }
        [Required]
        public string ModularName { get; set; }
        public string Remarks { get; set; }
    }

    public static class InputModularViewModelExtensions
    {
        public static Modular ToModel(this InputModularViewModel vm)
        {
            if (vm == null)
            {
                return null;
            }
            return new Modular
            {
                ModularId = vm.ModularId,
                ModularName = vm.ModularName,
                Remarks=vm.Remarks
            };
        }

        public static InputModularViewModel ConvertToViewModel(this Modular model) => new InputModularViewModel
        {
            ModularName = model.ModularName,
            ModularId = model.ModularId,
            Remarks=model.Remarks
        };
    }

}

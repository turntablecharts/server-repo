using System.ComponentModel.DataAnnotations;
using Core.Entities;

namespace Presentation.ViewModels
{
    public class TtcUserVM : TtcUser
    {
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string Role {get; set;}
    }
}
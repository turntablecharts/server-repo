using System.ComponentModel.DataAnnotations;
using Core.Entities;
using Presentation.Enums;

namespace Presentation.ViewModels
{
    public class TtcUserVM : TtcUser
    {
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        
        public AppUserRoles Role {get; set;}
    }
}
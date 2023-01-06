using Core.Entities;

namespace Presentation.ViewModels
{
    public class MagazineVM : MagazineData
    {
        public string Email { get; set; }
        public string Edition { get; set; }
    }
}
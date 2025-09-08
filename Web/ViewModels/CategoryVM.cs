using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels
{
    public class CategoryVM
    {
        public Guid? Id { get; set; }

        [StringLength(100, MinimumLength =3)]
        public string Name { get; set; } = string.Empty;

        
    }
}

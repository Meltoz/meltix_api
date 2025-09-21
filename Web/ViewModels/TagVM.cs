using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels
{
    public class TagVM
    {
        [Required]
        public Guid Id { get; set; }

        [StringLength(50, MinimumLength =3)]
        public string Name { get; set; } = string.Empty;

        public int? VideoCount {get;set;}
    }
}

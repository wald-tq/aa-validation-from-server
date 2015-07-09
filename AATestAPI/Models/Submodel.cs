using System.ComponentModel.DataAnnotations;

namespace AATestAPI.Models
{
    public class Submodel
    {
        [Required]
        [MaxLength(15)]
        public string Street { get; set; }

        [MaxLength(5)]
        public string City { get; set; }
    }
}
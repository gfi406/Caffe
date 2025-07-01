using System.ComponentModel.DataAnnotations;

namespace Caffe.Models.Dto
{
    public class AddToCartDto
    {
        [Required]
        public Guid MenuItemId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }
}

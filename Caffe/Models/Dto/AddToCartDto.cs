namespace Caffe.Models.Dto
{
    public class AddToCartDto
    {
        public Guid MenuItemId { get; set; }
        public int Quantity { get; set; } = 1;
    }
}

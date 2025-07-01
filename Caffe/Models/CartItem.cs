using Caffe.Models.Entity;

namespace Caffe.Models
{
    public class CartItem : BaseEntity
    {
        public Guid CartId { get; set; }
        public Cart Cart { get; set; }

        public int Price { get; set; }

        public Guid MenuItemId { get; set; }
        public MenuItem MenuItem { get; set; }

        public int Quantity { get; set; }

    }
}

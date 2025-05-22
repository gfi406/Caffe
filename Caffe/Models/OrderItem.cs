using Caffe.Models.Entity;

namespace Caffe.Models
{
    public class OrderItem : BaseEntity
    {
        public Guid OrderId { get; set; }
        public Order Order { get; set; }

        public Guid MenuItemId { get; set; }
        public MenuItem MenuItem { get; set; }

        public int Quantity { get; set; }
        public int PriceAtOrderTime { get; set; }
    }
}

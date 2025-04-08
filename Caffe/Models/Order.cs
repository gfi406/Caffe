using Caffe.Models.Entity;

namespace Caffe.Models
{
    public class Order : BaseEntity
    {
        public Cart Cart { get; set; }
        public Guid CartId { get; set; }
        public User User { get; set; }
        public Guid user_id { get; set; }
        public string status { get; set; }
        public string orderNumber { get; set; }
        public string paymentMethod { get; set; }
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}

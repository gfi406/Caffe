using Caffe.Models.Entity;

namespace Caffe.Models
{
    public class Cart :BaseEntity
    {
        public List<CartItem> Items { get; set; } = new List<CartItem>();

        public User User { get; set; }
        public Guid user_id { get; set; }
       public int? totalPrice { get; set; }
        public Order Order { get; set; }
    }
}

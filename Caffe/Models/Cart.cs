using Caffe.Models.Entity;

namespace Caffe.Models
{
    public class Cart :BaseEntity
    {
        public List<MenuItem> Items { get; set; }

        public User User { get; set; }
        public Guid user_id { get; set; }

        public int? totalPrice { get; set; }
        public Order Order { get; set; }
    }
}

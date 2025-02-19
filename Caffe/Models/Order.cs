using Caffe.Models.Entity;

namespace Caffe.Models
{
    public class Order : BaseEntity
    {
        public Cart Cart { get; set; }
        public Guid CartId { get; set; }
        public User User { get; set; }
        public Guid user_id { get; set; }

        public String status { get; set; }
        public int orderNumber { get; set; }

        public String paymentMethod { get; set; }
        
        

    }
}

using Caffe.Models.Entity;

namespace Caffe.Models
{
    public class User : BaseEntity
    {
        public bool is_admin { get; set; }
        public bool is_active { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string phone { get; set; }
        public string image { get; set; }
        public Cart Cart { get; set; }
        public List<Order> Orders { get; set; }


    }
}

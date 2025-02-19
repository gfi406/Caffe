using Caffe.Models.Entity;

namespace Caffe.Models
{
    public class MenuItem : BaseEntity
    {
        public Boolean is_availble { get; set; }
        public String title { get; set; }
        public String description { get; set; }
        public String img { get; set; }
        public int price { get; set; }   
    }
}

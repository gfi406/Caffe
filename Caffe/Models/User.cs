using Caffe.Models.Entity;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Caffe.Models
{
    public class User : BaseEntity
    {

        public bool is_admin { get; set; }
        public bool is_active { get; set; }

        // personal data
        public string name { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string phone { get; set; }
        public string? image { get; set; }


        public byte[]? UserIcon { get; set; }
        public string? FileName { get; set; }
        public string? ContentType { get; set; }       

        public Cart Cart { get; set; }
        public List<Order> Orders { get; set; }

        public User()
        {
            name = String.Empty;
            email = String.Empty;
            password = String.Empty;
            phone = String.Empty;
           

            UpdateTimestamp();
        }


    }
}

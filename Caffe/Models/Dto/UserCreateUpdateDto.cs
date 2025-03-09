namespace Caffe.Models.Dto
{
    public class UserCreateUpdateDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public bool IsAdmin { get; set; }
        public string image { get; set; }
        public bool IsActive { get; set; } = true;
    }
}

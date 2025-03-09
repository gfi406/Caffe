namespace Caffe.Models.Dto
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsActive { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string? Image { get; set; }
        
        public Guid? CartId { get; set; }
        public List<Guid> OrderIds { get; set; } = new List<Guid>();
    }
}

namespace Caffe.Models.Dto
{
    public class CartDto
    {
        public Guid Id { get; set; }
        public List<MenuItemDto> Items { get; set; } = new List<MenuItemDto>();
        public Guid UserId { get; set; }
        public int? TotalPrice { get; set; }
        public Guid? OrderId { get; set; }
    }
}

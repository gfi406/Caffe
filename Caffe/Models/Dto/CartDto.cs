namespace Caffe.Models.Dto
{
    public class CartDto
    {
        public Guid Id { get; set; }
        public List<MenuCartItemDto> Items { get; set; } = new List<MenuCartItemDto>();
        public Guid UserId { get; set; }
        
        public int? TotalPrice { get; set; }
        public Guid? OrderId { get; set; }
    }
}

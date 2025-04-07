namespace Caffe.Models.Dto
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        public Guid CartId { get; set; }
        public Guid UserId { get; set; }
        public string Status { get; set; }
        public int OrderNumber { get; set; }
        public string PaymentMethod { get; set; }
        public List<MenuCartItemDto> Items { get; set; } = new List<MenuCartItemDto>();
        public int? TotalPrice { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

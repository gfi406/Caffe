namespace Caffe.Models.Dto
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        public Guid CartId { get; set; }
        public Guid UserId { get; set; }
        public string Status { get; set; }
        public string OrderNumber { get; set; }
        public string PaymentMethod { get; set; }
        public List<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();
        public int? TotalPrice { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

namespace Caffe.Models.Dto
{
    public class OrderCreateDto
    {
        public Guid CartId { get; set; }
        public string PaymentMethod { get; set; }
    }
}

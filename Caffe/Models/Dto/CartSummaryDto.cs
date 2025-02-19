namespace Caffe.Models.Dto
{
    public class CartSummaryDto
    {
        public Guid Id { get; set; }
        public int ItemCount { get; set; }
        public int? TotalPrice { get; set; }
        public Guid UserId { get; set; }
    }
}

namespace Caffe.Models.Dto
{
    public class MenuItemDto
    {
        public Guid Id { get; set; }
        public bool IsAvailable { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public int Price { get; set; }
    }
}

using Caffe.Models.Enum;

namespace Caffe.Models.Dto
{
    public class MenuItemCreateUpdateDto
    {
        public bool IsAvailable { get; set; } = true;
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public int Price { get; set; }
        public FoodCategory Category { get; set; }
    }
}

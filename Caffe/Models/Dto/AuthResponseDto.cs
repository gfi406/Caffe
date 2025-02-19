namespace Caffe.Models.Dto
{
    public class AuthResponseDto
    {
        public string Token { get; set; }
        public UserDto User { get; set; }
        public DateTime Expiration { get; set; }
    }
}

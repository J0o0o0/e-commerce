namespace e_commerce.DTOs.AuthDTOs
{
    public class AuthResultDto
    {
        public string Token { get; set; }
        public int UserId { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Role { get; set; }
    }
}
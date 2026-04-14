namespace e_commerce.DTOs.AuthDTOs
{
    public class RegisterBuyerDto
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ShippingAddress { get; set; }
        public string PhoneNumber { get; set; }
    }
}
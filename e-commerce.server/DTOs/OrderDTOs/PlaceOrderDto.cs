using e_commerce.Models;

namespace e_commerce.DTOs.OrderDTOs
{
    public class PlaceOrderDto
    {
        public PaymentProvider PaymentProvider { get; set; } = PaymentProvider.CashOnDelivery;
       
    }
}
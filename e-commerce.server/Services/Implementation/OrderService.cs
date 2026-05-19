using e_commerce.DTOs.OrderDTOs;
using e_commerce.Models;
using e_commerce.Repositories.Interfaces;
using e_commerce.Services.Interfaces;

namespace e_commerce.Services.Implementation
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<OrderResponseDto> PlaceOrderAsync(int userId, PlaceOrderDto dto)
        {
            // 1 — get the buyer
            var buyer = await _unitOfWork.Users.GetBuyerByUserIdAsync(userId);
            if (buyer == null)
                throw new Exception("User needs to login");

            // 2 — get cart with items
            var cart = await _unitOfWork.Carts.GetByBuyerIdWithItemsAsync(buyer.Id);
            if (cart == null || !cart.Items.Any())
                throw new Exception("Cart is empty");

            // 3 — check stock before placing order
            foreach (var item in cart.Items)
            {
                if (item.Product.Stock < item.Quantity)
                    throw new Exception($"Product '{item.Product.Name}' only has {item.Product.Stock} left in stock.");
            }

            // 4 — snapshot items into order items
            var orderItems = cart.Items.Select(i => new OrderItem
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                PricePerUnit = i.Product.Price,
                TotalPrice = i.Product.Price * i.Quantity
            }).ToList();

            // 5 — build the order
            var order = new Order
            {
                OrderNumber = GenerateOrderNumber(),
                BuyerId = buyer.Id,
                Items = orderItems,
                TotalPrice = orderItems.Sum(i => i.TotalPrice),
                Status = OrderStatus.Pending,
                PaymentProvider = dto.PaymentProvider,
                ShippingAddress = buyer.ShippingAddress ?? "No address on file",
                OrderDate = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // 6 — save order + deduct stock + clear cart
            await _unitOfWork.Orders.AddAsync(order);

            foreach (var item in cart.Items.ToList())
            {
                item.Product.Stock -= item.Quantity;
                _unitOfWork.CartItems.Delete(item);
            }

            await _unitOfWork.SaveChangesAsync();

            return MapToDto(order);
        }

        public async Task<OrderResponseDto> GetOrderByIdAsync(int orderId, int userId)
        {
            var buyer = await _unitOfWork.Users.GetBuyerByUserIdAsync(userId);
            if (buyer == null)
                throw new Exception("User needs to login");

            var order = await _unitOfWork.Orders.GetOrderWithItemsAsync(orderId);
            if (order == null)
                throw new Exception("Order not found");

            if (order.BuyerId != buyer.Id)
                throw new UnauthorizedAccessException("You don't have access to this order");

            return MapToDto(order);
        }

        public async Task<IEnumerable<OrderResponseDto>> GetOrderHistoryAsync(int userId)
        {
            var buyer = await _unitOfWork.Users.GetBuyerByUserIdAsync(userId);
            if (buyer == null)
                throw new Exception("User needs to login");

            var orders = await _unitOfWork.Orders.GetOrdersByBuyerIdAsync(buyer.Id);
            return orders.Select(MapToDto);
        }

        public async Task<OrderResponseDto> CancelOrderAsync(int orderId, int userId)
        {
            var buyer = await _unitOfWork.Users.GetBuyerByUserIdAsync(userId);
            if (buyer == null)
                throw new Exception("User needs to login");

            var order = await _unitOfWork.Orders.GetOrderWithItemsAsync(orderId);
            if (order == null)
                throw new Exception("Order not found");

            if (order.BuyerId != buyer.Id)
                throw new UnauthorizedAccessException("You don't have access to this order");

            if (order.Status != OrderStatus.Pending)
                throw new Exception($"Cannot cancel an order with status {order.Status}");

            order.Status = OrderStatus.Cancelled;
            order.UpdatedAt = DateTime.UtcNow;

            // Refund if already paid
            if (order.PaymentStatus == PaymentStatus.Paid)
            {
                order.PaymentStatus = PaymentStatus.Refunded;
                // TODO: trigger actual refund to Stripe/Paymob
            }

            // Restore stock
            foreach (var item in order.Items.ToList())
            {
                item.Product.Stock += item.Quantity;
            }

            _unitOfWork.Orders.Update(order);
            await _unitOfWork.SaveChangesAsync();
            return MapToDto(order);
        }

        // — generate unique order number
        private string GenerateOrderNumber()
        {
            var datePart = DateTime.UtcNow.ToString("yyyyMMdd");
            var randomPart = new Random().Next(1000, 9999);
            return $"ORD-{datePart}-{randomPart}";
        }

        // — mapper
        private OrderResponseDto MapToDto(Order order) => new OrderResponseDto
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            Status = order.Status,
            PaymentProvider = order.PaymentProvider,
            TotalPrice = order.TotalPrice,
            OrderDate = order.OrderDate,
            ShippingAddress = order.ShippingAddress,
            Items = order.Items.Select(i => new OrderItemResponseDto
            {
                ProductName = i.Product.Name,
                Quantity = i.Quantity,
                PricePerUnit = i.PricePerUnit,
                TotalPrice = i.PricePerUnit * i.Quantity,
                ProductImageUrl = i.Product.Images
                    ?.OrderBy(img => img.DisplayOrder)
                    .FirstOrDefault()?.ImageUrl
            }).ToList()
        };
    }
}
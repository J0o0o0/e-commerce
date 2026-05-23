using e_commerce.DTOs.OrderDTOs;
using e_commerce.Models;
using e_commerce.Repositories.Interfaces;
using e_commerce.Services.Interfaces;
using System.Threading.Tasks;

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

            if (order.Status == OrderStatus.Cancelled)
                throw new Exception("Order is already cancelled.");

            if (order.Status != OrderStatus.Pending)
                throw new Exception($"Cannot cancel an order with status {order.Status}");

            order.Status = OrderStatus.Cancelled;
            order.UpdatedAt = DateTime.UtcNow;


            // Cancel all items + restore stock + refund if paid
            foreach (var item in order.Items.Where(i => i.Status != OrderItemStatus.Cancelled).ToList())
            {
                item.Status = OrderItemStatus.Cancelled;
                HandleItemCancellation(item);
            }

            _unitOfWork.Orders.Update(order);
            await _unitOfWork.SaveChangesAsync();
            return MapToDto(order);
        }

        // ========================
        // SELLER METHODS
        // ========================

        public async Task<PaginatedResponse<SellerOrderResponseDto>> GetSellerOrdersAsync(int userId, int page, int pageSize, int? statusFilter)
        {
            var seller = await _unitOfWork.Users.GetSellerByUserIdAsync(userId);
            if (seller == null)
                throw new Exception("Only sellers can access this.");

            var status = statusFilter.HasValue ? (OrderItemStatus)statusFilter.Value : (OrderItemStatus?)null;
            var orders = await _unitOfWork.Orders.GetSellerOrdersAsync(seller.Id, page, pageSize, status);
            //var totalCount = await _unitOfWork.Orders.GetSellerOrdersCountAsync(seller.Id, status);

            var dtoList = orders.Select(o => MapToSellerDto(o, seller.Id)).ToList();

            return new PaginatedResponse<SellerOrderResponseDto>
            {
                Data = dtoList,
                Page = page,
                PageSize = pageSize,
                TotalCount = dtoList.Count,
                TotalPages = (int)Math.Ceiling((double)dtoList.Count / pageSize),
                HasNext = page * pageSize < dtoList.Count,
                HasPrevious = page > 1
            };
        }

        public async Task<SellerOrderResponseDto> GetSellerOrderDetailAsync(int orderId, int userId)
        {
            var seller = await _unitOfWork.Users.GetSellerByUserIdAsync(userId);
            if (seller == null)
                throw new Exception("Only sellers can access this.");

            var order = await _unitOfWork.Orders.GetSellerOrderDetailAsync(orderId, seller.Id);
            if (order == null)
                throw new Exception("Order not found or you don't have products in this order.");

            return MapToSellerDto(order, seller.Id);
        }

        public async Task<SellerOrderItemDto> UpdateItemStatusAsync(int orderItemId, int userId, UpdateItemStatusDto dto)
        {
            var seller = await _unitOfWork.Users.GetSellerByUserIdAsync(userId);
            if (seller == null)
                throw new Exception("Only sellers can access this.");

            var orderItem = await _unitOfWork.Orders.GetOrderItemByIdAsync(orderItemId);
            if (orderItem == null)
                throw new Exception("Order item not found.");

            // Verify this product belongs to this seller
            if (orderItem.Product.SellerId != seller.Id)
                throw new UnauthorizedAccessException("This product doesn't belong to you.");

            // Validate status transitions
            ValidateStatusTransition(orderItem.Status, dto.NewStatus);

            var previousStatus = orderItem.Status;
            orderItem.Status = dto.NewStatus;
            orderItem.Order.UpdatedAt = DateTime.UtcNow;

            // If seller cancels this item — handle refund
            if (dto.NewStatus == OrderItemStatus.Cancelled && previousStatus != OrderItemStatus.Cancelled)
            {
                HandleItemCancellation(orderItem);
            }

            // Auto-update order status based on ALL item statuses
            var order = await _unitOfWork.Orders.GetOrderWithItemsAsync(orderItem.OrderId);
            UpdateOrderStatus(order);

            _unitOfWork.Orders.Update(orderItem.Order);
            await _unitOfWork.SaveChangesAsync();

            return MapToSellerItemDto(orderItem);
        }

        // ========================
        // PRIVATE HELPERS
        // ========================
        private void HandleItemCancellation(OrderItem orderItem)
        {
            var order = orderItem.Order;

            // 1. Restore stock
            orderItem.Product.Stock += orderItem.Quantity;
            // 2. Deduct cancelled item amount from order total
            if (order.PaymentStatus == PaymentStatus.Pending)
            {
                order.TotalPrice -= orderItem.TotalPrice;

                // Safety: don't let total go below 0
                if (order.TotalPrice < 0)
                    order.TotalPrice = 0;
            }
            // 3. Handle refund based on payment status
            if (order.PaymentStatus == PaymentStatus.Paid)
            {
                // TODO: Integrate actual refund with Stripe/Paymob
                // For now, mark as refund pending
                order.PaymentStatus = PaymentStatus.Refunded;
                // Later you would do something like:
                // await _stripeService.CreateRefundAsync(order.PaymentIntentId, orderItem.TotalPrice);
                // await _paymobService.RefundAsync(order.PaymobOrderId, orderItem.TotalPrice);
            }
            // If CashOnDelivery and not paid yet → just adjust total, no actual refund needed
        }
        private string GenerateOrderNumber()
        {
            var datePart = DateTime.UtcNow.ToString("yyyyMMdd");
            var randomPart = new Random().Next(1000, 9999);
            return $"ORD-{datePart}-{randomPart}";
        }

        private void ValidateStatusTransition(OrderItemStatus current, OrderItemStatus next)
        {
            // Define allowed transitions
            var allowed = new Dictionary<OrderItemStatus, List<OrderItemStatus>>
            {
                [OrderItemStatus.Pending] = new() { OrderItemStatus.Approved, OrderItemStatus.Cancelled },
                [OrderItemStatus.Approved] = new() { OrderItemStatus.Processing, OrderItemStatus.Cancelled },
                [OrderItemStatus.Processing] = new() { OrderItemStatus.Shipped },
                [OrderItemStatus.Shipped] = new() { OrderItemStatus.Delivered },
                [OrderItemStatus.Shipped] = new() { OrderItemStatus.OutForDelivery },
                [OrderItemStatus.OutForDelivery] = new() { OrderItemStatus.Delivered },
                [OrderItemStatus.Delivered] = new() { },
                [OrderItemStatus.Cancelled] = new() { }
            };

            if (!allowed.ContainsKey(current) || !allowed[current].Contains(next))
                throw new Exception($"Cannot change status from {current} to {next}.");
        }

        private async Task UpdateOrderStatus(Order order)
        {
            
            var items = order.Items;

            // If ANY item is cancelled by seller, leave order handling to buyer/system
            if (items.All(i => i.Status == OrderItemStatus.Cancelled))
            {
                order.Status = OrderStatus.Cancelled;
                return;
            }

            // Find the "lowest" status among active (non-cancelled) items
            var activeItems = items.Where(i => i.Status != OrderItemStatus.Cancelled).ToList();

            if (activeItems.All(i => i.Status == OrderItemStatus.Pending))
            {
                order.Status = OrderStatus.Pending;
            }
            else if (activeItems.All(i => i.Status >= OrderItemStatus.Delivered))
            {
                order.Status = OrderStatus.Delivered;
            }
            else if (activeItems.All(i => i.Status >= OrderItemStatus.OutForDelivery))
            {
                order.Status = OrderStatus.OutForDelivery;
            }
            else if (activeItems.All(i => i.Status >= OrderItemStatus.Shipped ))
            {
                order.Status = OrderStatus.Shipped;
            }
            else if (activeItems.All(i => i.Status >= OrderItemStatus.Approved ))
            {
                order.Status = OrderStatus.Processing;
            }
        }

        // Buyer mapper
        private OrderResponseDto MapToDto(Order order) => new OrderResponseDto
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            Status = order.Status,
            PaymentStatus = order.PaymentStatus,
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

        // Seller mapper
        private SellerOrderResponseDto MapToSellerDto(Order order, int sellerId)
        {
            var myItems = order.Items
                .Where(i => i.Product.SellerId == sellerId)
                .Select(i => MapToSellerItemDto(i))
                .ToList();

            return new SellerOrderResponseDto
            {
                OrderId = order.Id,
                OrderNumber = order.OrderNumber,
                OrderStatus = order.Status,
                PaymentStatus = order.PaymentStatus,
                PaymentProvider = order.PaymentProvider,    
                OrderTotalPrice = order.TotalPrice,
                MyItemsTotal = myItems.Where(i => i.Status != OrderItemStatus.Cancelled).Sum(i => i.TotalPrice),
                OrderDate = order.OrderDate,
                TotalItemsInOrder = order.Items.Count,
                MyItemsCount = myItems.Count,
                BuyerName = order.Buyer?.User?.UserName ?? "Unknown",
                BuyerEmail = order.Buyer?.User?.Email ?? "Unknown",
                BuyerPhoneNumber = order.Buyer?.User?.PhoneNumber ?? "Unknown",
                ShippingAddress = order.ShippingAddress,
                MyItems = myItems
            };
        }

        private SellerOrderItemDto MapToSellerItemDto(OrderItem item) => new SellerOrderItemDto
        {
            OrderItemId = item.Id,
            ProductId = item.ProductId,
            ProductName = item.Product.Name,
            ProductImageUrl = item.Product.Images
                ?.OrderBy(img => img.DisplayOrder)
                .FirstOrDefault()?.ImageUrl,
            Quantity = item.Quantity,
            PricePerUnit = item.PricePerUnit,
            TotalPrice = item.PricePerUnit * item.Quantity,
            Status = item.Status
        };
    }
}
using e_commerce.DTOs.OrderDTOs;
using e_commerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using e_commerce.Extensions;

namespace e_commerce.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // POST api/order/place
        [HttpPost("place")]
        public async Task<IActionResult> PlaceOrder([FromBody] PlaceOrderDto dto)
        {
            try
            {
                var userId = User.GetUserId();
                var order = await _orderService.PlaceOrderAsync(userId, dto);
                return Ok(order);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET api/order/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            try
            {
                var userId = User.GetUserId();
                var order = await _orderService.GetOrderByIdAsync(id, userId);
                return Ok(order);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        // GET api/order/history
        [HttpGet("history")]
        public async Task<IActionResult> GetOrderHistory()
        {
            try
            {
                var userId = User.GetUserId();
                var orders = await _orderService.GetOrderHistoryAsync(userId);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST api/order/5/cancel
        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            try
            {
                var userId = User.GetUserId();
                var order = await _orderService.CancelOrderAsync(id, userId);
                return Ok(order);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        // ========================
        // SELLER ENDPOINTS
        // ========================

        // GET api/order/seller?page=1&pageSize=10&status=1
        [HttpGet("seller")]
        public async Task<IActionResult> GetSellerOrders(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] int? status = null)
        {
            try
            {
                var userId = User.GetUserId();
                var orders = await _orderService.GetSellerOrdersAsync(userId, page, pageSize, status);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET api/order/seller/5
        [HttpGet("seller/{id}")]
        public async Task<IActionResult> GetSellerOrderDetail(int orederId)
        {
            try
            {
                var userId = User.GetUserId();
                var order = await _orderService.GetSellerOrderDetailAsync(orederId, userId);
                return Ok(order);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        // PUT api/order/seller/item/10/status
        [HttpPut("seller/item/{orderItemId}/status")]
        public async Task<IActionResult> UpdateItemStatus(
            int orderItemId,
            [FromBody] UpdateItemStatusDto dto)
        {
            try
            {
                var userId = User.GetUserId();
                var item = await _orderService.UpdateItemStatusAsync(orderItemId, userId, dto);
                return Ok(item);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
using Common;
using Common.Events;
using Common.Events.Dtos;
using Common.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderService.Controllers.Dtos;
using OrderService.Data;

namespace OrderService.Controllers;

[ApiController]
[Route("[controller]")]
public class OrdersController : ControllerBase
{

    private readonly ILogger<OrdersController> _logger;
    private readonly IMessageQueueService _rabbitMqService;
    private readonly OrderDbContext _dbContext;
    public OrdersController(OrderDbContext dbContext, IMessageQueueService rabbitMqService, ILogger<OrdersController> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
        _rabbitMqService = rabbitMqService;
    }


    [HttpGet]
    [Route("orders")]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders()
    {
        return await _dbContext.Orders.Select(x => new OrderDto(x)).ToListAsync();
    }



    [HttpPost]
    [Route("orders")]
    public async Task<IActionResult> CreateOrder([FromBody] OrderDto dto)
    {

        var dbProduct = await _dbContext.Products.FirstOrDefaultAsync(x => x.ProductId == dto.ProductId && x.Quantity >= dto.NumberOfItems);

        // Validate the incoming DTO
        if (dbProduct == null)
        {
            string errorMessage = $"Product with ID {dto.ProductId} not found or insufficient quantity.";
            _logger.LogError(errorMessage);
            return BadRequest(errorMessage);
        }

        dbProduct.Quantity -= dto.NumberOfItems;

        await _dbContext.Orders.AddAsync(new()
        {
            CustomerName = dto.CustomerName,
            NumberOfItems = dto.NumberOfItems,
            ProductId = dto.ProductId
        });
        await _dbContext.SaveChangesAsync();


        OrderCreatedEventDto eventData = new OrderCreatedEventDto
        {
            NumberOfItems = dto.NumberOfItems,
            ProductId = dto.ProductId
        };

        await new OrderCreatedEvent(eventData, _rabbitMqService).PublishAsync();


        return Ok();
    }
}

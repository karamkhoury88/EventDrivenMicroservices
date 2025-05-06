using Common;
using Common.Events;
using Common.Events.Dtos;
using Common.Services;
using InventoryService.Controllers.Dtos;
using InventoryService.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductsController : ControllerBase
{

    private readonly ILogger<ProductsController> _logger;
    private readonly IMessageQueueService _rabbitMqService;
    private readonly InventoryDbContext _dbContext;
    public ProductsController(InventoryDbContext dbContext, ILogger<ProductsController> logger, IMessageQueueService rabbitMqService)
    {
        _dbContext = dbContext;
        _logger = logger;
        _rabbitMqService = rabbitMqService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
    {
        return await _dbContext.Products.Select(x => new ProductDto(x)).ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<ProductDto>> CreateProduct([FromBody] ProductDto dto)
    {
        Product dbProduct = (await _dbContext.Products.AddAsync(new Product(dto.Name, dto.Quantity))).Entity;
        await _dbContext.SaveChangesAsync();

        ProductCreatedEventDto eventData = new ProductCreatedEventDto
        {
            Name = dbProduct.Name,
            ProductId = dbProduct.ProductId,
            Quantity = dbProduct.Quantity
        };

        await new ProductCreatedEvent(eventData, _rabbitMqService).PublishAsync();

        return CreatedAtAction(nameof(GetProducts), new { id = dbProduct.Id }, dto);
    }
}

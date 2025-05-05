using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderService.Controllers.Dtos;
using OrderService.Data;

namespace OrderService.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductsController : ControllerBase
{

    private readonly ILogger<ProductsController> _logger;
    private readonly OrderDbContext _dbContext;
    public ProductsController(OrderDbContext dbContext, ILogger<ProductsController> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }


    [HttpGet]
    [Route("products")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
    {
        return await _dbContext.Products.Select(x => new ProductDto(x)).ToListAsync();
    }

}

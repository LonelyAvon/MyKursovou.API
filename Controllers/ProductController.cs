using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyKursovoy.Domain.Context;
using MyKursovoy.Domain.Models;

namespace MyKursovoy.API.Controllers;
[ApiController]
[Route("[controller]")]
public class ProductController : ControllerBase
{
    private KursovoyContext _db;
    public ProductController(KursovoyContext db)
    {
        _db = db;
    }
    [HttpGet]
    [Route("GetQuantity")]
    public async Task<ActionResult<Product>> Get(CancellationToken ct)
    {
        var productList = await _db.Products.Skip(5).Take(5).ToListAsync(ct);
            
        return Ok(productList);
    }
    [HttpGet]
    [Route("GetAll")]
    public async Task<ActionResult<List<ProductCombiner>>> GetAll(CancellationToken ct)
    {
        var productList = await _db.Products.Include(x=>x.TypeoftechnicNavigation).ToListAsync(ct);

        var productsDTO = productList.Select(x =>
        {
            var p = x;
            string ex = "";
            if (Convert.ToBoolean(p.Existance))
                ex = "В наличии";
            else
                ex = "Нет в наличии";
            return new ProductCombiner
            {
                Id = p.Id,
                Cost = p.Cost,
                Brandname = p.Brandname,
                Description = p.Description,
                Existance = ex,
                Name = p.Name,
                Photoname = p.Photoname,
                ShortDescription = p.ShortDescription,
                Typeoftechnic = p.Typeoftechnic,
                Type = p.TypeoftechnicNavigation.Name,
            };
        });  
        
        return Ok(productsDTO);
    }
    [HttpGet]
    [Route("GetCart/{id:guid}")]
    public async Task<ActionResult<Product>> GetCart(string id,CancellationToken ct)
    {
        var findOrder = await _db.Orders.Where(x => x.IdClient == id).FirstOrDefaultAsync();

        if (findOrder is null)
        {
            var newOrder = new Order
            {
                Dateorder = DateOnly.FromDateTime(DateTime.Now),
                IdClient = id,
            };

            await _db.Orders.AddAsync(newOrder);
            await _db.SaveChangesAsync();
            return Ok(null);
        }

        var productOrders = await _db.Productorders.Where(x => x.IdOrder == findOrder.Id)
            .Include(x => x.IdProductNavigation).ToListAsync();
        var products = productOrders.Select(x=>
        {
            var p = x.IdProductNavigation;
            string ex = "";
            if (Convert.ToBoolean(p.Existance))
                ex = "В наличии";
            else
                ex = "Не в наличии";
            return new ProductCombiner
            {
                Id = p.Id,
                Cost = p.Cost,
                Brandname = p.Brandname,
                Description = p.Description,
                Existance = ex,
                Name = p.Name,
                Photoname = p.Photoname,
                ShortDescription = p.ShortDescription,
                Typeoftechnic = p.Typeoftechnic
            };
        });

        var productParser = new ProductParser
        {
            Id_Order = findOrder.Id,
            Products = products,
        };
        
        return Ok(productParser);
    }
}
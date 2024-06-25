using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyKursovoy.Domain.Context;
using MyKursovoy.Domain.Models;

namespace MyKursovoy.API.Controllers;

[ApiController]
[Route("[controller]")]
public class CartController : ControllerBase
{
    private KursovoyContext _db;
    public CartController(KursovoyContext db)
    {
        _db = db;
    }
    [HttpPost]
    [Route("AddToCart")]
    public async Task<ActionResult<bool>> Create(CartCreateDTO cart, CancellationToken ct)
    {
        var findOrder = await _db.Orders.Where(x=> x.IdClient == cart.Guid).FirstOrDefaultAsync();
        if (findOrder is null)
        {
            var newOrder = new Order
            {
                Dateorder = DateOnly.FromDateTime(DateTime.Now),
                IdClient = cart.Guid,
            };

            await _db.Orders.AddAsync(newOrder);
            await _db.SaveChangesAsync();

            var productOrder = new Productorder
            {
                IdOrder = newOrder.Id,
                IdProduct = cart.IdProduct,
            };
            await _db.Productorders.AddAsync(productOrder);
            await _db.SaveChangesAsync();
            return Ok(true);
        }
        else
        {
            var productOrder = new Productorder
            {
                IdOrder = findOrder.Id,
                IdProduct = cart.IdProduct,
            };
            await _db.Productorders.AddAsync(productOrder);
            await _db.SaveChangesAsync();
            return Ok(true);
        }
    }

    [HttpDelete]
    [Route("DeleteAllInCart")]
    public async Task<ActionResult<int>> Delete(string ClientId)
    {
        var findOrder = await _db.Orders.Where(x => x.IdClient == ClientId).FirstOrDefaultAsync();

        var deleted = await _db.Productorders.Where(x => x.IdOrder == findOrder.Id).ExecuteDeleteAsync();

        return deleted;
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyKursovoy.Domain.Context;
using MyKursovoy.Domain.Models;

namespace MyKursovoy.API.Controllers;

[ApiController]
[Route("[controller]")]
public class TypesController : ControllerBase
{
    private KursovoyContext _db;
    public TypesController(KursovoyContext db)
    {
        _db = db;
    }
    [HttpGet]
    [Route("GetTypes")]
    public async Task<ActionResult<List<Typeoftechnic>>> GetTypes(CancellationToken ct)
    {
       return Ok(await _db.Typeoftechnics.ToListAsync(ct));
    }
}
using Microsoft.AspNetCore.Mvc;
using MyKursovoy.Domain.Context;
using MyKursovoy.Domain.Models;

namespace MyKursovoy.API.Controllers;

[ApiController]
[Route("[controller]")]
public class ClientController : ControllerBase
{
      private KursovoyContext _db;
      public ClientController(KursovoyContext context)
      {
       _db = context;
      }

      [HttpPost]
      [Route("Create/{id:guid}")]
      public async Task<ActionResult<Client>> Create(string id, CancellationToken ct)
      {
          var findUser = await _db.Clients.FindAsync(id.ToString());
          if (findUser != null)
          {
              return Ok(findUser);
          }

          var client = new Client
          {
              Id = id.ToString(),
          };
          await _db.Clients.AddAsync(client, ct);
          await _db.SaveChangesAsync(ct);
          return Ok(client);
      }
      [HttpGet]
      [Route("GetById/{id:guid}")]
      public async Task<ActionResult<Client>> GetById(string id)
      {
          var user = await _db.Clients.FindAsync(id);
          return user;
      }
      [HttpPost]
      [Route("UpdateUser")]
      public async Task<ActionResult<Client>> Update(Client client)
      {
          var user = await _db.Clients.FindAsync(client.Id);
          user.Surname = client.Surname;
          user.Name = client.Name;
          user.Patronymic = client.Patronymic;
          user.Phone = client.Phone;
          await _db.SaveChangesAsync();
          
          return user;
      }
}
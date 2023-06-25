using Microsoft.AspNetCore.Mvc;
using WarOfMinds.Common.DTO;
using WarOfMinds.Services.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WarOfMinds.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerController : ControllerBase
    {
        private readonly IPlayerService _playerService;
        public PlayerController(IPlayerService playerService)
        {
            _playerService = playerService;
        }
    
        // GET: api/<PlayerController>
        [HttpGet]
        public Task<List<PlayerDTO>> Get()
        {
            return _playerService.GetAllAsync();
        }

        // GET api/<PlayerController>/5
        [HttpGet("{id}")]
        public async Task<PlayerDTO> Get(int id)
        {
            PlayerDTO p=await _playerService.GetByIdAsync(id);
            p.PlayerPassword = "";
            return p;
        }
        
        // GET api/<PlayerController>/{email}/{password}
        [HttpGet("{email}/{password}")]
        public Task<PlayerDTO> Get(string email,string password)
        {
            return _playerService.GetByEmailAndPassword(email,password);
        }

        // GET api/<PlayerController>/GetHistory/{id}
        [HttpGet("GetHistory/{id}")]
        public Task<List<GameDTO>> GetHistory(int id)
        {
            return _playerService.GetHistory(id);
        }

        // POST api/<PlayerController>
        [HttpPost]
        public Task<PlayerDTO> Post([FromBody] PlayerDTO player)
        {
            return _playerService.AddAsync(player);
        }

        // PUT api/<PlayerController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<PlayerController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}

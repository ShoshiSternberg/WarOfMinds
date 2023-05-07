using Microsoft.AspNetCore.Mvc;
using WarOfMinds.Common.DTO;
using WarOfMinds.Services.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WarOfMinds.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly IGameService _gameService;

        public GameController(IGameService gameService)
        {
            _gameService = gameService;
        }

    
        // GET: api/<GameController>
        [HttpGet]
        public async Task<List<GameDTO>> Get()
        {
             return await _gameService.GetAllAsync();
            
        }

        // GET api/<GameController>/5
        [HttpGet("{id}")]
        public async Task<GameDTO> Get(int id)
        {
            return await _gameService.GetWholeByIdAsync(id);
        }

        // POST api/<GameController>
        [HttpPost]
        public async Task<GameDTO> Post([FromBody] GameDTO game)
        {
            return await _gameService.AddGameAsync(game);
        }

        // PUT api/<GameController>/5
        [HttpPut("{id}")]
        public Task<GameDTO> Put(int id, [FromBody] GameDTO game)
        {
            return _gameService.UpdateGameAsync(id,game);
        }

        // DELETE api/<GameController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}

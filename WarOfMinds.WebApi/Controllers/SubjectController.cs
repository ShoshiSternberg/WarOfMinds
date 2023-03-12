using Microsoft.AspNetCore.Mvc;
using WarOfMinds.Common.DTO;
using WarOfMinds.Services.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WarOfMinds.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectController : ControllerBase
    {
        private readonly ISubjectService _subjectService;
        public SubjectController(ISubjectService subjectService)
        {
            _subjectService= subjectService;
        }
        // GET: api/<SubjectController>
        [HttpGet]
        public Task<List<SubjectDTO>> Get()
        {
            return _subjectService.GetAllAsync();
        }

        // GET api/<SubjectController>/5
        [HttpGet("{id}")]
        public async Task<SubjectDTO> Get(int id)
        {
            return await _subjectService.GetByIdAsync(id);
        }

        // POST api/<SubjectController>
        [HttpPost]
        public Task<SubjectDTO> Post([FromBody] SubjectDTO subjectDTO)
        {
            return _subjectService.AddAsync(subjectDTO);
        }

        // PUT api/<SubjectController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<SubjectController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}

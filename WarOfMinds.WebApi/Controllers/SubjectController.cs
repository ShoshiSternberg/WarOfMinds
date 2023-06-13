using Microsoft.AspNetCore.Mvc;
using RestSharp;
using System.Text.Json;
using WarOfMinds.Common.DTO;
using WarOfMinds.Services.Interfaces;
using WarOfMinds.Services.Services;

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
            _subjectService = subjectService;
        }
        // GET: api/<SubjectController>
        [HttpGet]
        public async Task<List<SubjectDTO>> Get()
        {
            if (DateTime.Now.Day == 13)
            {
                var client = new RestClient("https://opentdb.com/api_category.php");
                var request = new RestRequest("", Method.Get);
                RestResponse response = await client.ExecuteAsync(request);

                string jsonString = response.Content;
                //המרה מג'יסון לאובייקט שאלה
                SubjectsRoot subjects =
                    JsonSerializer.Deserialize<SubjectsRoot>(jsonString);
                foreach (subjectAPI subject in subjects.trivia_categories)
                {
                    SubjectDTO subjectDTO = new SubjectDTO { SubjectID = subject.id, Subjectname = subject.name };
                    await _subjectService.AddAsync(subjectDTO);
                }

            }
            return await _subjectService.GetAllAsync();
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
        public Task<SubjectDTO> Put([FromBody] SubjectDTO value)
        {
            return _subjectService.UpdateAsync(value);
        }

        // DELETE api/<SubjectController>/5
        [HttpDelete("{id}")]
        public Task Delete(int id)
        {
            return _subjectService.DeleteByIdAsync(id);
        }
    }
}

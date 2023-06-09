using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace dotnet_rpg.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CharacterController : ControllerBase
    {
        private readonly ICharacterService _characterService;
        public CharacterController(ICharacterService characterService)
        {
            _characterService = characterService;
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult<ServiceResponse<List<GetCharacterDto>> >> Get()
        {
            return Ok(await _characterService.GetAllCharacters());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceResponse<GetCharacterDto>>> GetSingle(int id)
        {
            return Ok(await _characterService.GetCharacterById(id));
        }

        [HttpPost]
        public async Task<ActionResult<ServiceResponse<List<GetCharacterDto>>>> AddCharacter(AddCharacterDto character)
        {
            return Ok(await _characterService.AddCharacter(character));
        }

        [HttpPut]
        public async Task<ActionResult<ServiceResponse<GetCharacterDto>>> UpdateCharacter(UpdateCharacterDto character)
        {
            var response = await _characterService.UpdateCharacter(character);
            if(response.Data is null) {
                return NotFound(response);
            }
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ServiceResponse<GetCharacterDto>>> deleteCharacter(int id)
        {
            var response = await _characterService.DeleteCharacter(id);
            if(response.Data is null) {
                return NotFound(response);
            }
            return Ok(response);
        }

         [HttpPost("/Skill")]
        public async Task<ActionResult<ServiceResponse<List<GetCharacterDto>>>> AddCharacterSkill(AddCharacterSkillDto newCharacterSkill)
        {
            return Ok(await _characterService.AddCharacterSkill(newCharacterSkill));
        }

    }
}


using System.Security.Claims;

namespace dotnet_rpg.Services.CharacterService
{
    public class CharacterService : ICharacterService
    {

        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CharacterService(IMapper mapper, DataContext context, IHttpContextAccessor httpContextAccessor)
        {
            _mapper = mapper;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        private int GetUserId() => int.Parse(_httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        public async Task<ServiceResponse<List<GetCharacterDto>>> AddCharacter(AddCharacterDto newCharacter)
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();
            var character = _mapper.Map<Character>(newCharacter);
            character.User = await _context.Users.FirstOrDefaultAsync(u => u.Id == GetUserId());
            _context.Characters.Add(character);
            await _context.SaveChangesAsync();
            serviceResponse.Data = await _context.Characters
                .Where(c => c.User!.Id == GetUserId())
                .Select(c => _mapper.Map<GetCharacterDto>(c))
                .ToListAsync();
            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> DeleteCharacter(int id)
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();
            try
            {
                var character = await _context.Characters.FirstOrDefaultAsync(c => c.Id == id && c.User!.Id == GetUserId());
                if (character is null)
                {
                    throw new Exception($"Character with Id '{id}' not found");
                }
                _context.Characters.Remove(character);
                await _context.SaveChangesAsync();
                serviceResponse.Data = await _context.Characters.Select(c => _mapper.Map<GetCharacterDto>(c)).ToListAsync();
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacters()
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();
            var dbCharacters = await _context.Characters.Where(c => c.User!.Id == GetUserId()).ToListAsync();
            serviceResponse.Data = dbCharacters.Select(c => _mapper.Map<GetCharacterDto>(c)).ToList();
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCharacterDto>> GetCharacterById(int id)
        {
            var serviceResponse = new ServiceResponse<GetCharacterDto>();
            var dbCharacter = await _context.Characters.FirstOrDefaultAsync(c => c.Id == id && c.User!.Id == GetUserId());
            serviceResponse.Data = _mapper.Map<GetCharacterDto>(dbCharacter);
            return serviceResponse;

        }

        public async Task<ServiceResponse<GetCharacterDto>> UpdateCharacter(UpdateCharacterDto updateCharacter)
        {
            var serviceResponse = new ServiceResponse<GetCharacterDto>();
            try
            {
                var c = await _context.Characters.Include(c => c.User).FirstOrDefaultAsync(c => c.Id == updateCharacter.Id);
                if (c is null && c!.User!.Id == GetUserId())
                {
                    throw new Exception($"Character with Id '{updateCharacter.Id}' not found");
                }
                c.Name = updateCharacter.Name;
                c.HitPoints = updateCharacter.HitPoints;
                c.Strength = updateCharacter.Strength;
                c.Defence = updateCharacter.Defence;
                c.Intelligence = updateCharacter.Intelligence;
                c.Class = updateCharacter.Class;
                await _context.SaveChangesAsync();
                serviceResponse.Data = _mapper.Map<GetCharacterDto>(c);
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }

            return serviceResponse;
        }
    }
}
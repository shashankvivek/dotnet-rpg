namespace dotnet_rpg.Services
{
    public class CharacterService : ICharacterService
    {
        private static List<Character> characters = new List<Character>{
            new Character(),
            new Character{Id = 1,Name = "Sam" }
         };

        private readonly IMapper _mapper;

        public CharacterService(IMapper mapper)
        {
            _mapper = mapper;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> AddCharacter(AddCharacterDto newCharacter)
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();
            characters.Add(_mapper.Map<Character>(newCharacter));
            serviceResponse.Data = characters.Select(c => _mapper.Map<GetCharacterDto>(c)).ToList();
            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> DeleteCharacter(int id)
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();
            try
            {
                var characterIndex = characters.First(c => c.Id == id);
                if (characterIndex is null)
                {
                    throw new Exception($"Character with Id '{id}' not found");
                }
                characters.Remove(characterIndex);
                serviceResponse.Data = characters.Select(c => _mapper.Map<GetCharacterDto>(c)).ToList();
            }
            catch (Exception ex)
            {
                serviceResponse.success = false;
                serviceResponse.Message = ex.Message;
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacters()
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();
            serviceResponse.Data = characters.Select(c => _mapper.Map<GetCharacterDto>(c)).ToList();
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCharacterDto>> GetCharacterById(int id)
        {
            var c = characters.FirstOrDefault(c => c.Id == id);
            var serviceResponse = new ServiceResponse<GetCharacterDto>();
            serviceResponse.Data = _mapper.Map<GetCharacterDto>(c);
            return serviceResponse;

        }

        public async Task<ServiceResponse<GetCharacterDto>> UpdateCharacter(UpdateCharacterDto updateCharacter)
        {
            var serviceResponse = new ServiceResponse<GetCharacterDto>();
            try
            {
                var c = characters.FirstOrDefault(c => c.Id == updateCharacter.Id);
                if (c is null)
                {
                    throw new Exception($"Character with Id '{updateCharacter.Id}' not found");
                }
                c.Name = updateCharacter.Name;
                c.HitPoints = updateCharacter.HitPoints;
                c.Strength = updateCharacter.Strength;
                c.Defence = updateCharacter.Defence;
                c.Intelligence = updateCharacter.Intelligence;
                c.Class = updateCharacter.Class;
                serviceResponse.Data = _mapper.Map<GetCharacterDto>(c);
            }
            catch (Exception ex)
            {
                serviceResponse.success = false;
                serviceResponse.Message = ex.Message;
            }

            return serviceResponse;
        }
    }
}
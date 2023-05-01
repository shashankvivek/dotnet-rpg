using System.Security.Claims;

namespace dotnet_rpg.Services.WeaponService
{
    public class WeaponService : IWeaponService
    {
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        public WeaponService(DataContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        private int GetUserId() => int.Parse(_httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        public async Task<ServiceResponse<GetCharacterDto>> AddWeapon(AddWeaponDto newWeapon)
        {
            var response = new ServiceResponse<GetCharacterDto>();

            try 
            {
                var character = await _context.Characters.FirstOrDefaultAsync(c => 
                        c.Id == newWeapon.CharacterId && c!.User!.Id == GetUserId());
                
                if(character is null)
                {
                    response.Success = false;
                    response.Message = "character not found";
                }
                
                var weapon = new Weapon {
                    Name = newWeapon.Name,
                    Damage = newWeapon.Damage,
                    Character = character
                };

                _context.Weapons.Add(weapon);
                await _context.SaveChangesAsync();

                response.Data = _mapper.Map<GetCharacterDto>(character);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }
    }
}
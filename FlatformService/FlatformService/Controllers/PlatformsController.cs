using AutoMapper;
using FlatformService.Data;
using FlatformService.Dtos;
using FlatformService.Models;
using FlatformService.SyncDataServices.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlatformService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlatformsController : ControllerBase
    {
        private readonly IPlatformRepo _platformRepo;
        private readonly IMapper _mapper;
        private readonly ICommandDataClient _commandDataClient;

        public PlatformsController(IPlatformRepo platformRepo, 
            IMapper mapper,
            ICommandDataClient commandDataClient)
        {
            _platformRepo = platformRepo;
            _mapper = mapper;
            _commandDataClient = commandDataClient;
        }

        [HttpGet]
        [Route("GetPlatforms")]
        public IActionResult GetPlatforms()
        {
            Console.WriteLine("--> Getting Platforms....");

            var platformItems = _platformRepo.GetAllPlatforms();
            return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platformItems));
        }

        [HttpGet]
        [Route("GetPlatformById")]
        public IActionResult GetPlatformById([FromQuery]int id)
        {
            var item = _platformRepo.GetPlatformById(id);
            if(item == null)
                return NotFound();
            return Ok(_mapper.Map<PlatformReadDto>(item));
        } 

        [HttpPost]
        [Route("CreatePlatform")]
        public async Task<IActionResult> CreatePlatform([FromBody]PlatformCreateDto platformCreateDto)
        {
            var platformModel = _mapper.Map<Platform>(platformCreateDto);

            _platformRepo.CreatePlatform(platformModel);
            _platformRepo.SaveChanges();

            var platformReadDto = _mapper.Map<PlatformReadDto>(platformModel);

            try
            {
                await _commandDataClient.SendPlatformToCommand(platformReadDto);
            }
            catch(Exception ex)
            {
                Console.WriteLine($"--> Could not send synchronously: {ex.Message}");
            }
            return Ok(platformReadDto);
        }
    }
}

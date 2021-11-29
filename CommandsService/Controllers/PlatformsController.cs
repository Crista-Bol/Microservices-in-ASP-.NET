using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers
{
    [Route("api/c/[controller]")]
    [ApiController]
    public class PlatformsController:ControllerBase
    {
        private readonly ICommandRepo _repo;
        private readonly IMapper _mapper;

        public PlatformsController(ICommandRepo repo, IMapper mapper)
        {
            _repo=repo;
            _mapper=mapper;
        }

        public ActionResult<IEnumerable<PlatformReadDto>> GetAllPlatforms(){
            Console.WriteLine("--> Getting Platforms from CommandService");

            var platformItems=_repo.GetAllPlatforms();

            return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platformItems));
        }
        
        [HttpPost]
        public ActionResult TestInBoundConnection()
        {
            Console.WriteLine("--> InBound Command Service");
            return Ok("Inbound Test from Platform Contoller");
        }
    }
}
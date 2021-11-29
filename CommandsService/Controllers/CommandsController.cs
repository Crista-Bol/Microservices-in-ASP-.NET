using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers
{
    [Route("api/c/platforms/{platformId}/[controller]")]
    [ApiController]
    public class CommandsController:ControllerBase
    {
        private readonly ICommandRepo _repo;
        private readonly IMapper _mapper;

        public CommandsController(ICommandRepo repo,IMapper mapper)
        {
            _repo=repo;
            _mapper=mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<CommandReadDto>> GetAllCommands(int platformId)
        {
            Console.WriteLine("--> Getting all Platforms");

            if(!_repo.PlatformExist(platformId))
            {
                return NotFound();
            }

            var commands=_repo.GetCommandsForPlatform(platformId);
            return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(commands));
        }

        [HttpGet("{commandId}", Name = "GetCommandForPlatform")]
        public ActionResult<CommandReadDto> GetCommandForPlatform(int platformId,int commandId)
        {
            Console.WriteLine($"--> Hit Get Commands for Platform {platformId}/{commandId}");
            var plat=_repo.PlatformExist(platformId);
            
            if(plat==null){
                return NotFound();
            }

            var command=_repo.GetCommand(platformId,commandId);
            if(command==null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<CommandReadDto>(command));

        }

        [HttpPost]
        public ActionResult<CommandReadDto> CreateCommandForPlatform(int platformId, CreateCommandDto createComDto)
        {
            Console.WriteLine($"--> Hit Create Command For Platform {platformId}");
            Console.WriteLine("hey2");
            var plat=_repo.PlatformExist(platformId);

            if(plat==null)
            {
                return NotFound();
            }

            var command=_mapper.Map<Command>(createComDto);

            _repo.CreateCommand(platformId,command);
            _repo.SaveChanges();

            var comReadDto=_mapper.Map<CommandReadDto>(command);

            return CreatedAtRoute(nameof(GetCommandForPlatform), 
            new {platformId = platformId, commandId = comReadDto.Id}, comReadDto);

        }
    }
}
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;

namespace PlatformService.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    public class PlatformsController: ControllerBase
    {
        private readonly IPlatformRepo _repository;
        private readonly IMapper _mapper;

        private readonly ICommandDataClient _commandDataClient;

        private readonly IMessageBusClient _messageBusClient;

        public PlatformsController(IPlatformRepo repository, IMapper mapper, 
                    ICommandDataClient commandDataClient,
                    IMessageBusClient messageBusClient)
        {
            _repository=repository;
            _mapper=mapper;
            _commandDataClient=commandDataClient;
            _messageBusClient=messageBusClient;
        }

        [HttpGet]
        public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
        {
            Console.WriteLine("Getting Platforms");
            var paltformItems=_repository.GetAllPlatforms();
            return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(paltformItems));
        }

        [HttpGet("{Id}",Name="GetPlatformById")]
        public ActionResult<PlatformReadDto> GetPlatformById(int id)
        {
            var platformItem=_repository.GetPlatformById(id);

            if(platformItem!=null)
            {
                return Ok(_mapper.Map<PlatformReadDto>(platformItem));
            }else
            {
                return NotFound();
            }
        }


        [HttpPost]
        public async Task<ActionResult<PlatformReadDto>> CreatePlatform(PlatformCreateDto platformCreateDto)
        {

            var platformModel=_mapper.Map<Platform>(platformCreateDto);
            _repository.CreatePlatform(platformModel);
            _repository.saveChanges();

            var platformReadDto=_mapper.Map<PlatformReadDto>(platformModel);

            //Sending message syncronously
            try{
                await _commandDataClient.SendPlatformToCommand(platformReadDto);
            }catch(Exception ex)
            {
                Console.WriteLine("Coudn't send synchronously "+ex.Message);
            }
            //Sending message asyncronously
            try
            {
                var publishedDto=_mapper.Map<PlatformPublishedDto>(platformReadDto);
                publishedDto.Event="Published_Platform";
                _messageBusClient.PublishedNewPlatform(publishedDto);

            }catch(Exception ex)
            {
                Console.WriteLine("Coudn't send asynchronously "+ex.Message);
            }
            return CreatedAtRoute(nameof(GetPlatformById),new {Id = platformReadDto.Id},platformReadDto);
        }
    }
}
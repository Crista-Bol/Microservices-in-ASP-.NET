using AutoMapper;
using CommandsService.Dtos;
using CommandsService.Models;
using PlatformService;

namespace CommandsService.Profiles
{
    public class CommandsProfile:Profile
    {
        public CommandsProfile()
        {
           //Source->Target
            CreateMap<Platform, PlatformReadDto>(); 
            CreateMap<CreateCommandDto, Command>(); 
            CreateMap<Command,CommandReadDto>();
            CreateMap<PlatformPublishDto,Platform>()
                    .ForMember(dest =>dest.ExternalID,opt=>opt.MapFrom(src =>src.Id));
            CreateMap<GrpcPlatformModel,Platform>()
                    .ForMember(dest =>dest.ExternalID, opt=>opt.MapFrom(src =>src.PlatformId));
                    
            
        }
        

    }
}
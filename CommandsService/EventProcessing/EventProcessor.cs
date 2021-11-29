
using System.Text.Json;
using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;

namespace CommandsService.EventProcessor
{
    public class EventProcessor : IEventProcessor
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IMapper _mapper;

        public EventProcessor(IServiceScopeFactory scopeFactory, IMapper mapper)
        {
            _scopeFactory=scopeFactory;
            _mapper=mapper;
        }
      
        void IEventProcessor.EventProcessor(string message)
        {
            var eventType=DetermineEvent(message);
            switch(eventType)
            {
                case EventType.PublishedPlat:
                addPlatform(message);
                break;
                default:
                break;

            }
        }

        private EventType DetermineEvent(string notification)
        {
            Console.WriteLine("--> Determining event");
            var eventType=JsonSerializer.Deserialize<GenericEventDto>(notification);

            switch(eventType.Event)
            {
                case "Published_Platform":
                    Console.WriteLine("--> This is published plat");
                    return EventType.PublishedPlat;
                default: 
                    Console.WriteLine("--> Couldn't determine event type");
                    return EventType.Undetermined;
            }
        }

        private void addPlatform(string platformPublishedMessage)
        {
            using(var scope=_scopeFactory.CreateScope())
            {
                var repo=scope.ServiceProvider.GetRequiredService<ICommandRepo>();
                var platformPublishedDto=JsonSerializer.Deserialize<PlatformPublishDto>(platformPublishedMessage);

                try
                {
                    var plat=_mapper.Map<Platform>(platformPublishedDto);

                    if(!repo.ExternalPlatformExist(plat.ExternalID))
                    {
                        repo.CreatePlatform(plat);
                        repo.SaveChanges();
                    }else{
                        Console.WriteLine("---> Platform already exists.");
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"-----> error occured during adding platform {ex.Message}");
                }

            }
        }
    }

    enum EventType
    {
        PublishedPlat,
        Undetermined
    }
}
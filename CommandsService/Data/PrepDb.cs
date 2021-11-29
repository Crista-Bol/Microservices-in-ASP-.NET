using CommandsService.Data;
using CommandsService.Models;
using CommandsService.SyncDataServices.Grpc;

namespace CommandsService.Data
{
    public static class PrepDb
    {
        public static void PrepPopulation(WebApplication webApplication)
        {
            using(var serviceScope=webApplication.Services.CreateScope())
            {
                var grpcClient=serviceScope.ServiceProvider.GetService<IPlatformDataClient>();

                var platforms=grpcClient.ReturnAllPlatforms();

                SeedData(serviceScope.ServiceProvider.GetService<ICommandRepo>(),platforms);
            }
        }

        private static void SeedData(ICommandRepo commandRepo, IEnumerable<Platform> platforms)
        {
            Console.WriteLine("--> Seeding Data");

            if(platforms!=null)
            {
                foreach(var plat in platforms)
                {
                    if(!commandRepo.ExternalPlatformExist(plat.ExternalID))
                    {
                        commandRepo.CreatePlatform(plat);
                    }
                    commandRepo.SaveChanges();
                }
            }else{
               Console.WriteLine("--> plats null"); 
            }
            
        }
    } 
}
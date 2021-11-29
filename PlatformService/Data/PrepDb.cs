using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PlatformService.Models;

namespace PlatformService.Data
{
    public static class PrepDb
    {
        public static void prepPopulation(IApplicationBuilder app, bool isProd)
        {
            //creating service scope to use dbcontext
            using(var serviceScope=app.ApplicationServices.CreateScope())
            {
                SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>(),isProd);   
            }
        }

        public static void SeedData(AppDbContext context,bool isProd)
        {
            if(isProd)
            {
                Console.WriteLine("Attempting to try migration");
                try{
                    context.Database.Migrate();

                }catch(Exception ex)
                {
                    Console.WriteLine($"error occured during migration {ex.Message}");
                }
            }



            if(!context.Platforms.Any()){

                Console.WriteLine("Seeding Data");
                context.Platforms.AddRange(
                    new Platform(){Name="dotnet",Publisher="Microsoft",Cost="Free"},
                    new Platform(){Name="SQL",Publisher="Microsoft",Cost="Free"},
                    new Platform(){Name="Kubernetes",Publisher="Microsoft",Cost="Free"}
                );
                context.SaveChanges();
                
            }else
            {
                Console.WriteLine("We already have data");
            }

        }
    }
}
using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using PlatformService.Models;

namespace PlatformService.Data
{
    public static class PrepDb
    {
        public static void prepPopulation(IApplicationBuilder app)
        {
            //creating service scope to use dbcontext
            using(var serviceScope=app.ApplicationServices.CreateScope())
            {
                SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>());   
            }
        }

        public static void SeedData(AppDbContext context)
        {
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
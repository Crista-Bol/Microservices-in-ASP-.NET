using System.Collections.Generic;
using PlatformService.Models;

namespace PlatformService.Data
{
    public interface IPlatformRepo
    {

        void saveChanges();

        IEnumerable<Platform> GetAllPlatforms();
        
        Platform GetPlatformById(int Id);
        void CreatePlatform(Platform plat);

    }
}
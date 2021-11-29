using CommandsService.Models;

namespace CommandsService.Data
{
    public interface ICommandRepo
    {
        bool SaveChanges();

        //Platforms
        IEnumerable<Platform> GetAllPlatforms();
        void CreatePlatform(Platform plat);
        bool PlatformExist(int platId);
        bool ExternalPlatformExist(int extPlatform);

        //Commands
        IEnumerable<Command> GetCommandsForPlatform(int platId);
        Command GetCommand(int platId,int comId);

        void CreateCommand(int platId,Command com);
    }
}
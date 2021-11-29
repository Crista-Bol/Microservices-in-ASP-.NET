using CommandsService.Models;

namespace CommandsService.Data
{
    public class CommandRepo : ICommandRepo
    {
        public CommandRepo(AppDbContext context)
        {
            _context=context;
        }

        public AppDbContext _context { get; }

        public void CreateCommand(int platId, Command com)
        {
           if(com==null)
            {
                throw new ArgumentNullException(nameof(com));
            } 
            com.PlatformId=platId;
            _context.Commands.Add(com);
        }

        public void CreatePlatform(Platform plat)
        {
            if(plat==null)
            {
                throw new ArgumentNullException(nameof(plat));
            }
            _context.Platforms.Add(plat);
        }

        public IEnumerable<Platform> GetAllPlatforms()
        {
             return _context.Platforms.ToList();
        }

        public bool ExternalPlatformExist(int extPlatform)
        {
             return _context.Platforms.Any(p=>p.ExternalID==extPlatform);
        }
        public Command GetCommand(int platId, int comId)
        {
             return _context.Commands
            .Where(c=>c.PlatformId==platId && c.Id==comId).FirstOrDefault();
        }

        public IEnumerable<Command> GetCommandsForPlatform(int platId)
        {
            return _context.Commands
            .Where(c=>c.PlatformId==platId)
            .OrderBy(c=>c.Platform.Name);
        }

        public bool PlatformExist(int platId)
        {
            return _context.Platforms.Any(p=>p.Id==platId);
        }

        public bool SaveChanges()
        {
            return (_context.SaveChanges()>=0);
        }
    }
}
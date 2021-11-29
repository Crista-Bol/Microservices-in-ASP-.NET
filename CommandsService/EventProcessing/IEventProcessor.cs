namespace CommandsService.EventProcessor
{
    public interface IEventProcessor
    {
        void EventProcessor(string message);
    }
}
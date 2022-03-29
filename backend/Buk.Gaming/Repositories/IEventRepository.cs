using Buk.Gaming.Models;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Buk.Gaming.Repositories
{
    public interface IEventRepository
    {
        Task<List<EventInfo>> GetAllEventsAsync();

        Task<EventInfo> GetEventInfoAsync(string eventId);
    }
}
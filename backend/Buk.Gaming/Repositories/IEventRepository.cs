using Buk.Gaming.Models;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;


namespace Buk.Gaming.Repositories
{
    public interface IEventRepository
    {
        Task<List<EventInfo>> GetAllEventsAsync();

        Task<EventInfo> GetEventInfoAsync(string eventId);
    }
}
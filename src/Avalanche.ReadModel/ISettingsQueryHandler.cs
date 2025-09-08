using Avalanche.ReadModel.Models;
using Avalanche.ReadModel.Queries;
using Broadcast;

namespace Avalanche.ReadModel
{
    public interface ISettingsQueryHandler :
        IQueryHandler<IEnumerable<EventModel>, GetEventStoreEvents>,
        IQueryHandler<IEnumerable<ApiKey>, GetApiKeys>,
        IQueryHandler<ApiKey, GetApiKey>
    {
    }
}

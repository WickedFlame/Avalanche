using Avalanche.ReadModel.Queries;
using Broadcast;

namespace Avalanche.ReadModel
{
    public interface ISettingsQueryHandler : IQueryHandler<IEnumerable<EventModel>, GetEventStoreEvents>
    {
    }
}

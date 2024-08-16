using Kalendarzyk.Models.EventModels;
using Kalendarzyk.Models.EventTypesModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kalendarzyk.Data
{
    public interface IEventRepository
    {
        // Event methods
        Task AddEventAsync(EventModel eventToAdd);
        Task<IEnumerable<EventModel>> GetEventsListAsync();
        Task<EventModel> GetEventByIdAsync(int eventId);
        Task<EventModel> GetTypeByIdAsync(int typeId);
        Task<EventModel> GetGroupByIdAsync(int groupId);
        Task UpdateEventAsync(EventModel eventToUpdate);
        Task DeleteFromEventsListAsync(EventModel eventToDelete);
        Task ClearAllEventsListAsync();

        // Event group methods
        Task AddEventGroupAsync(EventGroupModel eventGroupToAdd);
        Task<IEnumerable<EventGroupModel>> GetEventGroupsListAsync();
        Task UpdateEventGroupAsync(EventGroupModel eventGroupToUpdate);
        Task DeleteFromEventGroupListAsync(EventGroupModel eventGroupToDelete);
        Task ClearAllEventGroupsAsync();

        // Event type methods
        Task AddEventTypeAsync(EventTypeModel eventTypeToAdd);
        Task<IEnumerable<EventTypeModel>> GetEventTypesListAsync();
        Task UpdateEventTypeAsync(EventTypeModel eventTypeToUpdate);
        Task DeleteFromEventTypesListAsync(EventTypeModel eventTypeToDelete);
        Task ClearAllEventTypesAsync();

        Task SaveEventsAndTypesToFile(IEnumerable<EventModel> eventsToSave = null);
        Task LoadEventsAndTypesFromFile();
    }
}

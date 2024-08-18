using CommunityToolkit.Mvvm.Input;
using Kalendarzyk.Models.EventTypesModels;
using System.Collections.ObjectModel;
using TheKalendarzyk.ViewModels.Events;

namespace TheKalendarzyk.ViewModels.CCViewModels
{
    internal interface IEventGroupsCCViewModel
    {
        public EventGroupModel SelectedEventGroup { get; set; }
        ObservableCollection<EventGroupViewModel> EventGroupsOC { get; set; }
        RelayCommand<EventGroupViewModel> EventGroupSelectedCommand { get; }
    }
}
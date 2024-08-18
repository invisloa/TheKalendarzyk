using CommunityToolkit.Mvvm.Input;
using Kalendarzyk.Models.EventTypesModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheKalendarzyk.ViewModels.Events;

namespace TheKalendarzyk.Views.CustomControls.CCInterfaces
{
    public interface IEventGroupsSelectorCCViewModel
    {
        public EventGroupModel SelectedEventGroup { get; set; }
        ObservableCollection<EventGroupViewModel> EventGroupsVisualsOC { get; set; }
        RelayCommand<EventGroupViewModel> EventGroupSelectedCommand { get; }
    }
}

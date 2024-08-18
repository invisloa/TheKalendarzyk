using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Mvvm.Input;
using Kalendarzyk.Models.EventModels;
using Kalendarzyk.Models.EventTypesModels;
using TheKalendarzyk.Services;

namespace TheKalendarzyk.ViewModels.Pages
{
    class EventPageViewModel : EventOperationsBaseViewModel
    {

            #region Fields
            private AsyncRelayCommand _asyncDeleteEventCommand;
            private AsyncRelayCommand _asyncShareEventCommand;
            #endregion
            #region Properties
            public string PageTitle => IsEditMode ? "Edit Event" : "Add Event";
            public string HeaderText => IsEditMode ? $"EDIT EVENT" : "ADD NEW EVENT";
            public override bool IsEditMode => _selectedCurrentEvent != null;

            public AsyncRelayCommand AsyncDeleteEventCommand
            {
                get => _asyncDeleteEventCommand;
                set => _asyncDeleteEventCommand = value;
            }

            public AsyncRelayCommand AsyncShareEventCommand
            {
                get => _asyncShareEventCommand;
                set => _asyncShareEventCommand = value;
            }

            #endregion


            #region Ctors
            // ctor for creating evnents create event mode
            public EventPageViewModel(DateTime selectedDate)
                : base()
            {
                IsCompletedButton = Factory.CreateNewChangableFontsIconAdapter(false, "check_box", "check_box_outline_blank");
                EventTypesInfoButton = Factory.CreateNewChangableFontsIconAdapter(true, "info", "info_outline");
                ExtraOptionsHelperToChangeName = Factory.CreateNewExtraOptionsEventHelperClass();
                StartDateTime = selectedDate;
                EndDateTime = selectedDate;
                _asyncSubmitEventCommand = new AsyncRelayCommand(AddEventAsync, CanExecuteSubmitCommand);

            }


            // ctor for editing events edit event mode
            public EventPageViewModel(EventModel eventToEdit)
            : base()
            {

                _selectedCurrentEvent = eventToEdit;

                _asyncSubmitEventCommand = new AsyncRelayCommand(AsyncEditEventAndGoBack, CanExecuteSubmitCommand);
                AsyncDeleteEventCommand = new AsyncRelayCommand(AsyncDeleteSelectedEvent);
                AsyncShareEventCommand = new AsyncRelayCommand(AsyncShareEvent);
                SelectEventTypeCommand = null;

                // Set properties based on eventToEdit
                ExtraOptionsHelperToChangeName = Factory.CreateNewExtraOptionsEventHelperClass(eventToEdit.EventType);
                OnEventTypeSelectedCommand(eventToEdit.EventType);

                Title = _selectedCurrentEvent.Title;
                Description = _selectedCurrentEvent.Description;
                StartDateTime = _selectedCurrentEvent.StartDateTime.Date;
                EndDateTime = _selectedCurrentEvent.EndDateTime.Date;
                SelectedEventGroup = _selectedCurrentEvent.EventType.EventGroup;
                SelectedEventType = _selectedCurrentEvent.EventType;

                IsCompletedButton = Factory.CreateNewChangableFontsIconAdapter(_selectedCurrentEvent.IsCompleted, "check_box", "check_box_outline_blank");
                EventTypesInfoButton = Factory.CreateNewChangableFontsIconAdapter(false, "info", "info_outline");


                FilterAllEventTypesOCByEventGroup(SelectedEventGroup); // CANNOT CHANGE MAIN EVENT TYPE


                EventGroupSelectedCommand = null;
                StartExactTime = _selectedCurrentEvent.StartDateTime.TimeOfDay;
                EndExactTime = _selectedCurrentEvent.EndDateTime.TimeOfDay;
            }
            #endregion
            #region Command Execution Methods

            private bool CanExecuteSubmitCommand()
            {
                if (string.IsNullOrWhiteSpace(Title) || SelectedEventType == null)
                {
                    return false;
                }
                return true;
            }

            private async Task AddEventAsync()
            {
                _selectedCurrentEvent = Factory.CreatePropperEvent(Title, Description, StartDateTime.Date + StartExactTime, EndDateTime.Date + EndExactTime, SelectedEventType, ExtraOptionsHelperToChangeName.DefaultMeasurementSelectorCCHelper.QuantityAmount ?? null, ExtraOptionsHelperToChangeName.MicroTasksCCAdapter?.MicroTasksOC ?? null); // XXXX YYYYY  TO CHECK IF to list wont make error in ExtraOptionsHelperToChangeName.MicroTasksCCAdapter.MicroTasksOC.ToList() TODO !!!!!add microtasks

                bool areSameList = ReferenceEquals(EventRepository.AllEventsList, _eventTimeConflictChecker.allEvents);
                // create a new confilict checker to stop not same list issues
                if (!areSameList)
                {
                    _eventTimeConflictChecker = Factory.CreateNewEventTimeConflictChecker(EventRepository.AllEventsList);
                }
                // Create a new Event based on the selected EventType
                if (!_eventTimeConflictChecker.IsEventConflict(PreferencesManager.GetEventTypeTimesDifferent(), PreferencesManager.GetEventGroupTimesDifferent(), _selectedCurrentEvent))
                {
                    await EventRepository.AddEventAsync(_selectedCurrentEvent);
                }
                else
                {
                    await App.Current.MainPage.DisplayActionSheet("ALREADY AN EVENT\nIN THE SPECIFIED TIME.", "OK", null);
                    return;
                }
                //ClearFields();
                await Shell.Current.GoToAsync("..");    // TODO CHANGE NOT WORKING!!!

            }

            private async Task AsyncEditEvent()
            {
                _selectedCurrentEvent.Title = Title;
                _selectedCurrentEvent.Description = Description;
                _selectedCurrentEvent.EventType = (EventTypeModel)SelectedEventType;
                _selectedCurrentEvent.StartDateTime = StartDateTime.Date + StartExactTime;
                _selectedCurrentEvent.EndDateTime = EndDateTime.Date + EndExactTime;
                _selectedCurrentEvent.IsCompleted = IsCompletedButton.IsSelected;


                //THIS IS NOT BEING REFRESHED in the view!!! _selectedCurrentEvent.MicroTasksList = ExtraOptionsHelperToChangeName.MicroTasksCCAdapter.MicroTasksOC.ToList();

                _selectedCurrentEvent.EventType.MicroTasksList = ExtraOptionsHelperToChangeName.MicroTasksCCAdapter?.MicroTasksOC.ToList();
                ExtraOptionsHelperToChangeName.DefaultMeasurementSelectorCCHelper.QuantityAmount = new Quantity(ExtraOptionsHelperToChangeName.DefaultMeasurementSelectorCCHelper.SelectedMeasurementUnit.TypeOfMeasurementUnit, ExtraOptionsHelperToChangeName.DefaultMeasurementSelectorCCHelper.QuantityValue);
                _selectedCurrentEvent.EventType.QuantityAmount = ExtraOptionsHelperToChangeName.DefaultMeasurementSelectorCCHelper.QuantityAmount;
                await EventRepository.UpdateEventAsync(_selectedCurrentEvent);
            }
            private async Task AsyncEditEventAndGoBack()
            {
                await AsyncEditEvent();
                await Shell.Current.GoToAsync("..");
            }
            private void FilterAllEventTypesOCByEventGroup(EventGroupModel value)
            {
                var tempFilteredEventTypes = AllEventTypesOC.ToList().FindAll(x => x.EventGroup.Equals(value));
                AllEventTypesOC = new ObservableCollection<EventTypeModel>(tempFilteredEventTypes);
                OnPropertyChanged(nameof(AllEventTypesOC));
            }

            private async Task AsyncDeleteSelectedEvent()
            {
                var action = await App.Current.MainPage.DisplayActionSheet($"Delete event {_selectedCurrentEvent.Title}", "Cancel", null, "Delete");
                switch (action)
                {
                    case "Delete":
                        await EventRepository.DeleteFromEventsListAsync(_selectedCurrentEvent);
                        await Shell.Current.GoToAsync("..");
                        break;
                    default:
                        // Cancel was selected or back button was pressed.
                        break;
                }
                return;

            }
            #endregion
        }

    
}

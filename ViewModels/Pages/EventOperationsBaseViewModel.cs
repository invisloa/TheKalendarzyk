using CommunityToolkit.Mvvm.Input;
using Kalendarzyk.Data;
using Kalendarzyk.Models.EventTypesModels;
using Kalendarzyk.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheKalendarzyk.Services;
using TheKalendarzyk.ViewModels.Events;

namespace TheKalendarzyk.ViewModels.Pages
{
    public abstract class EventOperationsBaseViewModel : BaseViewModel
    {

        EventsService _eventsService;
        private bool _canSubmitEvent;
        public bool CanSubmitEvent      // added since color converter doesnt work with canexecute
        {
            get => _canSubmitEvent;
            set
            {
                _canSubmitEvent = value;
                OnPropertyChanged();
            }
        }
        // ctor
        public EventOperationsBaseViewModel()
        {
            _eventRepository =  Factory.GetEventRepository;
            _eventsService = new EventsService(_eventRepository);

            _eventGroupsCCHelper = Factory.CreateNewIEventGroupViewModelClass(_eventsService.AllEventGroupsOC);
            AllEventTypesOC = new ObservableCollection<EventTypeModel>(_eventRepository.AllEventTypesList);
            AllEventsListOC = _eventRepository.AllEventsList;
            EventGroupSelectedCommand = new RelayCommand<EventGroupViewModel>(OnEventGroupSelected);
            SelectEventTypeCommand = new RelayCommand<EventTypeModel>(OnEventTypeSelectedCommand);
            _eventTimeConflictChecker = Factory.CreateNewEventTimeConflictChecker(_eventRepository.AllEventsList);
        }

        //Fields
        #region Fields
        // Language
        private int _fontSize = 20;
        List<MicroTask> microTasksList = new List<MicroTask>();
        protected IEventTimeConflictChecker _eventTimeConflictChecker;
        private ChangableFontsIconAdapter _eventTypesInfoButton;
        private ChangableFontsIconAdapter _isCompletedButton;


        // normal fields
        protected IEventGroupsCCViewModel _eventGroupsCCHelper;
        protected IEventRepository _eventRepository;
        protected EventModel _selectedCurrentEvent;
        protected string _title;
        protected string _description;
        protected DateTime _startDateTime = DateTime.Today;
        protected TimeSpan _startExactTime = DateTime.Now.TimeOfDay;
        protected DateTime _endDateTime = DateTime.Today;
        protected TimeSpan _endExactTime = DateTime.Now.TimeOfDay;
        protected AsyncRelayCommand _asyncSubmitEventCommand;
        protected Color _eventGroupBackgroundColor;
        protected ObservableCollection<EventTypeModel> _eventTypesOC;
        protected ObservableCollection<EventModel> _allEventsListOC;
        protected EventTypeModel _selectedEventType;
        private RelayCommand _goToAddNewEventTypePageCommand;
        private RelayCommand _goToAddEventPageCommand;
        public event Action<EventGroupModel> EventGroupChanged;


        #endregion
        //Properties
        #region Properties
        public abstract bool IsEditMode { get; }
        public int FontSize => _fontSize;
        public ChangableFontsIconAdapter EventTypesInfoButton

        {
            get => _eventTypesInfoButton;
            set
            {
                _eventTypesInfoButton = value;
                OnPropertyChanged();
            }
        }
        public ChangableFontsIconAdapter IsCompletedButton

        {
            get => _isCompletedButton;
            set
            {
                _isCompletedButton = value;
                OnPropertyChanged();
            }
        }

        private ExtraOptionsEventsHelperClass extraOptionsSelectorCC;
        public ExtraOptionsEventsHelperClass ExtraOptionsHelperToChangeName
        {
            get => extraOptionsSelectorCC;
            set => extraOptionsSelectorCC = value;
        }
        public string EventTypePickerText { get => "Select event Type"; }
        public RelayCommand GoToAddEventPageCommand
        {
            get
            {
                return _goToAddEventPageCommand ?? (_goToAddEventPageCommand = new RelayCommand(GoToAddEventPage));
            }
        }
        public MicroTasksCCAdapterVM MicroTasksCCAdapter { get; set; }

        public EventGroupModel SelectedEventGroup
        {
            get => _eventGroupsCCHelper.SelectedEventGroup;
            set
            {
                _eventGroupsCCHelper.SelectedEventGroup = value;
                OnPropertyChanged();
            }
        }

        private void FilterAllEventTypesOCByEventGroup(EventGroupModel value)
        {
            AllEventTypesOC = _eventRepository.AllEventTypesList.Where(x => x.EventGroup.Equals(value)).ToObservableCollection();
            OnPropertyChanged(nameof(AllEventTypesOC));
        }
        public ObservableCollection<EventGroupViewModel> EventGroupsVisualsOC
        {
            get => _eventGroupsCCHelper.EventGroupsVisualsOC;
            set => _eventGroupsCCHelper.EventGroupsVisualsOC = value;
        }
        public ObservableCollection<EventTypeModel> AllEventTypesOC
        {
            get => _eventTypesOC;
            set
            {
                _eventTypesOC = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<EventModel> AllEventsListOC
        {
            get => _allEventsListOC;
            set
            {
                _allEventsListOC = value;
                OnPropertyChanged();
            }
        }
        public bool IsEventTypeSelected
        {
            get => _selectedEventType != null;
        }

        // Basic Event Information
        #region Basic Event Information
        public EventTypeModel SelectedEventType
        {
            get => _selectedEventType;
            set
            {
                if (_selectedEventType == value) return;
                _selectedEventType = value;
                _asyncSubmitEventCommand.NotifyCanExecuteChanged();
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsEventTypeSelected));
                CanSubmitEvent = !string.IsNullOrWhiteSpace(_title) && SelectedEventType != null;
                OnPropertyChanged(nameof(CanSubmitEvent));
            }
        }
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged();
                CanSubmitEvent = !string.IsNullOrWhiteSpace(_title) && SelectedEventType != null;
                OnPropertyChanged(nameof(CanSubmitEvent));
                _asyncSubmitEventCommand.NotifyCanExecuteChanged();
            }
        }
        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged();
            }
        }
        // Start Date/Time

        // setting logic is in the setter because TimePicker and DatePicker doesnt support commands
        public DateTime StartDateTime
        {
            get => _startDateTime;
            set
            {
                if (_startDateTime == value) return;
                _startDateTime = value;
                OnPropertyChanged();
                if (!IsEditMode)
                {
                    SetEndExactTimeAccordingToEventType();
                }
                if (_startDateTime > _endDateTime)
                {
                    _endDateTime = _startDateTime;
                    OnPropertyChanged(nameof(EndDateTime));
                }
            }
        }
        // setting logic is in the setter because TimePicker and DatePicker doesnt support commands
        public DateTime EndDateTime
        {
            get => _endDateTime;
            set
            {
                try
                {
                    if (_endDateTime == value) return;
                    if (_startDateTime > value)
                    {
                        _endDateTime = _startDateTime = value;
                        OnPropertyChanged(nameof(StartDateTime));
                    }
                    else
                    {
                        _endDateTime = value;
                    }
                    OnPropertyChanged();
                }
                catch
                {
                    _endDateTime = _startDateTime;
                    OnPropertyChanged(nameof(EndDateTime));
                }

            }
        }
        // setting logic is in the setter because TimePicker and DatePicker doesnt support commands
        public TimeSpan StartExactTime
        {
            get => _startExactTime;
            set
            {
                if (_startExactTime == value) return; // Avoid unnecessary setting and triggering.
                _startExactTime = value;
                OnPropertyChanged();
                if (!IsEditMode)
                {
                    SetEndExactTimeAccordingToEventType();
                }
                if (_startDateTime.Date == _endDateTime.Date && _startExactTime > _endExactTime)
                {
                    _endExactTime = _startExactTime;
                    OnPropertyChanged(nameof(EndExactTime));
                }
            }
        }

        // setting logic is in the setter because TimePicker and DatePicker doesnt support commands
        public TimeSpan EndExactTime
        {
            get => _endExactTime;
            set
            {
                try
                {
                    if (_endExactTime == value) return; // Avoid unnecessary setting and triggering.
                    _endExactTime = value;
                    var startDate = StartDateTime;
                    var endDate = EndDateTime;
                    if (_startDateTime.Date == _endDateTime.Date && value < _startExactTime)
                    {
                        _startExactTime = value;
                        OnPropertyChanged(nameof(StartExactTime));
                    }
                    OnPropertyChanged();
                }
                catch
                {
                    _endExactTime = _startExactTime;
                    OnPropertyChanged(nameof(EndExactTime));
                }

            }
        }
        #endregion

        // Command
        public AsyncRelayCommand AsyncSubmitEventCommand => _asyncSubmitEventCommand;
        public RelayCommand<EventTypeModel> SelectEventTypeCommand { get; set; }
        public RelayCommand<EventGroupViewModel> EventGroupSelectedCommand { get; set; }
        public RelayCommand GoToAddNewEventTypePageCommand => _goToAddNewEventTypePageCommand ?? (_goToAddNewEventTypePageCommand = new RelayCommand(GoToAddNewEventTypePage));

        protected IEventRepository EventRepository
        {
            get => _eventRepository;
            set => _eventRepository = value;
        }
        #endregion


        // Helper Methods
        #region Helper Methods
        private bool IsNumeric(string value)
        {
            return Decimal.TryParse(value, out _);
        }

        private void GoToAddEventPage()
        {
            Application.Current.MainPage.Navigation.PushAsync(new EventPage(DateTime.Today));
        }


        protected void ClearFields()
        {
            Title = "";
            Description = "";
            IsCompletedButton.IsSelected = false;
            // TODO Show POPUP ???
        }
        protected void SetVisualsForSelectedEventType()
        {
            // set the color of the selected event type and set others colors to blacked out
            foreach (var eventType in AllEventTypesOC)       // it sets colors in a different AllEventTypesOC then SelectedEventType is...
            {
                eventType.EventTypeColorString = "#FFFFFFFF"; // White color in ARGB format
                eventType.IsSelectedToFilter = false;
            }
            var SelectedEventType = AllEventTypesOC.FirstOrDefault(x => x.Equals(_selectedEventType));
            //--

            SelectedEventType.IsSelectedToFilter = true;
            AllEventTypesOC = new ObservableCollection<EventTypeModel>(AllEventTypesOC); // ?????? this is done to trigger the property changed event
            var selectedEventGroup = EventGroupsVisualsOC.Where(x => x.EventGroup.Equals(SelectedEventType.EventGroup)).FirstOrDefault();
            _eventGroupsCCHelper.EventGroupSelectedCommand.Execute(selectedEventGroup);
            SelectedEventGroup = SelectedEventType.EventGroup;
            OnPropertyChanged(nameof(EventGroupsVisualsOC));

        }
        protected virtual void OnEventGroupSelected(EventGroupViewModel selectedEventGroup)
        {
            if (SelectedEventGroup == null || SelectedEventGroup != selectedEventGroup.EventGroup)
            {
                _eventGroupsCCHelper.EventGroupSelectedCommand.Execute(selectedEventGroup);
                SelectedEventGroup = _eventGroupsCCHelper.SelectedEventGroup;
                FilterAllEventTypesOCByEventGroup(SelectedEventGroup);
            }
            if (AllEventTypesOC.Count > 0)
            {
                OnEventTypeSelectedCommand(AllEventTypesOC[0]);
            }
            else
            {
                SelectedEventType = null;
            }
        }


        private void GoToAddNewEventTypePage()
        {
            Application.Current.MainPage.Navigation.PushAsync(new AddNewEventTypePage());
        }

        #endregion
        protected void OnEventTypeSelectedCommand(EventTypeModel selectedEventType)
        {
            SelectedEventType = selectedEventType;
            if (!IsEditMode)
            {
                ExtraOptionsHelperToChangeName.OnEventTypeChanged(selectedEventType);
                            }
            SetVisualsForSelectedEventType();
        }
    }
}

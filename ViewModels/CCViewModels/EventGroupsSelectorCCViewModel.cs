using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Kalendarzyk.Models;
using Kalendarzyk.Models.EventTypesModels;
using Kalendarzyk.ViewModels;
using TheKalendarzyk.ViewModels.Events;

namespace TheKalendarzyk.ViewModels.CCViewModels
{
    public class EventGroupsSelectorCCViewModel : BaseViewModel, IEventGroupsCCViewModel
    {
        // Constants
        private const int FullOpacity = 1;
        private const float FadedOpacity = 0.3f;
        private const int NoBorderSize = 0;
        private const int BorderSize = 10;

        // Fields
        private readonly ObservableCollection<EventGroupModel> _eventGroupsList;
        private readonly Dictionary<EventGroupModel, EventGroupViewModel> _eventVisualDetails;

        // Properties
        private EventGroupModel _selectedEventGroup;
        public EventGroupModel SelectedEventGroup
        {
            get => _selectedEventGroup;
            set
            {
                if (_selectedEventGroup != value)
                {
                    _selectedEventGroup = value;
                    OnPropertyChanged();
                }
            }
        }

        private IconModel _selectedVisualElement;
        public IconModel SelectedVisualElement
        {
            get => _selectedVisualElement;
            set
            {
                if (_selectedVisualElement != value)
                {
                    _selectedVisualElement = value;
                    OnPropertyChanged();
                }
            }
        }

        private Color _selectedColor = Color.FromRgb(255, 0, 0); // Default to red
        public Color SelectedColor
        {
            get => _selectedColor;
            set
            {
                if (_selectedColor != value)
                {
                    _selectedColor = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<EventGroupViewModel> EventGroupsOC { get; set; }

        // Commands
        public ICommand EventGroupSelectedCommand { get; private set; }

        // Events
        public event Action<EventGroupModel> EventGroupChanged;

        // ctor
        public EventGroupsSelectorCCViewModel(ObservableCollection<EventGroupModel> eventGroupsList)
        {
            _eventGroupsList = eventGroupsList ?? throw new ArgumentNullException(nameof(eventGroupsList));
            _eventVisualDetails = new Dictionary<EventGroupModel, EventGroupViewModel>();
            EventGroupSelectedCommand = new RelayCommand<EventGroupViewModel>(SetEventGroupFromViewModel);
            InitializeEventGroupsVisuals();
        }

        // Private Methods
        private void SetEventGroupFromViewModel(EventGroupViewModel groupViewModel)
        {
            var selectedEventGroup = _eventGroupsList.FirstOrDefault(o => o.Equals(groupViewModel));
            if (selectedEventGroup == null)
            {
                throw new ArgumentException($"Invalid EventGroup value: {groupViewModel}");
            }
            SelectedEventGroup = selectedEventGroup;
            VisuallySelectEventGroupElement();
            EventGroupChanged?.Invoke(SelectedEventGroup);
        }

        private void VisuallySelectEventGroupElement()
        {
            // Deselect all currently selected items
            var selectedItems = _eventVisualDetails.Values.Where(vm => vm.IsSelected).ToList();
            foreach (var item in selectedItems)
            {
                item.IsSelected = false;
            }

            // Select the new element
            if (SelectedEventGroup != null && _eventVisualDetails.TryGetValue(SelectedEventGroup, out var viewModel))
            {
                viewModel.IsSelected = true;
            }
        }


        private void InitializeEventGroupsVisuals()
        {
            EventGroupsOC = new ObservableCollection<EventGroupViewModel>();

            foreach (EventGroupModel eventType in _eventGroupsList)
            {
                var viewModel = new EventGroupViewModel(eventType);
                _eventVisualDetails[eventType] = viewModel;
                EventGroupsOC.Add(viewModel);
            }
        }
    }
}

using Kalendarzyk.Models.EventModels;

namespace TheKalendarzyk.Views;

public partial class EventPage : ContentPage
{
    // For adding events
    public EventPage(DateTime selcetedDate)
    {
        try
        {
            InitializeComponent();
        }
        catch (Exception ex)
        {
            DisplayAlert("Error", $"{ex}", "InitializeComponent yyy");
        }
        try
        {
            BindingContext = new EventOperationsViewModel(selcetedDate);
        }
        catch (Exception ex)
        {
            DisplayAlert("Error", $"{ex}", "BindingContext yyy");
        }

    }
    public EventPage()
    {
        var today = DateTime.Today;
        InitializeComponent();
        BindingContext = new EventOperationsViewModel(today);

    }
    // For editing events
    public EventPage(EventModel eventModel)
    {
        InitializeComponent();
        try
        {
            BindingContext = new EventOperationsViewModel(eventToEdit: eventModel);
        }
        catch (Exception ex)
        {
            DisplayAlert("Error", $"{ex}", "yyy");
        }
    }
}

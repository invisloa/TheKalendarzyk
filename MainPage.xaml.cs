using Kalendarzyk.Data;

namespace TheKalendarzyk
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {

            InitializeComponent();
            
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            SQLiteRepository sqLiteRepository = await SQLiteRepository.CreateAsync();

        }
    }

}

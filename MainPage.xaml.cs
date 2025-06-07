using System.Text.RegularExpressions;

namespace TableShot
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
            var groups = App.UserGroups; // set after login
            AdminButton.IsVisible = groups.Contains("admin");
            CompetitionButton.IsVisible = groups.Contains("competitor");
        }

    }

}

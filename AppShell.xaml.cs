namespace TableShot
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
            Routing.RegisterRoute(nameof(SignUpPage), typeof(SignUpPage));
            Routing.RegisterRoute(nameof(ConfirmSignUpPage), typeof(ConfirmSignUpPage));
            Routing.RegisterRoute(nameof(SignInPage), typeof(SignInPage));
            Routing.RegisterRoute(nameof(TableEditPage), typeof(TableEditPage));
            Routing.RegisterRoute(nameof(TableViewPage), typeof(TableViewPage));

        }
    }
}

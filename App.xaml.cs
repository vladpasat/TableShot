namespace TableShot
{
    public partial class App : Application
    {
        public static IReadOnlyList<string> UserGroups { get; set; }

        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}
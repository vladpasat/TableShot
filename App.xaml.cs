using Amazon.DynamoDBv2.DocumentModel;
using TableShot.ViewModels;
namespace TableShot
{
    public partial class App : Application
    {
        public static IReadOnlyList<string> UserGroups { get; set; }
        public static string IdToken { get; set; }
        public static TableItemsViewModel TableVm { get; } = new TableItemsViewModel();
        public App()
        {
            InitializeComponent();
            // 3) Set your Shell (or MainPage) as before
            MainPage = new AppShell();
        }
    }
}
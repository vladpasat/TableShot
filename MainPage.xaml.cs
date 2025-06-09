using System.Text.RegularExpressions;
using Amazon.CognitoIdentity;
using Amazon;
using Amazon.DynamoDBv2;
using TableShot.Models;
using TableShot.ViewModels;
using TableShot;


namespace TableShot
{
    public partial class MainPage : ContentPage
    {
        readonly TableItemsViewModel _vm;

        public MainPage()
        {
            InitializeComponent();
            var groups = App.UserGroups; // set after login
            ButtonAddTable.IsVisible = groups.Contains("admin");
            ButtonEditTable.IsVisible = groups.Contains("admin");
            _vm = App.TableVm;
            //const string identityPoolid = "eu-north-1:39350368-a57a-4e6b-b949-9c7068b6e368";
            //var creds = new CognitoAWSCredentials(
            //    identityPoolid,
            //    RegionEndpoint.EUNorth1
            //);
            //var ddbClient = new AmazonDynamoDBClient(creds, RegionEndpoint.EUNorth1);
            //CompetitionButton.IsVisible = groups.Contains("competitor");
        }


        private async void ButtonAddTable_Clicked(object sender, EventArgs e)
        {
            string tableId = await DisplayPromptAsync(
                        "Table Name",
                        "Enter a name for the table:",
                        accept: "OK",
                        cancel: "Cancel",
                        placeholder: "e.g. ScoresTable");

            if (string.IsNullOrWhiteSpace(tableId))
            {
                await DisplayAlert("Invalid", "Table name cannot be empty.", "OK");
                return;
            }


            var existing = await _vm.GetAllTableDefinitionsAsync();
            if (existing.Any(d => d.TableId.Equals(tableId, StringComparison.OrdinalIgnoreCase)))
            {
                await DisplayAlert("Duplicate", "A table with that name already exists.", "OK");
                return;
            }

            var answer = await DisplayPromptAsync(
                "New Table",
                "How many extra columns?",
                accept: "OK", cancel: "Cancel",
                placeholder: "e.g. 3",
                keyboard: Keyboard.Numeric);

            if (!int.TryParse(answer, out var extraCols) || extraCols < 0)
            {
                await DisplayAlert("Invalid", "Enter a non-negative integer.", "OK");
                return;
            }


            await _vm.CreateTableDefinitionAsync(tableId, extraCols);

        }


        private async void ButtonEditTable_Clicked(object sender, EventArgs e)
        {
            var defs = await _vm.GetAllTableDefinitionsAsync();
            if (defs.Count == 0)
            {
                await DisplayAlert("No Tables", "You have no tables yet.", "OK");
                return;
            }

            var ids = defs.Select(d => d.TableId).ToArray();
            var choice = await DisplayActionSheet(
                "Select a table",
                "Cancel", null,
                ids);

            if (string.IsNullOrEmpty(choice) || choice == "Cancel")
                return;

            await Shell.Current.GoToAsync(
                $"{nameof(TableEditPage)}?tableId={Uri.EscapeDataString(choice)}");
        }

        private async void ButtonViewTable_Clicked(object sender, EventArgs e)
        {
            // 1) Load all definitions
            var defs = await _vm.GetAllTableDefinitionsAsync();
            if (defs == null || defs.Count == 0)
            {
                await DisplayAlert("No Tables", "There are no tables yet.", "OK");
                return;
            }

            // 2) Ask the user to pick one
            var tableIds = defs.Select(d => d.TableId).ToArray();
            var choice = await DisplayActionSheet(
                "Select a table to view",
                "Cancel",
                null,
                tableIds);

            if (string.IsNullOrEmpty(choice) || choice == "Cancel")
                return;


            // 3) Navigate to the TableViewPage, passing the tableId
            await Shell.Current.GoToAsync(
                $"{nameof(TableViewPage)}?tableId={Uri.EscapeDataString(choice)}");
        }
    }

}

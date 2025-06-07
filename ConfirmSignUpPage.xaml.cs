using System;
using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.Extensions.CognitoAuthentication;
using Microsoft.Maui.Controls;

namespace TableShot
{
    [QueryProperty(nameof(Email), "email")]
    public partial class ConfirmSignUpPage : ContentPage
    {
        // TODO: replace these with your actual Cognito Pool ID, Client ID, and region
        private const string PoolId = "eu-north-1_0ELf78r9L";
        private const string ClientId = "1r3poi7410ffq6vd741drf22gg";
        private static readonly RegionEndpoint CognitoRegion = RegionEndpoint.EUNorth1;

        private readonly CognitoUserPool _userPool;
        private readonly IAmazonCognitoIdentityProvider _provider;

        public ConfirmSignUpPage()
        {
            InitializeComponent();

            _provider = new AmazonCognitoIdentityProviderClient(CognitoRegion);
            _userPool = new CognitoUserPool(PoolId, ClientId, _provider);
        }

        // Binds the 'email' query parameter into the readonly EmailEntry
        private string email;
        public string Email
        {
            get => email;
            set
            {
                email = value;
                EmailEntry.Text = email;
            }
        }

        private async void OnConfirmClicked(object sender, EventArgs e)
        {
            LoadingIndicator.IsVisible = LoadingIndicator.IsRunning = true;
            ConfirmButton.IsEnabled = false;

            try
            {
                var user = new CognitoUser(email, ClientId, _userPool, _provider);
                // Confirm the user with the code they received

                // >>> no var result here—ConfirmSignUpAsync returns Task, not a value
                await user.ConfirmSignUpAsync(CodeEntry.Text, true);

                // if we get here, the confirmation succeeded
                await DisplayAlert("Success", "Your account has been confirmed!", "OK");
                await Shell.Current.GoToAsync(nameof(SignInPage));
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
            finally
            {
                LoadingIndicator.IsVisible = LoadingIndicator.IsRunning = false;
                ConfirmButton.IsEnabled = true;
            }
        }
    }
}
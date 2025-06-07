using System;
using System.Collections.Generic;
using Microsoft.Maui.Controls;
using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Runtime;

namespace TableShot
{
    public partial class SignUpPage : ContentPage
    {
        // TODO: Move these to secure config or Constants
        private const string PoolId = "eu-north-1_0ELf78r9L";
        private const string ClientId = "1r3poi7410ffq6vd741drf22gg";
        private static readonly RegionEndpoint CognitoRegion = RegionEndpoint.EUNorth1;

        private readonly AmazonCognitoIdentityProviderClient _provider;

        public SignUpPage()
        {
            InitializeComponent();
            _provider = new AmazonCognitoIdentityProviderClient(new AnonymousAWSCredentials(), CognitoRegion);
        }

        private async void OnSignUpClicked(object sender, EventArgs e)
        {
            string email = EmailEntry.Text?.Trim();
            string password = PasswordEntry.Text;
            string confirm = ConfirmPasswordEntry.Text;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirm))
            {
                await DisplayAlert("Error", "All fields are required.", "OK");
                return;
            }
            if (password != confirm)
            {
                await DisplayAlert("Error", "Passwords do not match.", "OK");
                return;
            }

            try
            {
                LoadingIndicator.IsVisible = true;
                LoadingIndicator.IsRunning = true;
                SignUpButton.IsEnabled = false;

                var signUpRequest = new SignUpRequest
                {
                    ClientId = ClientId,
                    Username = email,
                    Password = password,
                    UserAttributes = new List<AttributeType>
                    {
                        new AttributeType { Name = "email", Value = email }
                    }
                };

                var response = await _provider.SignUpAsync(signUpRequest);

                if ((bool)response.UserConfirmed)
                {
                    await DisplayAlert("Success", "Account created and confirmed!", "OK");
                    await Shell.Current.GoToAsync("//SignInPage");
                }
                else
                {
                    await DisplayAlert("Confirmation Required", "Check your email for a confirmation code.", "OK");
                    await DisplayAlert(
                    "Confirmation Required",
                    "Check your email for a confirmation code.",
                    "OK");

                    // Navigate to ConfirmSignUpPage, passing the email as a query parameter
                    await Shell.Current.GoToAsync(
                        $"//{nameof(ConfirmSignUpPage)}?email={Uri.EscapeDataString(email)}");
                }
            }
            catch (UsernameExistsException)
            {
                await DisplayAlert("Error", "An account with this email already exists.", "OK");
            }
            catch (InvalidPasswordException ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
            finally
            {
                LoadingIndicator.IsRunning = false;
                LoadingIndicator.IsVisible = false;
                SignUpButton.IsEnabled = true;
            }
        }

        private async void OnSignInTapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(SignInPage));
        }
    }
}
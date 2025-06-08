using System;
using Microsoft.Maui.Controls;
using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.Extensions.CognitoAuthentication;
using Amazon.Runtime;
using Microsoft.Maui.ApplicationModel;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using TableShot.ViewModels;
namespace TableShot
{
    public partial class SignInPage : ContentPage
    {
        // TODO: Move these to secure config or Constants
        private const string PoolId = "eu-north-1_0ELf78r9L";
        private const string ClientId = "1r3poi7410ffq6vd741drf22gg";
        private static readonly RegionEndpoint CognitoRegion = RegionEndpoint.EUNorth1; // change region as needed

        private readonly AmazonCognitoIdentityProviderClient _provider;
        private readonly CognitoUserPool _userPool;

        public SignInPage()
        {
            InitializeComponent();
            // Initialize AWS Cognito client
            _provider = new AmazonCognitoIdentityProviderClient(new AnonymousAWSCredentials(), CognitoRegion);
            _userPool = new CognitoUserPool(PoolId, ClientId, _provider);
        }

        private async void OnSignInClicked(object sender, EventArgs e)
        {
            string email = EmailEntry.Text?.Trim();
            string password = PasswordEntry.Text;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                await DisplayAlert("Error", "Please enter both email and password.", "OK");
                return;
            }

            try
            {
                // Show loading indicator
                LoadingIndicator.IsVisible = true;
                LoadingIndicator.IsRunning = true;
                SignInButton.IsEnabled = false;

                // Create Cognito user and initiate SRP auth
                var user = new CognitoUser(email, ClientId, _userPool, _provider);
                var authRequest = new InitiateSrpAuthRequest
                {
                    Password = password
                };
                AuthFlowResponse authResponse = await user.StartWithSrpAuthAsync(authRequest);

                if (authResponse.AuthenticationResult != null)
                {
                    // Successful login
                    // You can retrieve tokens: authResponse.AuthenticationResult.IdToken, AccessToken, RefreshToken
                    var idToken = authResponse.AuthenticationResult.IdToken;
                    var handler = new JwtSecurityTokenHandler();
                    var jwt = handler.ReadJwtToken(idToken);

                    App.IdToken = idToken;

                    App.UserGroups = jwt.Claims
                        .Where(c => c.Type == "cognito:groups")
                        .Select(c => c.Value)
                        .ToList();

                    await App.TableVm.InitAsync(idToken);
                    await Shell.Current.GoToAsync(nameof(MainPage));
                    //await MainThread.InvokeOnMainThreadAsync(async () =>
                    //    await Shell.Current.GoToAsync(nameof(MainPage))
                    //);
                }
                else
                {
                    await MainThread.InvokeOnMainThreadAsync(async () =>
                        await DisplayAlert("Login Failed", "Could not authenticate user.", "OK")
                    );
                }
            }
            catch (Exception ex)
            {
                await MainThread.InvokeOnMainThreadAsync(async () =>
                    await DisplayAlert("Error", ex.Message, "OK")
                );
            }
            finally
            {
                // Hide loading indicator
                LoadingIndicator.IsRunning = false;
                LoadingIndicator.IsVisible = false;
                SignInButton.IsEnabled = true;
            }
        }

        private async void OnForgotPasswordTapped(object sender, EventArgs e)
        {
            // TODO: Implement forgot password flow with Cognito
            await DisplayAlert("Forgot Password", "Password recovery flow not implemented yet.", "OK");
        }

        private async void OnSignUpTapped(object sender, EventArgs e)
        {
            // TODO: Navigate to Sign Up page, implement registration with Cognito
            await Shell.Current.GoToAsync(nameof(SignUpPage));
        }
    }
}

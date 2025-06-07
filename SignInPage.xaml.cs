namespace TableShot;

public partial class SignInPage : ContentPage
{
    public SignInPage()
    {
        InitializeComponent();
    }

    private async void OnSignInClicked(object sender, EventArgs e)
    {
        string username = UsernameEntry.Text;
        string password = PasswordEntry.Text;

        // Check that both fields have values.
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            await DisplayAlert("Error", "Please enter both username and password.", "OK");
            return;
        }

        // Replace this with your authentication logic.
        if (username == "user" && password == "password")
        {
            await DisplayAlert("Success", "Login successful.", "OK");
            // TODO: Navigate to the next page.
        }
        else
        {
            await DisplayAlert("Error", "Invalid username or password.", "OK");
        }
    }
}

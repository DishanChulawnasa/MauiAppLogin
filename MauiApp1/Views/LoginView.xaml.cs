using Npgsql;

namespace MauiApp1.Views;

public partial class LoginView : ContentPage
{   
    
    private NpgsqlConnection conn;
    string connstring = String.Format("Server={0};Port={1};" +
        "User Id={2};Password={3};Database={4};",
        "117.185.87.182", "5432", "postgres",
        "NeverTell!", "iot_db_development");
    private NpgsqlCommand cmd;
    private string sql = null;

    private bool isLoginSuceess = false;
    private bool isPasswordVisible = false;

    

    public LoginView()
	{
		InitializeComponent();
        conn = new NpgsqlConnection(connstring);
    }

    private async void LDKPassword_Clicked(object sender, EventArgs e)
    {
        await DisplayAlert("Login Help", "Please contact your sales representative or send email to " +
            "contact@gflowplus.com to request your account log in information.", "OK");
    }

    private void BShowPassword_Clicked(object sender, EventArgs e)
    {
        isPasswordVisible = !isPasswordVisible;
        PasswordEntry.IsPassword = !isPasswordVisible;

        var imageButton = sender as ImageButton;
        if (isPasswordVisible)
        {
            BShowPassword.Source = "hidepassword.png"; // Hide Password
        }
        else
        {
            BShowPassword.Source = "showpassword.png"; // Show Password
        }
    }

    private async void LAboutUs_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AboutUs());
    }

    private async void BLogin_Clicked(object sender, EventArgs e)
    {
        
        await Login();

        if (isLoginSuceess)
        {
            StatusWrongPW.IsVisible = false;
            ImageInfo.IsVisible = false;
            await Navigation.PushAsync(new MainPage());
        }
        else
        {
            StatusWrongPW.IsVisible = true;
            ImageInfo.IsVisible = true;
            //await DisplayAlert("Login Failed", "Invalid username or password.", "OK");
        }
    } 

    private async Task Login()
    {
        try
        {
            await conn.OpenAsync();

            sql = @"select * from u_login(:_username,:_password)";
            cmd = new NpgsqlCommand(sql, conn);

            // Set the expected data type for PostgreSQL
            cmd.Parameters.Add(new NpgsqlParameter("_username", NpgsqlTypes.NpgsqlDbType.Varchar));
            cmd.Parameters["_username"].Value = UsernameEntry.Text;

            cmd.Parameters.Add(new NpgsqlParameter("_password", NpgsqlTypes.NpgsqlDbType.Varchar));
            cmd.Parameters["_password"].Value = PasswordEntry.Text;

            int result = (int)await cmd.ExecuteScalarAsync();

            isLoginSuceess = (result == 1 || result == 2);

            await conn.CloseAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            await conn.CloseAsync();
        }
    }
}
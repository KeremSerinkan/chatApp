namespace chatApp;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
		InitializeComponent();
	}

    private async void btnSignIn_Clicked(System.Object sender, System.EventArgs e)
    {
        var authResult = await Auth(usernameEntry.Text, passwordEntry.Text);

        if (authResult  == "Auth")
        {
            // chat sayfasına geç
            await Shell.Current.GoToAsync($"//MainPage?username={usernameEntry.Text}");
        }
        else if (authResult == "Wrong Password")
        {
            await DisplayAlert("Uyarı", "Şifreniz Yanlış", "Tamam");
            return;
        }
        else if (authResult == "User Not Found, New User Created")
        {
            await DisplayAlert("Uyarı", "Böyle Bir Kullanıcı Bulunamadı, Yeni Oluşturuldu Tekrar Giriş Yapınız", "Tamam");
            return;
        }
        else
        {
            return;
        }
    }

    public static async Task<string> Auth(string userName, string pwd)
    {
        try
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "http://10.0.2.2:5180/api/Login/Auth");
            var collection = new List<KeyValuePair<string, string>>();
            collection.Add(new("NAME", userName));
            collection.Add(new("PASSWORD", pwd));
            var content = new FormUrlEncodedContent(collection);
            request.Content = content;
            var response = await client.SendAsync(request);
            var responseText = response.Content.ReadAsStringAsync().Result;

            return responseText;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            return "error";
        }
    }
}

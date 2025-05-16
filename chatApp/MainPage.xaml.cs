using System.Net.WebSockets;
using System.Text;
namespace chatApp;

public partial class MainPage : ContentPage, IQueryAttributable
{
    public string username;
	public MainPage()
	{
		InitializeComponent();
	}

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.ContainsKey("username"))
        {
            username = Uri.UnescapeDataString(query["username"].ToString());
        }
    }

    ClientWebSocket webSocket = new();
    CancellationTokenSource cts = new();
    string serverUri = "ws://10.0.2.2:5180/ws"; // Android için localhost -> 10.0.2.2

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await webSocket.ConnectAsync(new Uri("ws://10.0.2.2:5180/ws"), cts.Token);
        // Kullanıcı adını sunucuya ilk mesaj olarak gönder
        var introMessage = Encoding.UTF8.GetBytes($"__username__:{username}");
        await webSocket.SendAsync(new ArraySegment<byte>(introMessage), WebSocketMessageType.Text, true, cts.Token);

        _ = ListenForMessages();
    }

    private async Task ListenForMessages()
    {
        var buffer = new byte[1024];
        while (webSocket.State == WebSocketState.Open)
        {
            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cts.Token);
            var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
            MainThread.BeginInvokeOnMainThread(() =>
            {
                chatLabel.Text += $"{message}\n";
            });
        }
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        var msg = messageEntry.Text;
        if (string.IsNullOrWhiteSpace(msg)) return;

        var fullMessage = Encoding.UTF8.GetBytes(msg);
        await webSocket.SendAsync(new ArraySegment<byte>(fullMessage), WebSocketMessageType.Text, true, cts.Token);
        messageEntry.Text = "";
    }
}



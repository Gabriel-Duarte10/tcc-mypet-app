
using System.Collections.ObjectModel;
using System.Net.WebSockets;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Text.Json;
using tcc_mypet_app.Models.Dto;
using tcc_mypet_app.Services;

namespace tcc_mypet_app.Views.App;

public partial class ChatDetailPage : ContentPage
{
    public ObservableCollection<ChatMessages> ChatMessages { get; set; } = new ObservableCollection<ChatMessages>();
    private ClientWebSocket _webSocket = new ClientWebSocket();
    private readonly ApiService _api = new ApiService();
    private UserPetChatSessionDTO userPetChatSessionDTO;
    public UserPetChatSessionDTO UserPetChatSessionDTO
    {
        get { return userPetChatSessionDTO; }
        set
        {
            if (userPetChatSessionDTO != value)
            {
                userPetChatSessionDTO = value;
                OnPropertyChanged();
            }
        }
    }

    public UserDto _User;

    public ChatDetailPage(UserPetChatSessionDTO userPetChatSessionDTO)
    {
        InitializeComponent();
        this.userPetChatSessionDTO = userPetChatSessionDTO;
        BindingContext = this;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await GetMessagesAsync();
        await EnsureWebSocketConnected();
    }
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        var _ = CloseWebSocket(); // Chamada assíncrona sem await
    }

    private async Task GetMessagesAsync()
    {
        string jsonString = Preferences.Get("User", string.Empty);
        _User = JsonSerializer.Deserialize<UserDto>(jsonString);
        var chats = await _api.GetAsync<List<UserPetChatDTO>>($"ListMessages/{userPetChatSessionDTO.Id}");
        ChatMessages.Clear();
        foreach (var chatMessageDto in chats)
        {
            ChatMessages.Add(new ChatMessages
            {
                SessionId = userPetChatSessionDTO.Id,
                Text = chatMessageDto.Text,
                SenderUser = GetName(chatMessageDto.SenderUser.Id),
                IsSentByMe = chatMessageDto.SenderUser.Id == _User.Id,
                CorSender = chatMessageDto.SenderUser.Id == _User.Id ? Color.FromHex("#FF8A00") : Color.FromHex("#D9D9D9"),
                PositionSender = chatMessageDto.SenderUser.Id != _User.Id ? LayoutOptions.Start : LayoutOptions.End,
            });
        }
       
    }
    private async Task EnsureWebSocketConnected()
    {
        if (_webSocket.State != WebSocketState.Open)
        {
            CreateNewWebSocket();
            await ConnectToWebSocket();
        }
    }

    private async Task CloseWebSocket()
    {
        if (_webSocket.State == WebSocketState.Open)
        {
            await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
        }
        _webSocket.Dispose();
    }

    private void CreateNewWebSocket()
    {
        if (_webSocket != null)
        {
            // Encerrar o WebSocket existente se ele estiver aberto
            if (_webSocket.State == WebSocketState.Open)
            {
                var _ = _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
            }
            _webSocket.Dispose();
        }
        _webSocket = new ClientWebSocket();
    }



    private async Task ConnectToWebSocket()
    {
        try
        {
            await _webSocket.ConnectAsync(new Uri("ws://192.168.1.169:5031/chat"), CancellationToken.None);

            // Enviar o UserId logo após a conexão ser estabelecida
            var initialMessage = new { UserId = _User.Id };
            var messageString = JsonSerializer.Serialize(initialMessage);
            var buffer = new ArraySegment<byte>(System.Text.Encoding.UTF8.GetBytes(messageString));
            await _webSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);

            // Começar a ouvir mensagens do servidor
            _ = ListenForMessages();
        }
        catch (WebSocketException ex)
        {
            // Tratamento específico para exceções do WebSocket
            Console.WriteLine($"Erro ao conectar ao WebSocket: {ex.Message}");
        }
        catch (Exception ex)
        {
            // Tratamento geral para outras exceções
            Console.WriteLine($"Erro inesperado: {ex.Message}");
        }
    }

    private async void OnSend(object sender, EventArgs e)
    {
        if (_webSocket.State != WebSocketState.Open)
        {
            await EnsureWebSocketConnected();
        }
        var chatMessageDto = new ChatMessageDTO
        {
            User1Id = userPetChatSessionDTO.User1.Id,
            User2Id = userPetChatSessionDTO.User2.Id,
            SessionId = userPetChatSessionDTO.Id,
            PetId = userPetChatSessionDTO.Pet.Id,
            Text = MessageEntry.Text,
            SenderUser = _User.Id,
            ReceiverUser = userPetChatSessionDTO.User1.Id == _User.Id ? userPetChatSessionDTO.User2.Id : userPetChatSessionDTO.User1.Id
        };


        var messageString = JsonSerializer.Serialize(chatMessageDto);
        var buffer = new ArraySegment<byte>(System.Text.Encoding.UTF8.GetBytes(messageString));
        await _webSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);


        MessageEntry.Text = string.Empty;
    }

    private async Task ListenForMessages()
    {
        var buffer = new byte[1024 * 4];
        while (_webSocket.State == WebSocketState.Open)
        {
            var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            if (result.MessageType == WebSocketMessageType.Text)
            {
                var messageJson = System.Text.Encoding.UTF8.GetString(buffer, 0, result.Count);
                var chatMessageDto = JsonSerializer.Deserialize<ChatMessageDTO>(messageJson);
                if (chatMessageDto.SessionId == userPetChatSessionDTO.Id)
                {
                    ChatMessages.Add(new ChatMessages
                    {
                        SessionId = userPetChatSessionDTO.Id,
                        Text = chatMessageDto.Text,
                        SenderUser = GetName(chatMessageDto.SenderUser),
                        IsSentByMe = chatMessageDto.SenderUser == _User.Id,
                        CorSender = chatMessageDto.SenderUser == _User.Id ? Color.FromRgba("#FF8A00") : Color.FromRgba("#D9D9D9"),
                        PositionSender = chatMessageDto.SenderUser != _User.Id ? LayoutOptions.Start : LayoutOptions.End,
                    });
                }
            }
        }
    }
    private string GetName(int id)
    {
        if(id == _User.Id)
        {
            return "Você";
        }
        if (id == userPetChatSessionDTO.User1.Id)
        {
            return userPetChatSessionDTO.User1.Name;
        }
        else
        {
            return userPetChatSessionDTO.User2.Name;
        }
    }
    private async void OnFrameTapped(object sender, EventArgs e)
    {
        // Aqui, UserPetChatSessionDTO é a propriedade da sua classe ChatDetailPage
        var pet = UserPetChatSessionDTO.Pet;
        if (pet != null)
        {
            var petViewComplete = new PetViewComplete(pet);
            await Navigation.PushModalAsync(petViewComplete);
        }
    }

}


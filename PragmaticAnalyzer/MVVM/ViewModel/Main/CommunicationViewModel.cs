using PragmaticAnalyzer.Abstractions;
using PragmaticAnalyzer.Configs;
using PragmaticAnalyzer.Core;
using PragmaticAnalyzer.Enums;
using PragmaticAnalyzer.WorkingServer.Communication;
using System.Collections.ObjectModel;

namespace PragmaticAnalyzer.MVVM.ViewModel.Main
{
    public class CommunicationViewModel : ViewModelBase
    {
        private readonly IApiService _apiService;
        private readonly CancellationTokenSource _cts;
        private ChatMessage _typingMessage;
        public ObservableCollection<ChatMessage> Messages { get; } = [];
        public string UserInput { get => Get<string>(); set => Set(value); }
        public bool IsSending { get => Get<bool>(); set => Set(value); }

        public CommunicationViewModel(IApiService apiService)
        {
            IsSending = false;
            _apiService = apiService;
            _cts = new CancellationTokenSource();
            Messages.Add(new ChatMessage
            {
                Sender = MessageSender.Assistant,
                Text = "👋 Привет! Я готов к диалогу. Задавайте вопросы."
            });
        }

        public RelayCommand SendCommand => GetCommand(async o =>
        {
            await SendMessageAsync();
        }, o => !IsSending);
        public RelayCommand ClearCommand => GetCommand(o =>
        {
            ClearChat();
        });

        private async Task SendMessageAsync()
        {
            if (IsSending || string.IsNullOrWhiteSpace(UserInput)) return;

            var userMessage = UserInput.Trim();
            UserInput = string.Empty;
            IsSending = true;

            Messages.Add(new ChatMessage
            {
                Sender = MessageSender.User,
                Text = userMessage
            });

            _typingMessage = new ChatMessage
            {
                Sender = MessageSender.Assistant,
                Text = "🤔 Думаю...",
                IsTyping = true
            };
            Messages.Add(_typingMessage);

            try
            {
                var request = new RequestCommunication(userMessage, GlobalConfig.TranslatorPort);
                var response = await _apiService.SendRequestAsync<ResponseCommunication>(request, _cts.Token, 1000);

                // Удаляем индикатор
                Messages.Remove(_typingMessage);

                if (response.IsSuccess && response.Value?.Results?.Length > 0)
                {
                    var assistantText = response.Value.Results[0].Text?.Trim();
                    Messages.Add(new ChatMessage
                    {
                        Sender = MessageSender.Assistant,
                        Text = assistantText ?? "⚠️ Пустой ответ"
                    });
                }
                else
                {
                    Messages.Add(new ChatMessage
                    {
                        Sender = MessageSender.Assistant,
                        Text = $"❌ Ошибка: {response?.ErrorMessage ?? "Неизвестная ошибка"}"
                    });
                }
            }
            catch (OperationCanceledException)
            {
                Messages.Remove(_typingMessage);
                Messages.Add(new ChatMessage
                {
                    Sender = MessageSender.Assistant,
                    Text = "⏹️ Запрос отменён"
                });
            }
            catch (Exception ex)
            {
                Messages.Remove(_typingMessage);
                Messages.Add(new ChatMessage
                {
                    Sender = MessageSender.Assistant,
                    Text = $"💥 Исключение: {ex.Message}"
                });
            }
            finally
            {
                IsSending = false;
            }
        }
        private void ClearChat()
        {
            Messages.Clear();
            Messages.Add(new ChatMessage
            {
                Sender = MessageSender.Assistant,
                Text = "🧹 Чат очищен. Начнём сначала!"
            });
        }
        public void Dispose()
        {
            _cts?.Cancel();
            _cts?.Dispose();
        }
    }
    public class ChatMessage : ViewModelBase
    {
        public MessageSender Sender { get; set; }
        public string Text { get => Get<string>(); set => Set(value); }
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public bool IsTyping { get => Get<bool>(); set => Set(value); }
    }
}
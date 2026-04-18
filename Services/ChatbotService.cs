using OpenAI;
using OpenAI.Chat;

namespace Licenta.Services
{
    public class ChatbotService
    {
        private readonly OpenAIClient _client;
        private readonly string _model = "gpt-4o-mini";

        public ChatbotService(IConfiguration config)
        {
            _client = new OpenAIClient(config["OPENAI_API_KEY"]);
        }

        public async Task<string> AskAsync(IEnumerable<ChatMessage> messages)
        {
            var chat = _client.GetChatClient(_model);
            var result = await chat.CompleteChatAsync(messages);
            return result.Value.Content[0].Text;
        }
    }
}

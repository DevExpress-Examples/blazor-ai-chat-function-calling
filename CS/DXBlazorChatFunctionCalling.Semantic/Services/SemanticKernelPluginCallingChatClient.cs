using System.ComponentModel;

using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace DXBlazorChatFunctionCalling.Semantic.Services {
    public class SemanticKernelPluginCallingChatClient : IChatClient {
        public ChatClientMetadata Metadata => new ChatClientMetadata();
        private IChatCompletionService _chatCompletionService;
        private Kernel _kernel;
        private OpenAIPromptExecutionSettings _executionSettings;
        public SemanticKernelPluginCallingChatClient(Kernel kernel)
        {
            _kernel = kernel;
            _chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();
            _executionSettings = new OpenAIPromptExecutionSettings() { ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions };
        }

        public async Task<ChatResponse> GetResponseAsync(IEnumerable<ChatMessage> chatMessages, ChatOptions? options = null, CancellationToken cancellationToken = default)
        {
            var history = GetChatHistory(chatMessages);
            ChatMessageContent message = await _chatCompletionService.GetChatMessageContentAsync(history, _executionSettings, _kernel, cancellationToken);
            return new ChatResponse(new ChatMessage(ChatRole.Assistant, message.Content));
        }

        public async IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(IEnumerable<ChatMessage> chatMessages, ChatOptions? options = null, CancellationToken cancellationToken = default)
        {
            var history = GetChatHistory(chatMessages);
            await foreach(var item in _chatCompletionService.GetStreamingChatMessageContentsAsync(history, _executionSettings, _kernel, cancellationToken)) {
                yield return new ChatResponseUpdate(ChatRole.Assistant, item.Content);
            }
        }
        AuthorRole GetRole(ChatRole chatRole) {
            if(chatRole == ChatRole.User) return AuthorRole.User;
            if(chatRole == ChatRole.System) return AuthorRole.System;
            if(chatRole == ChatRole.Assistant) return AuthorRole.Assistant;
            if(chatRole == ChatRole.Tool) return AuthorRole.Tool;
            throw new Exception();
        }

        private ChatHistory GetChatHistory(IEnumerable<ChatMessage> chatMessages)
        {
            var history = new ChatHistory(chatMessages.Select(x => new ChatMessageContent(GetRole(x.Role), x.Text)));
            return history;
        }

        public void Dispose() {}

        public object? GetService(Type serviceType, object? serviceKey = null) => null;
    }
}

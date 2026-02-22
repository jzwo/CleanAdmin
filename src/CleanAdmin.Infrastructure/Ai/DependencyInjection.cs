using System.ClientModel;
using System.ComponentModel;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Hosting;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenAI;
using OpenAI.Chat;

namespace CleanAdmin.Infrastructure.Ai;

public static class DependencyInjection
{
    public static IHostApplicationBuilder AddAiInfrastructure(this IHostApplicationBuilder builder)
    {
        var openAiApiKey = builder.Configuration["OpenAI:Key"]
                           ?? throw new InvalidOperationException("Missing configuration:OpenAI:Key");
        var openAiModel = builder.Configuration["OpenAI:Model"] ?? "gpt-4o-mini";
        var openAiEndpoint = builder.Configuration["OpenAI:Endpoint"];

        var openAiOptions = new OpenAIClientOptions();
        if (!string.IsNullOrWhiteSpace(openAiEndpoint))
        {
            openAiOptions.Endpoint = new Uri(openAiEndpoint);
        }

        var chatClient = new ChatClient(openAiModel, new ApiKeyCredential(openAiApiKey), openAiOptions).AsIChatClient();
        builder.Services.AddChatClient(chatClient);

        builder.AddAIAgent("systemAssister", (_, key) => new ChatClientAgent(
            chatClient,
            name: key,
            instructions:
            "You assist users with system-related inquiries and tasks, providing accurate and helpful information.",
            tools: [AIFunctionFactory.Create(FormatStory)]
        ));

        builder.AddOpenAIResponses();
        builder.AddOpenAIConversations();

        return builder;
    }

    [Description("Formats the story for publication, revealing its title.")]
    private static string FormatStory(string title, string story) => $"""
                                                               **Title**: {title}

                                                               {story}
                                                               """;
}

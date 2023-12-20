using Azure;
using Azure.AI.OpenAI;
using static System.Environment;
using System.Text; // Para usar StringBuilder

string endpoint = GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT");
string key = GetEnvironmentVariable("AZURE_OPENAI_KEY");

OpenAIClient client = new(new Uri(endpoint), new AzureKeyCredential(key));

while (true) // Bucle infinito para múltiples preguntas
{
    Console.WriteLine("=== Ask Your Question (type 'exit' to quit) ===");
    string userQuestion = Console.ReadLine();

    if (userQuestion.ToLower() == "exit") // Salir del bucle si el usuario escribe 'exit'
    {
        break;
    }

    var chatCompletionsOptions = new ChatCompletionsOptions()
    {
        DeploymentName = GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENTGPT35"),
        Messages =
        {
            new ChatRequestSystemMessage("You are a helpful assistant."),
            new ChatRequestUserMessage(userQuestion),
        },
        MaxTokens = 100
    };

    StringBuilder responseBuilder = new StringBuilder();

    Console.WriteLine("\n=== Assistant's Response ===");

    await foreach (StreamingChatCompletionsUpdate chatUpdate in client.GetChatCompletionsStreaming(chatCompletionsOptions))
    {
        if (!string.IsNullOrEmpty(chatUpdate.ContentUpdate))
        {
            responseBuilder.Append(chatUpdate.ContentUpdate);
        }
    }

    Console.WriteLine(responseBuilder.ToString());
    Console.WriteLine("\n===========================");
}

Console.WriteLine("Goodbye!");

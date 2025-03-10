<!-- default badges list -->
![](https://img.shields.io/endpoint?url=https://codecentral.devexpress.com/api/v1/VersionRange/945988055/24.2.3%2B)
[![](https://img.shields.io/badge/Open_in_DevExpress_Support_Center-FF7200?style=flat-square&logo=DevExpress&logoColor=white)](https://supportcenter.devexpress.com/ticket/details/T1281509)
[![](https://img.shields.io/badge/📖_How_to_use_DevExpress_Examples-e9f6fc?style=flat-square)](https://docs.devexpress.com/GeneralInformation/403183)
[![](https://img.shields.io/badge/💬_Leave_Feedback-feecdd?style=flat-square)](#does-this-example-address-your-development-requirementsobjectives)
<!-- default badges end -->
# DevExpress Blazor AI Chat — Implement Function/Tool Calling

This example uses function/tool calling with the DevExpress Blazor [`DxAIChat`](https://docs.devexpress.com/Blazor/DevExpress.AIIntegration.Blazor.Chat.DxAIChat) component. Function calling allows a model to invoke external functions or APIs in response to user queries.

## What's Inside

The solution includes the following projects:

- **Project 1**: *DXBlazorChatFunctionCalling* implements function calling using the IChatClient interface from the Microsoft.Extensions.AI library.
- **Project 2**: *DXBlazorChatFunctionCalling.Semantic* creates a Microsoft Semantic Kernel plugin with a callable function.

## How Function Calling Works

1. The AI model detects when a request requires external execution.
2. The AI model identifies the appropriate function and necessary parameters. In this example, these are `CustomAIFunctions.GetWeatherTool()` and `WeatherPlugin.MyGetWhether(string city)` functions.
3. The AI model calls the function and processes the result.
4. The response is formatted and returned to the Blazor AI Chat control.

## Register AI Service

Before you run the example, register your AI service. This example uses Azure OpenAI. You’ll need to specify your credentials in *Program.cs*:

```csharp
// Replace with your endpoint, API key, and deployed AI model name.
string azureOpenAIEndpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT");
string azureOpenAIKey = Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY");
string deploymentName = string.Empty;
```

## Implementation Details

### Project 1: Tool Calling with IChatClient

- The `GetWeatherTool` function retrieves weather information for a specified city.
- The `System.ComponentModel.Description` attribute helps the AI model select and call the appropriate method (in this example this is the `GetWeather` method). By using this attribute, the AI-powered system can better analyze, select, and invoke the correct function when responding to user queries.

    ```csharp
    using System.ComponentModel;
    using Microsoft.Extensions.AI;

    namespace DXBlazorChatFunctionCalling.Services;
    public class CustomAIFunctions {
        public static AIFunction GetWeatherTool => AIFunctionFactory.Create(GetWeather);
        // Describe the function so the AI service understands its purpose
        public static AIFunction GetWeatherTool => AIFunctionFactory.Create(GetWeather); 
        [Description("Gets the current weather in the city")]
        public static string GetWeather([Description("The name of the city")] string city) {
        switch (city) {
            case "Los Angeles":
            case "LA":
            return GetTemperatureValue(20);
            case "London":
            return GetTemperatureValue(15);
            default:
            return $"The information about the weather in {city} is not available.";
            }
        }
        static string GetTemperatureValue(int value)
        {
        var valueInFahrenheits = value * 9 / 5 + 32;
        return $"{valueInFahrenheits}\u00b0F ({value}\u00b0C)";
        }
    }
    ```

- To enable function calling in a chat client, you must first configure chat client options. Once configured, call the `UseFunctionInvocation()` method to activate function invocation.

    ```csharp
    using Azure;
    using Azure.AI.OpenAI;
    using Microsoft.Extensions.AI;
    ...
    IChatClient chatClient = new ChatClientBuilder(azureChatClient)
        .ConfigureOptions(x =>
        {
            x.Tools = [CustomAIFunctions.GetWeatherTool];
        })
        .UseFunctionInvocation()
        .Build();

    builder.Services.AddChatClient(chatClient);
    ```

When a user asks the AI Chat about the weather, the AI model automatically calls the `GetWeatherTool` function and returns the formatted result to the AI Chat control.

### Project 2: Integrate Microsoft Semantic Kernel Plugins

The *DXBlazorChatFunctionCalling.Semantic* project implements a custom `IChatClient` to invoke `IChatCompletionService` methods from the Semantic Kernel. DevExpress AI-powered APIs use this interface to operate with LLMs.

The `WeatherPlugin` class implements a Microsoft Semantic Kernel plugin. The Microsoft.SemanticKernel.KernelFunction attribute annotates the `GetWeather()` method as a callable function within the Semantic Kernel runtime.

```csharp
using Microsoft.SemanticKernel;
using System.ComponentModel;
 
public class WeatherPlugin
{
    [KernelFunction]
    [Description("Gets the current weather in the city, returning a value in Celsius")]
    public static string GetWeather([Description("The name of the city")] string city) {
        switch(city) {
            case "Los Angeles":
            case "LA":
                return GetTemperatureValue(20);
            case "London":
                return GetTemperatureValue(15);
            default:
                return $"The information about the weather in {city} is not available.";
        }
    }
    static string GetTemperatureValue(int value)
    {
      var valueInFahrenheits = value * 9 / 5 + 32;
      return $"{valueInFahrenheits}\u00b0F ({value}\u00b0C)";
    }
}
```

## Files to Review

**DXBlazorChatFunctionCalling**

* [Program.cs](./CS/DXBlazorChatFunctionCalling/Program.cs)
* [CustomAIFunctions.cs](./CS/DXBlazorChatFunctionCalling/Services/CustomAIFunctions.cs)

**DXBlazorChatFunctionCalling.Semantic**

* [Program.cs](./CS/DXBlazorChatFunctionCalling.Semantic/Program.cs)
* [WeatherPlugin.cs](./CS/DXBlazorChatFunctionCalling.Semantic/Services/WeatherPlugin.cs)
* [SemanticKernelPluginCallingChatClient.cs](./CS/DXBlazorChatFunctionCalling.Semantic/Services/SemanticKernelPluginCallingChatClient.cs)

## Documentation

* [DevExpress Blazor AI Chat — Implement Function Calling (Blog Post)](https://community.devexpress.com/blogs/aspnet/archive/2025/02/26/devexpress-blazor-ai-chat-implement-function-calling.aspx)
* [DevExpress AI-powered Extensions for Blazor](https://docs.devexpress.com/Blazor/405228/ai-powered-extensions)
* [DevExpress Blazor AI Chat Control](https://docs.devexpress.com/Blazor/DevExpress.AIIntegration.Blazor.Chat.DxAIChat)

## Online Demo

* [AI-powered Extensions: AI Chat](https://demos.devexpress.com/blazor/AI/Chat#Overview)

## More Examples

- [Blazor AI Chat — How to add the DevExpress Blazor AI Chat component to your next Blazor, MAUI, WPF, and WinForms application](https://github.com/DevExpress-Examples/devexpress-ai-chat-samples)
* [Blazor Grid and Report Viewer — Incorporate an AI Assistant (Azure OpenAI) in your next DevExpress-powered Blazor app](https://github.com/DevExpress-Examples/blazor-grid-and-report-viewer-integrate-ai-assistant)


<!-- feedback -->
## Does this example address your development requirements/objectives?

[<img src="https://www.devexpress.com/support/examples/i/yes-button.svg"/>](https://www.devexpress.com/support/examples/survey.xml?utm_source=github&utm_campaign=blazor-ai-chat-function-calling&~~~was_helpful=yes) [<img src="https://www.devexpress.com/support/examples/i/no-button.svg"/>](https://www.devexpress.com/support/examples/survey.xml?utm_source=github&utm_campaign=blazor-ai-chat-function-calling&~~~was_helpful=no)

(you will be redirected to DevExpress.com to submit your response)
<!-- feedback end -->

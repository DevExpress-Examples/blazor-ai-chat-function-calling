using Azure;
using Azure.AI.OpenAI;
using DXBlazorChatFunctionCalling.Components;
using DXBlazorChatFunctionCalling.Services;
using Microsoft.Extensions.AI;

var builder = WebApplication.CreateBuilder(args);

// Replace with your endpoint, API key, and deployed AI model name.
string azureOpenAIEndpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT");
string azureOpenAIKey = Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY");
string deploymentName = string.Empty;

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();


builder.Services.AddDevExpressBlazor(options => { options.BootstrapVersion = DevExpress.Blazor.BootstrapVersion.v5; });
builder.Services.AddMvc();

 var azureChatClient = new AzureOpenAIClient(
     new Uri(azureOpenAIEndpoint),
     new AzureKeyCredential(azureOpenAIKey)).AsChatClient(deploymentName);

 IChatClient chatClient = new ChatClientBuilder(azureChatClient)
     .ConfigureOptions(x =>
     {
         x.Tools = [CustomAIFunctions.GetWeatherTool];
     })
     .UseFunctionInvocation()
     .Build();

builder.Services.AddChatClient(chatClient);
builder.Services.AddDevExpressAI();

var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
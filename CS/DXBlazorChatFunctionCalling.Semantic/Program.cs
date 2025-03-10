using DXBlazorChatFunctionCalling.Semantic.Components;
using DXBlazorChatFunctionCalling.Semantic.Services;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Plugins.Core;

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

var semanticKernelBuilder = Kernel.CreateBuilder();
semanticKernelBuilder.AddAzureOpenAIChatCompletion(
    deploymentName,
    azureOpenAIEndpoint,
    azureOpenAIKey);

// Add plugins from Microsoft.SemanticKernel.Plugins.Core
#pragma warning disable SKEXP0050
semanticKernelBuilder.Plugins.AddFromType<TimePlugin>();
semanticKernelBuilder.Plugins.AddFromType<WeatherPlugin>();
#pragma warning restore SKEXP0050

var globalKernel = semanticKernelBuilder.Build();
builder.Services.AddChatClient(new SemanticKernelPluginCallingChatClient(globalKernel));

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
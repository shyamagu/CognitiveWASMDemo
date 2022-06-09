using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorCognitiveWASM;
using BlazorCognitiveWASM.Model;
using BlazorCognitiveWASM.Model.Face;
using BlazorCognitiveWASM.Model.Read;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddSingleton<AuthManager>();
builder.Services.AddSingleton<FaceService>();
builder.Services.AddSingleton<ReadService>();


await builder.Build().RunAsync();

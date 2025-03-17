using Hometask;
using Hometask.Components;
using HometaskLib.Application;
using HometaskLib.Providers.Fixer;
using HometaskLib.UnitOfWork;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddServerSideBlazor();

builder.Services.AddHttpClient("HttpsClient", client =>
{
    client.BaseAddress = new Uri("https://localhost:7224/");
});
builder.Services.AddHttpClient("HttpClient", client =>
{
    client.BaseAddress = new Uri("http://localhost:5135/");
});

builder.Services.AddTransient<IFixerProvider, FixerProvider>();
builder.Services.AddScoped<ICurrencyService, CurrencyService>();
builder.Services.AddTransient<IUnitOfWorkFactory, UnitOfWorkFactory>();
builder.Services.AddHostedService<DailyExchangeRateService>();

var app = builder.Build();

app.UseCors(c => c.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
app.UseStaticFiles();
app.UseRouting();

app.UseAuthorization();
app.UseAntiforgery();
app.MapControllers();
app.MapBlazorHub();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
using ClinicTracking.Client.Components;
using ClinicTracking.Client.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add HttpClient for API calls - configured for server-side calls
builder.Services.AddHttpClient<IPatientService, PatientService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5014/");
});

builder.Services.AddHttpClient<ITreatmentService, TreatmentService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5014/");
});

builder.Services.AddHttpClient<IImportService, ImportService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5014/");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
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

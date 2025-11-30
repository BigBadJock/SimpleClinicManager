using ClinicTracking.API.Authentication;
using ClinicTracking.API.Services;
using ClinicTracking.Core.Interfaces;
using ClinicTracking.Infrastructure.Data;
using ClinicTracking.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Configure Entity Framework
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    // Use In-Memory database for development/testing if no connection string is provided
    builder.Services.AddDbContext<ClinicTrackingDbContext>(options =>
        options.UseInMemoryDatabase("ClinicTrackingDb"));
}
else
{
    builder.Services.AddDbContext<ClinicTrackingDbContext>(options =>
        options.UseSqlServer(connectionString));
}

// Register repositories and Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Register export service
builder.Services.AddScoped<IExportService, ExportService>();

// Register import service
builder.Services.AddScoped<IImportService, ImportService>();

// Configure Authentication (commented out for development, uncomment for production)
/*
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));
*/

// For development, we'll skip authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "Development";
})
.AddScheme<AuthenticationSchemeOptions, DevelopmentAuthenticationHandler>(
    "Development", options => { });

builder.Services.AddAuthorization();

// Add API Explorer and Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Clinic Tracking API", Version = "v1" });
});

// Add CORS for development
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient",
        policy =>
        {
            policy.WithOrigins("https://localhost:7000", "http://localhost:5000")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowBlazorClient");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Initialize database with sample data in development
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ClinicTrackingDbContext>();
    
    // Ensure database is created for in-memory database
    await context.Database.EnsureCreatedAsync();
    
    // Add sample data if empty
    //if (!context.PatientTrackings.Any())
    //{
    //    // First add sample treatments
    //    var sampleTreatments = new[]
    //    {
    //        new ClinicTracking.Core.Entities.Treatment
    //        {
    //            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
    //            Name = "Chemotherapy",
    //            Description = "Standard chemotherapy treatment",
    //            CreatedBy = "system",
    //            CreatedOn = DateTime.UtcNow
    //        },
    //        new ClinicTracking.Core.Entities.Treatment
    //        {
    //            Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
    //            Name = "Radiation Therapy",
    //            Description = "Radiation treatment",
    //            CreatedBy = "system",
    //            CreatedOn = DateTime.UtcNow
    //        },
    //        new ClinicTracking.Core.Entities.Treatment
    //        {
    //            Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
    //            Name = "Immunotherapy",
    //            Description = "Immune system enhancement therapy",
    //            CreatedBy = "system",
    //            CreatedOn = DateTime.UtcNow
    //        },
    //        new ClinicTracking.Core.Entities.Treatment
    //        {
    //            Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
    //            Name = "Targeted Therapy",
    //            Description = "Targeted drug therapy",
    //            CreatedBy = "system",
    //            CreatedOn = DateTime.UtcNow
    //        },
    //        new ClinicTracking.Core.Entities.Treatment
    //        {
    //            Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
    //            Name = "Hormone Therapy",
    //            Description = "Hormonal treatment",
    //            CreatedBy = "system",
    //            CreatedOn = DateTime.UtcNow
    //        }
    //    };

    //    context.Treatments.AddRange(sampleTreatments);
    //    await context.SaveChangesAsync();

    //    var samplePatients = new[]
    //    {
    //        new ClinicTracking.Core.Entities.PatientTracking
    //        {
    //            Id = Guid.NewGuid(),
    //            MRN = "MRN001",
    //            Name = "John Smith",
    //            ReferralDate = DateTime.Today.AddDays(-30),
    //            CreatedBy = "system",
    //            CreatedOn = DateTime.UtcNow
    //        },
    //        new ClinicTracking.Core.Entities.PatientTracking
    //        {
    //            Id = Guid.NewGuid(),
    //            MRN = "MRN002",
    //            Name = "Jane Doe",
    //            ReferralDate = DateTime.Today.AddDays(-20),
    //            CounsellingDate = DateTime.Today.AddDays(-10),
    //            TreatmentId = Guid.Parse("22222222-2222-2222-2222-222222222222"), // Radiation Therapy
    //            CreatedBy = "system",
    //            CreatedOn = DateTime.UtcNow
    //        },
    //        new ClinicTracking.Core.Entities.PatientTracking
    //        {
    //            Id = Guid.NewGuid(),
    //            MRN = "MRN003",
    //            Name = "Bob Johnson",
    //            ReferralDate = DateTime.Today.AddDays(-15),
    //            CounsellingDate = DateTime.Today.AddDays(-5),
    //            DispensedDate = DateTime.Today.AddDays(-2),
    //            TreatmentId = Guid.Parse("11111111-1111-1111-1111-111111111111"), // Chemotherapy
    //            NextAppointment = DateTime.Today.AddDays(7),
    //            CreatedBy = "system",
    //            CreatedOn = DateTime.UtcNow
    //        }
    //    };

    //    context.PatientTrackings.AddRange(samplePatients);
    //    await context.SaveChangesAsync();
    //}
}

app.Run();

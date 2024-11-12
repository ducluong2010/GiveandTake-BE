using GiveandTake_API.Extensions;
using Giveandtake_Business;
using Giveandtake_Business.Utils;
using Giveandtake_Services.Implements;
using Giveandtake_Services.Interfaces;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;

});

builder.Services.AddServices(builder.Configuration);
builder.Services.AddJwtValidation(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddConfigSwagger();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IMembershipService, MembershipService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<DonationService>();
builder.Services.AddScoped<AccountBusiness>();
builder.Services.AddHostedService<PremiumExpirationService>();
builder.Services.AddLogging();

builder.Services.AddCors(o =>
{
    o.AddPolicy("AllowAnyOrigin", corsPolicyBuilder =>
    {
        corsPolicyBuilder
            .SetIsOriginAllowed(x => _ = true)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseStaticFiles();

app.UseCors("AllowAnyOrigin");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

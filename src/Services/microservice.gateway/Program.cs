using Google.Api;
using microservice.common;
using microservice.core;
using microservice.gateway;
using microservice.gateway.Services;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

var jsonOpt = new JsonSerializerOptions()
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    PropertyNameCaseInsensitive = true,
};

string myAllowSpecificOrigins = "local";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: myAllowSpecificOrigins,
                      policy =>
                      {
                          policy//.WithOrigins("http://localhost:8080")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowAnyOrigin()
                          .AllowCredentials()
                          .WithOrigins("http://localhost:8080");
                          //.WithOrigins("*");
                      });
});

// Add services to the container.
builder.Services
       .AddControllers()
       .AddDapr(opt => opt.UseJsonSerializationOptions(jsonOpt));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddOptions<ServicesOptions>().Bind(builder.Configuration.GetSection("Services"));

builder.Services.AddSignalR(hubOptions =>
{
    hubOptions.EnableDetailedErrors = true;
})
.AddJsonProtocol(options => {
        options.PayloadSerializerOptions.PropertyNamingPolicy = null;
})
.AddNewtonsoftJsonProtocol();

builder.Services.AddHttpContextAccessor();

builder.Services.AddSingleton<ICurrentUserService, CurrentUserService>();
//builder.Services.AddSingleton<IUserIdProvider, CustomSignalRUserIdProvider>();
builder.Services.AddSingleton<INotificationHub, NotificationService>();


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

//app.UseHttpsRedirection();
app.UseRouting();
app.UseCors(myAllowSpecificOrigins);

app.UseCloudEvents();
app.UseAuthorization();

app.MapControllers();
app.MapSubscribeHandler();

app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<NotificationHub>("/notifications-event-hub", options =>
    {
        options.Transports = HttpTransportType.ServerSentEvents;
        endpoints.MapControllerRoute(
            name: "default",
            pattern: "{controller}/{action=Index}/{id?}");
    });
});

app.Run();

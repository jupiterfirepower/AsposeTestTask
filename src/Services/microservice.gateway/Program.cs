using Google.Api;
using Google.Protobuf.Reflection;
using microservice.gateway;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
                          policy//.WithOrigins("http://localhost:8081", "http://www.contoso.com")
                          .AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod(); 
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
//builder.Services.AddOptions<ServicesOptions>().Bind(builder.Configuration.Get<ServicesOptions>());

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

//app.UseHttpsRedirection();

app.UseCors(myAllowSpecificOrigins);

app.UseCloudEvents();
app.UseAuthorization();

app.MapControllers();
app.MapSubscribeHandler();

app.Run();

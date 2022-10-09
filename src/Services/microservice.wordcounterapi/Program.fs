namespace microservice.wordcounterapi

open System.Text.Json
open System

#nowarn "20"
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open microservice.wordcounterapi.ProvidedTypes

module Program =
    let exitCode = 0

    [<EntryPoint>]
    let main args =

        let builder = WebApplication.CreateBuilder(args)

        let jsonOpt = new JsonSerializerOptions()

        jsonOpt.PropertyNamingPolicy <- JsonNamingPolicy.CamelCase
        jsonOpt.PropertyNameCaseInsensitive <- true

        // Add services to the container.
        builder.Services
               .AddControllers()
               .AddDapr(fun opt -> opt.UseJsonSerializationOptions(jsonOpt) |> ignore)

        //configuration using FSharp.Data type provider
        let settingsFile = "appsettings." + Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") + ".json"
        let config = AppSettingsProvider.Load(settingsFile)
        builder.Services.AddSingleton<AppSettings>(config)

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer()
        builder.Services.AddSwaggerGen()

        let app = builder.Build()

        app.UseSwagger()
        app.UseSwaggerUI()

        //app.UseHttpsRedirection()
        app.UseAuthorization()

        app.UseCloudEvents()
        app.MapControllers()
        app.MapSubscribeHandler()

        app.Run()

        exitCode

namespace microservice.wordcounterapi

open FSharp.Data

[<AutoOpen>]
module ProvidedTypes = 
    type AppSettingsProvider = JsonProvider<"appsettings.json">

    type AppSettings = AppSettingsProvider.Root


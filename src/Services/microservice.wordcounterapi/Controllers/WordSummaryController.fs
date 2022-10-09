namespace microservice.wordcounterapi.Controllers

open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Logging
open Dapr.Client
open microservice.common
open microservice.dto.webdata
open Dapr
open FSharp.Control
open System
open System.Linq
open System.Collections.Generic
open microservice.wordcounterapi.data
open Aspose.NLP.FSharp.Core
open System.Net.Http
open microservice.wordcounterapi.ProvidedTypes

[<ApiController>]
[<Route("[controller]")>]
type WordSummaryController (logger : ILogger<WordSummaryController>, settings: AppSettings) =
    inherit ControllerBase()

    [<HttpGet>]
    member _.Get([<FromState(StoreNames.WordsSummaryStoreName)>]state:StateEntry<WebSummaryState>): WordItemSummary[] =
        logger.LogInformation($"Word Summary get result")

        let result = state.Value.Data 
                     |> Seq.map(fun (word, count, per, n) -> let ws = new WordItemSummary(word, count.ToString() + " " + "(" + (sprintf "%.2f" per) + "%)", n)
                                                             ws)
           
        result |> Seq.toArray

    [<HttpGet("{corellationId}")>]
    member _.Get(corellationId: string, [<FromServices>] daprClient : DaprClient): WordItemSummary[] =
        logger.LogInformation($"Word Summary get by corellationId: {corellationId}")
        let mutable result: seq<_> = Seq.empty
        task {
            let! state = daprClient.GetStateEntryAsync<WebSummaryState>(StoreNames.WordsSummaryStoreName, corellationId)
            let inline notNull value = not (obj.ReferenceEquals(value, null))

            if notNull state.Value then

               result <- state.Value.Data |> Seq.filter (fun (word, count, per, n) -> word.Length <> 0)
               |> Seq.map(fun (word, count, per, n) -> if count = 0 then (word, count + 1, per, n) else (word, count, per, n))
               |> Seq.sortBy (fun (word, count, per, n) -> -count)
               |> Seq.map(fun (word, count, per, n) -> let ws = new WordItemSummary(word, count.ToString() + " " + "(" + (sprintf "%.2f" per) + "%)", n)
                                                       ws)
        }
        |> Async.AwaitTask
        |> Async.RunSynchronously
           
        result |> Seq.toArray
        
    [<Topic(PubSubs.PubSub, Topics.WebTextDataTopicName)>]
    [<HttpPost>]
    member _.WordSummary(input : WebUriData, [<FromServices>] daprClient : DaprClient) =
        logger.LogInformation($"Text Word Summary Processing Url - {input.Url}")

        let text = input.Data

        let text_without_special = (removeSpecial text)

        let nWords = numWords text_without_special

        logger.LogInformation($"Text Word Summary Processing words count - {nWords}")

        let nSentenses = numSentences text  

        logger.LogInformation($"Text Word Summary Processing Sentences count - {nSentenses}")

        let nCharacters = text.Length

        logger.LogInformation($"Text Word Summary Processing Characters count - {nCharacters}")

        let nCharactersWithoutSpaces = text.Replace(" ", String.Empty).Length

        let nSpecialCharacters = nCharacters - text_without_special.Length

        logger.LogInformation($"Text Word Summary Processing Characters without spaces count - {nSpecialCharacters}")

        let result = swords_count(text)

        logger.LogInformation($"Text Word Summary Processing one word count - {Seq.length result}")

        let hset_result = new HashSet<string*int*float*int>()

        result |> Seq.iter (fun r -> hset_result.Add(r) |> ignore)

        for n = 2 to input.NWord do
            let dict = phrasegen_count text n
            logger.LogInformation($"Text Word Summary Processing phrasegen_count({n}) finished. Phrase count - {dict.Values.Count}")
            let result_tmp = pairs dict 

            let count = result_tmp.Count()
            logger.LogInformation($"Text Word Summary Processing phrasegen_count({n}). Phrase count - {count}")

            let result_n = result_tmp |> Seq.map (fun (w, c) -> (w, c , (float c)/(float count), n))
            result_n |> Seq.iter (fun r -> hset_result.Add(r) |> ignore)

        task {
            let! state = daprClient.GetStateEntryAsync<WebSummaryState>(StoreNames.WordsSummaryStoreName, input.CorrelationId.ToString())

            state.Value <- { CreatedOn = DateTime.UtcNow; CorrelationId = input.CorrelationId; Url = input.Url; 
                             Data = hset_result |> Seq.toArray; NWords = nWords; NSentenses = nSentenses; 
                             NCharacters = nCharacters; NCharactersNoSpaces = nCharactersWithoutSpaces; NSpecial = nSpecialCharacters }

            do! state.SaveAsync()

            logger.LogInformation($"Text Word Summary Processing WebSummaryState saved with key - {state.Key}")

            let notif = new Notification()
            notif.CorellationId <- input.CorrelationId.ToString()
            notif.Created <- DateTime.UtcNow

            let client = new HttpClient()
            let! g = client.PostAsJsonAsync(settings.Services.GateWayUrl, notif);
            
            let! corellationId = g.Content.ReadAsAsync<string>();
            logger.LogInformation((sprintf "Text Word Summary PostAsJsonAsync corellationId - %A" corellationId))
        }
        |> Async.AwaitTask
        |> Async.RunSynchronously

       

        hset_result |> Seq.toArray


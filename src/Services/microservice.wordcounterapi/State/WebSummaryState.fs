namespace microservice.wordcounterapi

module data =

 open System

 type WebSummaryState = 
    { CreatedOn: DateTime
      CorrelationId: Guid
      Url: string
      Data: seq<string * int * float * int>
      NWords: int
      NSentenses: int 
      NCharacters: int
      NCharactersNoSpaces: int
      NSpecial: int
      }


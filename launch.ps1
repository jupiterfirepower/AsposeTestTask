dapr run --app-id "webdownloader-service" --app-port "5001" --dapr-grpc-port "50010" --dapr-http-port "5010" --components-path "./components" -- dotnet run --project ./src/Services/microservice.webdownloaderapi/microservice.webdownloaderapi.csproj --urls="http://+:5001" 
dapr run --app-id "htmlparser-service" --app-port "5002" --dapr-grpc-port "50020" --dapr-http-port "5020" --components-path "./components" -- dotnet run --project ./src/Services/microservice.webhtmlparserapi/microservice.webhtmlparserapi.csproj --urls="http://+:5002"
dapr run --app-id "wordcounter-service" --app-port "5003" --dapr-grpc-port "50030" --dapr-http-port "5030" --components-path "./components" -- dotnet run --project ./src/Services/microservice.wordcounterapi/microservice.wordcounterapi.fsproj --urls="http://+:5003"







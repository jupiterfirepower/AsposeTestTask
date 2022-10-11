dapr run --app-id "webdownloader-service" --app-port "5001" --dapr-grpc-port "50010" --dapr-http-port "5010" --components-path "./components" -- dotnet run --project ./src/Services/microservice.webdownloaderapi/microservice.webdownloaderapi.csproj --urls="http://+:5001" 
dapr run --app-id "htmlparser-service" --app-port "5002" --dapr-grpc-port "50020" --dapr-http-port "5020" --components-path "./components" -- dotnet run --project ./src/Services/microservice.webhtmlparserapi/microservice.webhtmlparserapi.csproj --urls="http://+:5002"
dapr run --app-id "wordcounter-service" --app-port "5003" --dapr-grpc-port "50030" --dapr-http-port "5030" --components-path "./components" -- dotnet run --project ./src/Services/microservice.wordcounterapi/microservice.wordcounterapi.fsproj --urls="http://+:5003"
#dapr run --app-id "gateway-service" --app-port "5004" --dapr-grpc-port "50040" --dapr-http-port "5040" --components-path "./components" -- dotnet run --project ./src/Services/microservice.gateway/microservice.gateway.csproj --urls="http://+:5004"


host.docker.internal


$password = "pwd1234567890"
ConvertTo-SecureString $password -AsPlainText -Force

openssl pkcs12 -in ../certificates/cosmosdbemulator.pfx -clcerts -nokeys -out cosmosdbemulator.crt -passin pass:pwd1234567890

cp cosmosdbemulator.crt /usr/local/share/ca-certificates/





cp $CERT_TO_TRUST /usr/local/share/ca-certificates/


.\Microsoft.Azure.Cosmos.Emulator.exe /GenCert=host.docker.internal
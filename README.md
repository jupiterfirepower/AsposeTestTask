# Aspose Test Task
</br>
# How to run on Windows 10 or 11.
# Install Docker</br>
https://docs.docker.com/desktop/install/windows-install/</br>
</br>
https://desktop.docker.com/win/main/amd64/Docker%20Desktop%20Installer.exe</br>
</br>
# Install WSL 2</br> 
https://wslstorestorage.blob.core.windows.net/wslblob/wsl_update_x64.msi</br>
# Power shell command (in Windows Terminal)</br>
wsl --set-default-version 2
</br>
# Install Node and npm</br>
https://nodejs.org/en/download/</br>
node -v</br>
npm - v</br>
# Install Yarn</br>
npm install --global yarn</br>
yarn --version</br>
# Install Dapr CLI</br>
https://docs.dapr.io/getting-started/install-dapr-cli/</br>
</br>
Install the latest Dapr runtime binaries:</br>
dapr init</br>
</br>
# Install Azure Cosmos DB Emulator</br>
https://learn.microsoft.com/en-us/azure/cosmos-db/local-emulator?tabs=ssl-netstd21</br>
powershell -Command "iwr -useb https://raw.githubusercontent.com/dapr/cli/master/install/install.ps1 | iex"</br>
Change default port in Azure Cosmos DB Emulator Properties</br> 
- "C:\Program Files\Azure Cosmos DB Emulator\Microsoft.Azure.Cosmos.Emulator.exe" -port=8084</br>
add -port=8084</br>
Open in browser</br>
https://localhost:8084/_explorer/index.html</br>
Create database - state</br>
Create containers:</br>
webdatastate</br>
webdownloaderstate</br>
websumstate</br>

with key /partitionKey</br>

# Install Git Client for Windows</br>
https://git-scm.com/downloads</br>

Create dir for source codes.</br>
ASPOSE - for example</br>
cd ./ASPOSE</br>
git clone https://github.com/jupiterfirepower/AsposeTestTask.git .</br>
. - mean current directory(ASPOSE)</br>
</br>
# How to run local</br>
</br>
# Run Docker.(click on Desktop icon)</br>
# Run Terminal or Terminal Preview on Windows 10.</br>
Run command in terminal </br> 
cd to root dir ASPOSE</br> 
docker-compose up</br>
File in root source dir launch.ps1 contains command for run microservices in Dapr</br>
Run five terminals and run command(one for terminal)</br>
dapr run --app-id "webdownloader-service" --app-port "5001" --dapr-grpc-port "50010" --dapr-http-port "5010" --components-path "./components" -- dotnet run --project ./src/Services/microservice.webdownloaderapi/microservice.webdownloaderapi.csproj --urls="http://+:5001" </br>
dapr run --app-id "htmlparser-service" --app-port "5002" --dapr-grpc-port "50020" --dapr-http-port "5020" --components-path "./components" -- dotnet run --project ./src/Services/microservice.webhtmlparserapi/microservice.webhtmlparserapi.csproj --urls="http://+:5002" </br>
dapr run --app-id "wordcounter-service" --app-port "5003" --dapr-grpc-port "50030" --dapr-http-port "5030" --components-path "./components" -- dotnet run --project ./src/Services/microservice.wordcounterapi/microservice.wordcounterapi.fsproj --urls="http://+:5003"</br>
</br>
cd C:\PROJECTS\ASPOSE\src\Services\microservice.gateway  #- for example</br>
dotnet run</br>
</br>
cd C:\PROJECTS\ASPOSE\ui\wordcounter-app #- for example</br>
yarn serve</br>
</br>
App running at:</br>
  - Local:   http://localhost:8080/</br>
  - Network: http://192.168.50.193:8080/</br>
</br>
open in browser - http://localhost:8080/ (Ctrl + click mouse on link in terminal)</br>
</br>
.NET 6, C#/F#, Dapr, Asp.net WebApi, SignalR, TypeScript, Vue, Vuex, Vuetify(material ui)</br>
Architecture - Microservices, service bus(azure service bus, local used RabbitMQ)</br>
</br>
<img src="/img/DaprComponents1.jpg" width="650" title="dapr"></br>
</br>
<img src="/img/wordcount.jpg" width="650" title="dapr"></br>




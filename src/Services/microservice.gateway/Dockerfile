#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Services/microservice.gateway/microservice.gateway.csproj", "src/Services/microservice.gateway/"]
RUN dotnet restore "src/Services/microservice.gateway/microservice.gateway.csproj"
COPY . .
WORKDIR "/src/src/Services/microservice.gateway"
RUN dotnet build "microservice.gateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "microservice.gateway.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "microservice.gateway.dll"]

COPY ["/src/scripts/", "./scripts/"]
COPY ["/src/certificates/", "./certificates/"]
#WORKDIR /app/scripts
#RUN chmod +x trust_cosmos_db_emulator_crt.sh
#RUN sed -i 's/\r$//' trust_cosmos_db_emulator_crt.sh
#RUN ./trust_cosmos_db_emulator_crt.sh
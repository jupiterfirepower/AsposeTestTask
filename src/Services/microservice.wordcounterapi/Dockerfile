#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["microservice.wordcounterapi/microservice.wordcounterapi.fsproj", "microservice.wordcounterapi/"]
RUN dotnet restore "microservice.wordcounterapi/microservice.wordcounterapi.fsproj"
COPY . .
WORKDIR "/src/microservice.wordcounterapi"
RUN dotnet build "microservice.wordcounterapi.fsproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "microservice.wordcounterapi.fsproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "microservice.wordcounterapi.dll"]
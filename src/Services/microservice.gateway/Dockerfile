#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["microservice.gateway/microservice.gateway.csproj", "microservice.gateway/"]
RUN dotnet restore "microservice.gateway/microservice.gateway.csproj"
COPY . .
WORKDIR "/src/microservice.gateway"
RUN dotnet build "microservice.gateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "microservice.gateway.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "microservice.gateway.dll"]
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Weather/Weather.csproj", "Weather/"]
COPY ["Weather.sln", "."]

RUN dotnet restore "Weather/Weather.csproj"

COPY . .

WORKDIR "/src/Weather"
RUN dotnet build "Weather.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Weather.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Weather.dll"]

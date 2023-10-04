# Use the official image as a parent image
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

# Use the SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["./src/UrlShortener/UrlShortener.csproj", "./"]
RUN dotnet restore "UrlShortener.csproj"
COPY ./src/UrlShortener/ .
RUN dotnet build "UrlShortener.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "UrlShortener.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UrlShortener.dll"]

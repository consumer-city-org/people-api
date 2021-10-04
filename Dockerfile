#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app

EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["ConsumerCity.People.API/ConsumerCity.People.API.csproj", "ConsumerCity.People.API/"]
RUN dotnet restore "ConsumerCity.People.API/ConsumerCity.People.API.csproj"
COPY . .
WORKDIR "/src/ConsumerCity.People.API"
RUN dotnet build "ConsumerCity.People.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ConsumerCity.People.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ConsumerCity.People.API.dll"]
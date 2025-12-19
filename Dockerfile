FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY Calls.Api/Calls.Api.csproj ./Calls.Api/
RUN dotnet restore ./Calls.Api/Calls.Api.csproj

COPY . .
WORKDIR /src/Calls.Api
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "Calls.Api.dll"]
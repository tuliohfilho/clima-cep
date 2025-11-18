FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /build

COPY ./src/backend/. ./src

RUN dotnet restore src/Clima.Cep.Api/Clima.Cep.Api.csproj
RUN dotnet publish src/Clima.Cep.Api/Clima.Cep.Api.csproj -c Release -o src/Clima.Cep.Api/dist

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
COPY --from=build-env /build/src/Clima.Cep.Api/dist ./

ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://*:80
EXPOSE 80

ENTRYPOINT ["dotnet", "Clima.Cep.Api.dll"]
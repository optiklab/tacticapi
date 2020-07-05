FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /build
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o /api
#FROM mcr.microsoft.com/dotnet/core/runtime:3.1 AS final
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS final
WORKDIR /api
COPY --from=build /api .
ENV ASPNETCORE_URLS http://*:5000
#ENV ASPNETCORE_URLS http://*:80
ENV ASPNETCORE_ENVIRONMENT docker
ENTRYPOINT dotnet TacTicApi.dll
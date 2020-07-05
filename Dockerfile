FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /build
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o /api
#FROM mcr.microsoft.com/dotnet/core/runtime:3.1 AS final
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS final
WORKDIR /api
COPY --from=build /api .

##
#Config for my local machine:
#ENV ASPNETCORE_URLS http://*:5000
#ENV ASPNETCORE_URLS http://*:80

ENV ASPNETCORE_URLS=https://+:443;http://+:80
ENV ASPNETCORE_HTTPS_PORT=443
ENV ASPNETCORE_Kestrel__Certificates__Development__Password=luckystrike
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/https/TacTicApi.pfx
##

ENV ASPNETCORE_ENVIRONMENT docker
ENTRYPOINT dotnet TacTicApi.dll
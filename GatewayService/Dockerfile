# -------- BUILD STAGE --------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY GatewayService/GatewayService.csproj GatewayService/
RUN dotnet restore GatewayService/GatewayService.csproj

COPY GatewayService GatewayService
WORKDIR /src/GatewayService
RUN dotnet publish -c Release -o /app/publish

# -------- RUNTIME STAGE --------
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 6001
ENTRYPOINT ["dotnet", "GatewayService.dll"]


FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["InnoShop.ApiGateway/InnoShop.ApiGateway.csproj", "InnoShop.ApiGateway/"]
RUN dotnet restore "InnoShop.ApiGateway/InnoShop.ApiGateway.csproj"

COPY . .
WORKDIR "/src/InnoShop.ApiGateway"
RUN dotnet publish "InnoShop.ApiGateway.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .
	
ENTRYPOINT ["dotnet", "InnoShop.ApiGateway.dll"]

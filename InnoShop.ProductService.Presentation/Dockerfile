FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["InnoShop.ProductService.Presentation/InnoShop.ProductService.Presentation.csproj", "InnoShop.ProductService.Presentation/"]
COPY ["InnoShop.ProductService.Application/InnoShop.ProductService.Application.csproj", "InnoShop.ProductService.Application/"]
COPY ["InnoShop.ProductService.Domain/InnoShop.ProductService.Domain.csproj", "InnoShop.ProductService.Domain/"]
COPY ["InnoShop.ProductService.Infrastructure/InnoShop.ProductService.Infrastructure.csproj", "InnoShop.ProductService.Infrastructure/"]

RUN dotnet restore "InnoShop.ProductService.Presentation/InnoShop.ProductService.Presentation.csproj"

COPY . .

WORKDIR "/src/InnoShop.ProductService.Presentation"
RUN dotnet build "InnoShop.ProductService.Presentation.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "InnoShop.ProductService.Presentation.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "InnoShop.ProductService.Presentation.dll"]

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["InnoShop.UserService.Presentation/InnoShop.UserService.Presentation.csproj", "InnoShop.UserService.Presentation/"]
COPY ["InnoShop.UserService.Application/InnoShop.UserService.Application.csproj", "InnoShop.UserService.Application/"]
COPY ["InnoShop.UserService.Domain/InnoShop.UserService.Domain.csproj", "InnoShop.UserService.Domain/"]
COPY ["InnoShop.UserService.Infrastructure/InnoShop.UserService.Infrastructure.csproj", "InnoShop.UserService.Infrastructure/"]

RUN dotnet restore "InnoShop.UserService.Presentation/InnoShop.UserService.Presentation.csproj"

COPY . .

WORKDIR "/src/InnoShop.UserService.Presentation"
RUN dotnet build "InnoShop.UserService.Presentation.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "InnoShop.UserService.Presentation.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "InnoShop.UserService.Presentation.dll"]

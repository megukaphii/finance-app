#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["FinanceApp.Server/FinanceApp.Server.csproj", "FinanceApp.Server/"]
COPY ["FinanceApp.Abstractions/FinanceApp.Abstractions.csproj", "FinanceApp.Abstractions/"]
COPY ["FinanceApp.Data/FinanceApp.Data.csproj", "FinanceApp.Data/"]
COPY ["FinanceApp.Extensions.Sqlite/FinanceApp.Extensions.Sqlite.csproj", "FinanceApp.Extensions.Sqlite/"]
RUN dotnet restore "FinanceApp.Server/FinanceApp.Server.csproj"
COPY . .
WORKDIR "/src/FinanceApp.Server"
RUN dotnet build "FinanceApp.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FinanceApp.Server.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FinanceApp.Server.dll"]
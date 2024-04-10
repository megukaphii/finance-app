FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app
ENV TZ=Australia/Sydney
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["FinanceApp.Server/FinanceApp.Server.csproj", "FinanceApp.Server/"]
COPY ["FinanceApp.Data/FinanceApp.Data.csproj", "FinanceApp.Data/"]
COPY .git .git
RUN dotnet restore "FinanceApp.Server/FinanceApp.Server.csproj"
COPY . .
WORKDIR "/src/FinanceApp.Server"
RUN dotnet build "FinanceApp.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FinanceApp.Server.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 42069/tcp

ENTRYPOINT ["dotnet", "FinanceApp.Server.dll"]
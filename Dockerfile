# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:9.0-preview AS build
WORKDIR /src

# 1. Копируем только файлы решения и проектов
COPY ["pelican-magazine-backend-2025s.sln", "."]
COPY ["WebApplication6/Backend.csproj", "WebApplication6/"]
RUN dotnet restore "pelican-magazine-backend-2025s.sln"

# 2. Копируем остальные файлы
COPY . .

# 3. Собираем проект
WORKDIR "/src/WebApplication6"
RUN dotnet build "Backend.csproj" -c Release -o /app/build

# Publish Stage (используем тот же build-образ)
FROM build AS publish
WORKDIR "/src/WebApplication6"
RUN dotnet publish "Backend.csproj" -c Release -o /app/publish

# Final Stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0-preview AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Backend.dll"]

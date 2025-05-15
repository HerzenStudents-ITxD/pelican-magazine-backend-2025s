# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy project files and restore dependencies
COPY . ./
RUN dotnet restore --no-cache --source https://api.nuget.org/v3/index.json

# Build and publish the application
COPY . ./
RUN dotnet dev-certs https --trust
RUN dotnet publish -c Release -o out

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/out .
COPY --from=build /root/.dotnet/corefx/cryptography/x509stores/my/* /root/.dotnet/corefx/cryptography/x509stores/my/

ENV PATH="${PATH}:/usr/bin/dotnet"
EXPOSE 80
EXPOSE 443

ENTRYPOINT ["dotnet", "backend.dll"]

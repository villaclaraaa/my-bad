# =========================
# Build and publish
# =========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# Get latest Node.js to build Angular later
RUN apt-get update && apt-get install -y curl \
    && curl -fsSL https://deb.nodesource.com/setup_20.x | bash - \
    && apt-get install -y nodejs \
    && npm install -g npm@11 \
    && rm -rf /var/lib/apt/lists/*

# Copy project files
COPY src/api/Mybad.API/*.csproj src/api/Mybad.API/
COPY src/api/Mybad.Core/*.csproj src/api/Mybad.Core/
COPY src/api/Mybad.Core.Services/Mybad.Services.OpenDota/*.csproj src/api/Mybad.Core.Services/Mybad.Services.OpenDota/
COPY src/api/Mybad.Storage/*.csproj src/api/Mybad.Storage/
COPY src/ui/front/Mybad.AngularFront/*.esproj src/ui/front/Mybad.AngularFront/

# Restore dependencies
RUN dotnet restore src/api/Mybad.API/Mybad.API.csproj

# Copy full source
COPY src/ ./src/

# Before dotnet publish remove broken node_modules and install them again
WORKDIR /source/src/ui/front/Mybad.AngularFront
RUN rm -rf node_modules package-lock.json \
    && npm install

# Return to workdir
WORKDIR /source

# Publish (this runs Angular build internally)
RUN dotnet publish src/api/Mybad.API/Mybad.API.csproj \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

# =========================
# Runtime
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS runtime
WORKDIR /app

# Expose port 8080 to be available
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Mybad.API.dll"]
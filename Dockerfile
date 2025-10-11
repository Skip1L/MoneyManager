# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution file first for better layer caching
COPY ["MoneyManagerAPI/MoneyManagerAPI.sln", "."]

# Copy main project
COPY ["MoneyManagerAPI/MoneyManagerAPI.csproj", "MoneyManagerAPI/"]

# Copy all other project files
COPY ["DAL/DAL.csproj", "DAL/"]
COPY ["Domain/Domain.csproj", "Domain/"]
COPY ["DTOs/DTOs.csproj", "DTOs/"]
COPY ["Presentation/Presentation.csproj", "Presentation/"]
COPY ["Services/Services.csproj", "Services/"]

# Restore dependencies with clean cache
RUN dotnet restore "MoneyManagerAPI/MoneyManagerAPI.csproj" --force

# Copy everything else
COPY . .

# Build and publish
WORKDIR "/src/MoneyManagerAPI"
RUN dotnet publish "MoneyManagerAPI.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Install wget for health checks
RUN apt-get update && \
    apt-get install -y --no-install-recommends wget && \
    rm -rf /var/lib/apt/lists/*

# Copy published application
COPY --from=build /app/publish .

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=30s --retries=3 \
    CMD wget --no-verbose --tries=1 --spider http://localhost:5000/health || exit 1

# Expose ports
EXPOSE 5000
EXPOSE 5001

# Environment variables
ENV ASPNETCORE_URLS=http://+:5000;https://+:5001
ENV ASPNETCORE_ENVIRONMENT=Production
ENV DOTNET_RUNNING_IN_CONTAINER=true

ENTRYPOINT ["dotnet", "MoneyManagerAPI.dll"]
# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution file first for better layer caching
COPY ["MoneyManagerAPI/MoneyManagerAPI.sln", "."]
COPY ["MoneyManagerAPI/MoneyManagerAPI.csproj", "MoneyManagerAPI/"]
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

# Install openssl for certificate generation
RUN apt-get update && \
    apt-get install -y openssl && \
    rm -rf /var/lib/apt/lists/*

# Create directory for certificates
RUN mkdir -p /https

# Generate self-signed certificate
RUN openssl req -x509 -newkey rsa:4096 -sha256 -days 3650 -nodes \
    -keyout /https/aspnetapp.key \
    -out /https/aspnetapp.crt \
    -subj "/CN=moneymanager-api" \
    -addext "subjectAltName=DNS:moneymanager-api,DNS:localhost,IP:127.0.0.1" && \
    openssl pkcs12 -export -out /https/aspnetapp.pfx \
    -inkey /https/aspnetapp.key \
    -in /https/aspnetapp.crt \
    -passout pass:password && \
    chmod 644 /https/aspnetapp.pfx

# Copy published application
COPY --from=build /app/publish .

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=90s --retries=3 \
    CMD wget --no-verbose --tries=1 --spider http://localhost:5000/health || exit 1

# Expose ports
EXPOSE 5000
EXPOSE 5001

# Environment variables
ENV ASPNETCORE_URLS=http://+:5000;https://+:5001
ENV ASPNETCORE_Kestrel__Certificates__Default__Password=password
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/https/aspnetapp.pfx
ENV DOTNET_RUNNING_IN_CONTAINER=true

ENTRYPOINT ["dotnet", "MoneyManagerAPI.dll"]
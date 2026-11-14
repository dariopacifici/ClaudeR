# Multi-stage build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS builder
WORKDIR /src

# Copy solution files
COPY Clauder.slnx .
COPY Clauder.API/ ./Clauder.API/
COPY Clauder.Domain/ ./Clauder.Domain/
COPY Clauder.Data/ ./Clauder.Data/

# Build
RUN dotnet restore Clauder.slnx
RUN dotnet publish Clauder.API/Clauder.API.csproj -c Release -o /app/publish

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Install OpenSSL for certificate generation
RUN apt-get update && apt-get install -y openssl && rm -rf /var/lib/apt/lists/*

# Copy published app
COPY --from=builder /app/publish .

# Expose ports
EXPOSE 5000 5001

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=40s --retries=3 \
    CMD dotnet /app/Clauder.API.dll --check-health || exit 1

# Set environment
ENV ASPNETCORE_URLS=http://+:5000;https://+:5001
ENV ASPNETCORE_ENVIRONMENT=Production

# Run app
ENTRYPOINT ["dotnet", "Clauder.API.dll"]

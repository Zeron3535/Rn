FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy csproj and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/bin/Release/net8.0/publish/ ./

# Install curl for healthcheck
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Make port 8080 available
ENV ASPNETCORE_URLS=http://+:8080
ENV PORT=8080
EXPOSE 8080

# Environment variables for sensitive information
# These will be overridden by actual environment variables when deployed
ENV TELEGRAM_API_KEY=""
ENV TELEGRAM_CHAT_ID=""
ENV ADMIN_PASSWORD=""

# Add healthcheck with improved error handling and longer start period
HEALTHCHECK --interval=10s --timeout=5s --start-period=30s --retries=5 \
  CMD curl -f http://localhost:8080/health -o /dev/null -s || exit 1

# Ensure the app directory is writable
RUN chmod -R 777 /app

# Create a startup script
RUN echo '#!/bin/bash\n\
echo "Starting application..."\n\
echo "Checking directories..."\n\
mkdir -p /app/data\n\
mkdir -p /app/logs\n\
chmod -R 777 /app/data\n\
chmod -R 777 /app/logs\n\
echo "Directory permissions set"\n\
echo "Starting TinaKuafor..."\n\
exec dotnet TinaKuafor.dll\n\
' > /app/start.sh && chmod +x /app/start.sh

# Run the startup script on container startup
ENTRYPOINT ["/app/start.sh"]

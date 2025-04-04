# Base image for runtime with necessary dependencies (Debian)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

# Install dependencies on Debian
RUN apt-get update && apt-get install -y \
    libgdiplus \
    libc6-dev \
    libx11-dev \
    && ln -s /lib/x86_64-linux-gnu/libdl.so.2 /lib/x86_64-linux-gnu/libdl.so \
    && ln -s libgdiplus.so gdiplus.dll 

# Set working directory
WORKDIR /app

# Define listening port
ARG LISTEN_PORT=8080
EXPOSE ${LISTEN_PORT}

# Image for project build (Alpine)
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build

# Set working directory
WORKDIR /src

# Environment variables
ENV LISTEN_PORT=$LISTEN_PORT
ENV ASPNETCORE_ENVIRONMENT=$ASPNETCORE_ENVIRONMENT
ENV SYNCFUSION_LICENSE_KEY=$SYNCFUSION_LICENSE_KEY

# Copy and restore dependencies
COPY ["pdf_viewer.csproj", "./"]

# Install dependencies
RUN dotnet restore "./pdf_viewer.csproj"

# Verify installed dependencies
RUN dotnet list "./pdf_viewer.csproj" package

# Copy source code
COPY . .

# Build the project
RUN dotnet build -c Release -o /app

# Publish the application
FROM build AS publish
RUN dotnet publish -c Release -o /app

# Final image with published code (Debian)
FROM base AS final

# Set working directory
WORKDIR /app

# Copy files from the publish stage
COPY --from=publish /app .

# Define entry point
ENTRYPOINT ["dotnet", "/app/pdf_viewer.dll"]

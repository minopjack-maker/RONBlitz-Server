# ===== BUILD STAGE =====
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files
COPY RONBlitz.Server.csproj ./
RUN dotnet restore RONBlitz.Server.csproj

# Copy the rest of the app and build
COPY . .
RUN dotnet publish RONBlitz.Server.csproj -c Release -o /app/publish

# ===== RUNTIME STAGE =====
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Copy published output from build stage
COPY --from=build /app/publish .

# Expose the web server port
EXPOSE 8080

# Set environment variables for Render or other hosts
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Start the app
ENTRYPOINT ["dotnet", "RONBlitz.Server.dll"]
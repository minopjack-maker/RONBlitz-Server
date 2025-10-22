# Use the official ASP.NET Core 9.0 runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Copy your prebuilt publish output
COPY publish/ .

# Expose port 8080 for Render
EXPOSE 8080

# Configure ASP.NET environment
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Start the app
ENTRYPOINT ["dotnet", "RONBlitz.Server.dll"]

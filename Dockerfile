# Use the official .NET SDK image to build and run
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy everything
COPY . .

# Restore and publish the app
RUN dotnet restore
RUN dotnet publish -c Release -o out

# Use the ASP.NET runtime image for the final container
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/out .

# Expose port 8080 (Render uses this)
EXPOSE 8080

# Run the app
ENTRYPOINT ["dotnet", "RONBlitz.Server.dll"]

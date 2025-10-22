# Stage 1 — Build the app
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy everything and restore dependencies
COPY . .
RUN dotnet restore "RONBlitz.Server.csproj"

# Build and publish directly to /app/out
RUN dotnet publish "RONBlitz.Server.csproj" -c Release -o /app/out

# Stage 2 — Runtime container
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Copy published output from the build stage
COPY --from=build /app/out .

# Expose Render’s default port
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Run the app
ENTRYPOINT ["dotnet", "RONBlitz.Server.dll"]

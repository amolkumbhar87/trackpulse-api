FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy solution and project files
COPY ["TrackPulse.sln", "./"]
COPY ["api/TrackPulse.API/TrackPulse.API.csproj", "api/TrackPulse.API/"]
COPY ["api/TrackPulse.DataModels/TrackPulse.DataModels.csproj", "api/TrackPulse.DataModels/"]
COPY ["api/TrackPulse.Interfaces/TrackPulse.Interfaces.csproj", "api/TrackPulse.Interfaces/"]
COPY ["api/TrackPulse.Repository/TrackPulse.Repository.csproj", "api/TrackPulse.Repository/"]
COPY ["api/TrackPulse.Services/TrackPulse.Services.csproj", "api/TrackPulse.Services/"]

# Restore dependencies
RUN dotnet restore "TrackPulse.sln"

# Copy the rest of the code
COPY . .

# Build the API project
WORKDIR "/src/api/TrackPulse.API"
RUN dotnet build "TrackPulse.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
WORKDIR "/src/api/TrackPulse.API"
RUN dotnet publish "TrackPulse.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TrackPulse.API.dll"]
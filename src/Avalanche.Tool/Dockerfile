# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["NuGet.Config", "."]
COPY ["src/Avalanche.Tool/Avalanche.Tool.csproj", "src/Avalanche.Tool/"]
COPY ["src/Avalanche.Domain/Avalanche.Domain.csproj", "src/Avalanche.Domain/"]
COPY ["src/Avalanche.ReadModel/Avalanche.ReadModel.csproj", "src/Avalanche.ReadModel/"]
COPY ["src/Avalanche.Runner/Avalanche.Runner.csproj", "src/Avalanche.Runner/"]
COPY ["src/Avalanche.WriteModel/Avalanche.WriteModel.csproj", "src/Avalanche.WriteModel/"]
COPY ["src/Broadcast/Broadcast.csproj", "src/Broadcast/"]
RUN dotnet restore "./src/Avalanche.Tool/Avalanche.Tool.csproj"
COPY . .
WORKDIR "/src/src/Avalanche.Tool"
RUN dotnet build "./Avalanche.Tool.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Avalanche.Tool.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Avalanche.Tool.dll"]
# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

USER root
RUN chmod -R 755 /var/lib/apt/lists
RUN apt-get update && \
    apt-get install -y locales && \
    locale-gen en_US.UTF-8 && \
    echo "LANG=en_US.UTF-8" > /etc/locale.conf && \
    dpkg-reconfigure -f noninteractive locales

ENV LANG en_US.UTF-8
ENV LANGUAGE en_US:en
ENV LC_ALL en_US.UTF-8

ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["TaskManagementSystem/TaskManagementSystem.csproj", "TaskManagementSystem/"]
COPY ["TaskManagementSystem.DataAccess/TaskManagementSystem.DataAccess.csproj", "TaskManagementSystem.DataAccess/"]
COPY ["TaskManagementSystem.Common/TaskManagementSystem.Common.csproj", "TaskManagementSystem.Common/"]
RUN dotnet restore "./TaskManagementSystem/TaskManagementSystem.csproj"
COPY . .
WORKDIR "/src/TaskManagementSystem"
RUN dotnet build "./TaskManagementSystem.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./TaskManagementSystem.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["sh", "-c", "dotnet ef database update --no-build && dotnet TaskManagementSystem.dll"]
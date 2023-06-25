#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["WarOfMinds.WebApi/WarOfMinds.WebApi.csproj", "WarOfMinds.WebApi/"]
COPY ["WarOfMinds.Common/WarOfMinds.Common.csproj", "WarOfMinds.Common/"]
COPY ["WarOfMinds.Context/WarOfMinds.Context.csproj", "WarOfMinds.Context/"]
COPY ["WarOfMinds.Repositories/WarOfMinds.Repositories.csproj", "WarOfMinds.Repositories/"]
COPY ["WarOfMinds.Services/WarOfMinds.Services.csproj", "WarOfMinds.Services/"]
RUN dotnet restore "WarOfMinds.WebApi/WarOfMinds.WebApi.csproj"
COPY . .
WORKDIR "/src/WarOfMinds.WebApi"
RUN dotnet build "WarOfMinds.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WarOfMinds.WebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WarOfMinds.WebApi.dll"]


# Add SQL Server support
# Install SQL Server tools and libraries
RUN apt-get update \
    && apt-get install -y curl gnupg \
    && curl https://packages.microsoft.com/keys/microsoft.asc | apt-key add - \
    && curl https://packages.microsoft.com/config/debian/10/prod.list > /etc/apt/sources.list.d/mssql-release.list \
    && apt-get update \
    && ACCEPT_EULA=Y apt-get install -y msodbcsql17 mssql-tools unixodbc-dev \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*

# Set up SQL Server connection strings and configuration
ENV SQL_SERVER_NAME="(localdb)\\mssqllocaldb"
ENV SQL_SERVER_PORT=1433
ENV SQL_SERVER_DB="WarOfMindsDB"

# Set up entry point script to wait for SQL Server to be ready before running the application
COPY docker-entrypoint.sh .
RUN chmod +x docker-entrypoint.sh
ENTRYPOINT ["./docker-entrypoint.sh", "dotnet", "WarOfMinds.WebApi.dll"]
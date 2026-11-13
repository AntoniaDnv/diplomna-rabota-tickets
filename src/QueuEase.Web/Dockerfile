# Built from the repository root (see docker-compose.yml), because the backend needs
# three projects (Web, Services, Data) and the shared files global.json and Directory.Build.props.

# Stage 1: build. The SDK image has the compiler; it is large and is not shipped.
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /source

# Only the project files first: while the package list is unchanged,
# Docker reuses the cached "dotnet restore" layer.
COPY global.json Directory.Build.props ./
COPY src/QueuEase.Data/QueuEase.Data.csproj src/QueuEase.Data/
COPY src/QueuEase.Services/QueuEase.Services.csproj src/QueuEase.Services/
COPY src/QueuEase.Web/QueuEase.Web.csproj src/QueuEase.Web/
RUN dotnet restore src/QueuEase.Web/QueuEase.Web.csproj

COPY src/ src/
RUN dotnet publish src/QueuEase.Web/QueuEase.Web.csproj -c Release -o /app --no-restore

# Stage 2: run. The final image has only the ASP.NET Core runtime and the compiled application.
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .

# Official .NET images listen on port 8080 and include a user without administrator rights.
EXPOSE 8080
USER $APP_UID
ENTRYPOINT ["dotnet", "QueuEase.Web.dll"]

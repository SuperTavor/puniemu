# ==========================
# Build stage
# ==========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY . .

RUN dotnet restore Puniemu.csproj

RUN dotnet publish \
    Puniemu.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# ==========================
# Runtime stage
# ==========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "Puniemu.dll"]